using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomTilemap
{
    public class Tilemap : MonoBehaviour
    {
        #region Private fields
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
        private TileSprites _tileSprites;
        private Tile[,] _tiles;
        #endregion

        #region Public fields
        public static Tilemap Instance;
        #endregion

        #region Properties

        #endregion

        #region Methods
        private void Awake()
        {
            Instance = this;

            InitializeTiles();
            InitializePlatforms();
            _compositeCollider = _platformsContent.GetComponent<CompositeCollider2D>();
            _prevAreaRect = new RectInt(new Vector2Int(0, 0), new Vector2Int(_width, _height));
            _currentAreaRect = new RectInt(new Vector2Int(0, 0), new Vector2Int(_width, _height));
            _prevPosition = Vector3Int.FloorToInt(Camera.main.transform.position) - new Vector3Int(_width / 2, _height / 2, -30);
        }

        private void Start()
        {
            WorldDataManager.Instance.OnColliderChanged += HandleColliderChanged;
            WorldDataManager.Instance.OnDataChanged += HandleDataChanged;
            _needUpdateTiles = true;
        }

        private void FixedUpdate()
        {
            _currentPosition = Vector3Int.FloorToInt(Camera.main.transform.position) - new Vector3Int(_width / 2, _height / 2, -30);
            _blocksTilemap.position = _currentPosition;
            UpdateTilemap();
            _prevPosition = _currentPosition;
        }

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
            int tileX = position.x - _currentPosition.x;
            int tileY = position.y - _currentPosition.y;

            ref WorldCellData blockData = ref WorldDataManager.Instance.GetWorldCellData(position.x, position.y);
            UpdateBlockSprite(ref blockData);
            UpdateWallSprite(ref blockData);
            UpdateLiquidSprite(ref blockData);
            //UpdateBlockDamageSprite(blockData.BlockDamagePercent);
            //UpdateWallDamageSprite(blockData.WallDamagePercent);
            float blockDamage = MiningDamageController.Instance.GetBlockDamage(position);
            float wallDamage = MiningDamageController.Instance.GetWallDamage(position);
            byte blockDamageId = (byte)(Mathf.Min(blockDamage / blockData.BlockData.DamageToBreak, 1) * 100);
            byte wallDamageId = (byte)(Mathf.Min(wallDamage / blockData.WallData.DamageToBreak, 1) * 100);
            UpdateBlockDamageSprite(blockDamageId);
            UpdateWallDamageSprite(wallDamageId);
            CreatePlatform(position);
            _tiles[tileX, tileY].UpdateSprites(_tileSprites);
        }

        private void UpdateBlockSprite(ref WorldCellData data)
        {
            _tileSprites.BlockSprite = data.GetBlockSprite();
        }

        private void UpdateWallSprite(ref WorldCellData data)
        {
            _tileSprites.WallSprite = data.GetWallSprite();
        }

        private void UpdateLiquidSprite(ref WorldCellData data)
        {
            _tileSprites.LiquidSprite = null;
            if (data.IsValidForLiquid && data.IsLiquid)
            {
                _tileSprites.LiquidSprite = data.GetLiquidSprite();
            }
        }

        private void UpdateBlockDamageSprite(byte damage)
        {
            _tileSprites.BlockDamageSprite = null;
            if (damage != 0)
            {
                int index = Mathf.CeilToInt(damage / (100f / _blockDamageSprites.Length)) - 1;
                _tileSprites.BlockDamageSprite = _blockDamageSprites[index];
            }
        }

        private void UpdateWallDamageSprite(byte damage)
        {
            _tileSprites.WallDamageSprite = null;
            if (damage != 0)
            {
                int index = Mathf.CeilToInt(damage / (100f / _wallDamageSprites.Length)) - 1;
                _tileSprites.WallDamageSprite = _wallDamageSprites[index];
            }
        }

        private void UpdateCompositeCollider()
        {
            _compositeCollider.GenerateGeometry();
        }

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
                WorldDataManager.Instance.GetColliderShape(x, y),
                WorldDataManager.Instance.IsColliderHorizontalFlipped(x, y));
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
                        WorldDataManager.Instance.GetColliderShape(x, y),
                        WorldDataManager.Instance.IsColliderHorizontalFlipped(x, y));
                    _needUpdateCompositeCollider = true;
                }
            }
        }
        #endregion
    }
}