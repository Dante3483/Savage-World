using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomTilemap
{
    public class Tilemap : Singleton<Tilemap>
    {
        #region Fields
        [Header("Main")]
        [SerializeField]
        private int _width;
        [SerializeField]
        private int _height;
        [SerializeField]
        private CompositeCollider2D _compositeCollider;

        [Header("Blocks tilemap")]
        [SerializeField]
        private Tile _tilePrefab;
        [SerializeField]
        private Transform _blocksTilemap;
        [SerializeField]
        private Vector2 _tilesOffset = new(0.5f, 0.5f);

        [Header("Tile damage")]
        [SerializeField]
        private Sprite[] _blockDamageSprites;
        [SerializeField]
        private Sprite[] _wallDamageSprites;

        [Header("Platforms")]
        [SerializeField]
        private SolidPlatform _platformPrefab;
        [SerializeField]
        private Transform _platformsContent;
        private Dictionary<Vector2Int, SolidPlatform> _listOfUsedPlatforms;
        private List<SolidPlatform> _listOfFreePlatforms;
        private bool _needUpdateCompositeCollider;
        private bool _needUpdateTiles;
        private Vector3Int _currentPosition;
        private Vector3Int _prevPosition;
        private RectInt _currentAreaRect;
        private RectInt _prevAreaRect;
        private Tile[,] _tiles;
        private WorldDataManager _worldDataManager;
        private MiningDamageController _miningDamageController;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods
        protected override void Awake()
        {
            base.Awake();
            _worldDataManager = WorldDataManager.Instance;
            _miningDamageController = MiningDamageController.Instance;
            InitializeTiles();
            InitializePlatforms();
            _compositeCollider = _platformsContent.GetComponent<CompositeCollider2D>();
            _prevAreaRect = new RectInt(new Vector2Int(0, 0), new Vector2Int(_width, _height));
            _currentAreaRect = new RectInt(new Vector2Int(0, 0), new Vector2Int(_width, _height));
            _prevPosition = Vector3Int.FloorToInt(Camera.main.transform.position) - new Vector3Int(_width / 2, _height / 2, -30);
        }

        private void Start()
        {
            _needUpdateTiles = true;
        }

        private void FixedUpdate()
        {
            _currentPosition = Vector3Int.FloorToInt(Camera.main.transform.position) - new Vector3Int(_width / 2, _height / 2, -30);
            _blocksTilemap.position = _currentPosition;
            UpdateTilemap();
            _prevPosition = _currentPosition;
        }

        private void OnEnable()
        {
            _worldDataManager.CellDataChanged += HandleDataChanged;
            _worldDataManager.CellColliderChanged += HandleColliderChanged;
            _miningDamageController.BlockDamageChanged += UpdateBlockDamageSprite;
            _miningDamageController.WallDamageChanged += UpdateWallDamageSprite;
        }

        private void OnDisable()
        {
            _worldDataManager.CellDataChanged -= HandleDataChanged;
            _worldDataManager.CellColliderChanged -= HandleColliderChanged;
            _miningDamageController.BlockDamageChanged -= UpdateBlockDamageSprite;
            _miningDamageController.WallDamageChanged -= UpdateWallDamageSprite;
        }
        #endregion

        #region Public Methods
        public void CreatePlatform(Vector2Int position)
        {
            int x = position.x;
            int y = position.y;
            _listOfUsedPlatforms.TryGetValue(position, out SolidPlatform platform);
            if (platform == null)
            {
                platform = GetFirstFreePlatform();
                platform.SetActive();
                platform.transform.position = new Vector2(x, y) + _tilesOffset;
                _listOfUsedPlatforms.Add(position, platform);
            }
            platform.SetPolygonColliderPoints(
                _worldDataManager.GetColliderShape(x, y),
                _worldDataManager.IsColliderHorizontalFlipped(x, y));
        }

        public void RemovePlatform(Vector2Int position)
        {
            _listOfUsedPlatforms.TryGetValue(position, out SolidPlatform platform);
            if (platform != null)
            {
                _listOfUsedPlatforms.Remove(position);
                SetPlatformFree(platform);
            }
        }
        #endregion

        #region Private Methods
        private void InitializeTiles()
        {
            _tiles = new Tile[_width, _height];
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Tile tile = Instantiate(_tilePrefab, new Vector2(x, y) + _tilesOffset, Quaternion.identity, _blocksTilemap);
                    tile.name = "Tile";
                    _tiles[x, y] = tile;
                }
            }
        }

        private void InitializePlatforms()
        {
            _listOfUsedPlatforms = new();
            _listOfFreePlatforms = new();
            for (int i = 0; i < _width * _height; i++)
            {
                SolidPlatform platform = Instantiate(_platformPrefab, _platformsContent);
                _listOfFreePlatforms.Add(platform);
            }
        }

        private void UpdateTilemap()
        {
            int differenceX = _currentPosition.x - _prevPosition.x;
            int differenceY = _currentPosition.y - _prevPosition.y;
            bool isPositionChanged = differenceX != 0 || differenceY != 0;
            _prevAreaRect.position = new(_prevPosition.x, _prevPosition.y);
            _currentAreaRect.position = new(_currentPosition.x, _currentPosition.y);

            if (isPositionChanged)
            {
                foreach (Vector2Int position in _prevAreaRect.allPositionsWithin)
                {
                    if (!_currentAreaRect.Contains(position))
                    {
                        RemovePlatform(position);
                    }
                }
                _needUpdateTiles = true;
                _needUpdateCompositeCollider = true;
            }
            if (_needUpdateTiles)
            {
                _needUpdateTiles = false;
                foreach (Vector2Int position in _currentAreaRect.allPositionsWithin)
                {
                    UpdateTileData(position);
                }
            }
            if (_needUpdateCompositeCollider)
            {
                _needUpdateCompositeCollider = false;
                UpdateCompositeCollider();
            }
        }

        private void UpdateTileData(Vector2Int position)
        {
            float rawBlockDamage = _miningDamageController.GetBlockDamage(position);
            float rawWallDamage = _miningDamageController.GetWallDamage(position);
            UpdateBlockSprite(position);
            UpdateWallSprite(position);
            UpdateLiquidSprite(position);
            UpdateBlockDamageSprite(position, rawBlockDamage);
            UpdateWallDamageSprite(position, rawWallDamage);
            CreatePlatform(position);
        }

        private void UpdateBlockSprite(Vector2Int position)
        {
            Vector2Int tilePositon = GetLocalTilePosition(position);
            Sprite sprite = _worldDataManager.GetBlockSprite(position.x, position.y);
            _tiles[tilePositon.x, tilePositon.y].UpdateBlockSprite(sprite);
        }

        private void UpdateWallSprite(Vector2Int position)
        {
            Vector2Int tilePositon = GetLocalTilePosition(position);
            Sprite sprite = _worldDataManager.GetWallSprite(position.x, position.y);
            _tiles[tilePositon.x, tilePositon.y].UpdateWallSprite(sprite);
        }

        private void UpdateLiquidSprite(Vector2Int position)
        {
            Vector2Int tilePositon = GetLocalTilePosition(position);
            Sprite sprite = _worldDataManager.GetLiquidSprite(position.x, position.y);
            _tiles[tilePositon.x, tilePositon.y].UpdateLiquidSprite(sprite);
        }

        private void UpdateBlockDamageSprite(Vector2Int position, float rawDamage)
        {
            Vector2Int tilePositon = GetLocalTilePosition(position);
            if (rawDamage > 0)
            {
                float maxDamage = _worldDataManager.GetBlockData(position.x, position.y).DamageToBreak;
                int damage = (int)(Mathf.Min(rawDamage / maxDamage, 1) * 100);
                if (damage != 0)
                {
                    int id = Mathf.CeilToInt(damage / (100f / _blockDamageSprites.Length)) - 1;
                    Sprite sprite = _blockDamageSprites[id];
                    _tiles[tilePositon.x, tilePositon.y].UpdateBlockDamage(sprite);
                }
            }
            else
            {
                _tiles[tilePositon.x, tilePositon.y].UpdateBlockDamage(null);
            }
        }

        private void UpdateWallDamageSprite(Vector2Int position, float rawDamage)
        {
            Vector2Int tilePositon = GetLocalTilePosition(position);
            if (rawDamage > 0)
            {
                float maxDamage = _worldDataManager.GetWallData(position.x, position.y).DamageToBreak;
                int damage = (int)(Mathf.Min(rawDamage / maxDamage, 1) * 100);
                if (damage != 0)
                {
                    int id = Mathf.CeilToInt(damage / (100f / _wallDamageSprites.Length)) - 1;
                    Sprite sprite = _wallDamageSprites[id];
                    _tiles[tilePositon.x, tilePositon.y].UpdateWallDamage(sprite);
                }
            }
            else
            {
                _tiles[tilePositon.x, tilePositon.y].UpdateWallDamage(null);
            }
        }

        private void UpdateCompositeCollider()
        {
            _compositeCollider.GenerateGeometry();
        }

        private SolidPlatform GetFirstFreePlatform()
        {
            SolidPlatform freePlatform = _listOfFreePlatforms.FirstOrDefault();
            if (freePlatform == null)
            {
                return Instantiate(_platformPrefab, _platformsContent);
            }
            _listOfFreePlatforms.Remove(freePlatform);
            freePlatform.SetActive();
            return freePlatform;
        }

        private Vector2Int GetLocalTilePosition(Vector2Int globalPosition)
        {
            return new(globalPosition.x - _currentPosition.x, globalPosition.y - _currentPosition.y);
        }

        private void SetPlatformFree(SolidPlatform usedPlatform)
        {
            usedPlatform.transform.position = new(-10, -10);
            usedPlatform.SetInactive();
            _listOfFreePlatforms.Add(usedPlatform);
        }

        private bool IsInRenderArea(int x, int y)
        {
            return _currentAreaRect.Contains(new(x, y));
        }

        private void HandleDataChanged(int x, int y)
        {
            if (IsInRenderArea(x, y))
            {
                UpdateTileData(new(x, y));
            }
        }

        private void HandleColliderChanged(int x, int y)
        {
            if (IsInRenderArea(x, y))
            {
                _listOfUsedPlatforms.TryGetValue(new(x, y), out SolidPlatform platform);
                if (platform != null)
                {
                    platform.SetPolygonColliderPoints(
                        _worldDataManager.GetColliderShape(x, y),
                        _worldDataManager.IsColliderHorizontalFlipped(x, y));
                    _needUpdateCompositeCollider = true;
                }
            }
        }
        #endregion
    }
}