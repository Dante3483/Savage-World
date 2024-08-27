using SavageWorld.Runtime.GameSession;
using SavageWorld.Runtime.Managers;
using SavageWorld.Runtime.Player.Interactions;
using SavageWorld.Runtime.Utilities.Others;
using SavageWorld.Runtime.Utilities.Pools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SavageWorld.Runtime.WorldMap
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
        private Dictionary<Vector2Int, SolidPlatform> _usedPlatformsByPositions;
        private Dictionary<Vector2Int, HashSet<GameObject>> _setsOfRequestedObjectsByPlatformPosition;
        private HashSet<Vector2Int> _setOfPlatformPositionsToCreate;
        private HashSet<Vector2Int> _setOfPlatformPositionsToRemove;
        private ObjectsPool<SolidPlatform> _platformsPool;

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
            _currentPosition = Vector3Int.FloorToInt(GameManager.Instance.GetPlayerTransform().position) - new Vector3Int(_width / 2, _height / 2, -30);
            _blocksTilemap.position = _currentPosition;
            UpdateTilemap();
            _prevPosition = _currentPosition;
        }

        private void OnEnable()
        {
            _worldDataManager.CellDataChanged += CellDataChangedEventHandler;
            _worldDataManager.CellColliderChanged += CellColliderChangedEventHandler;
            _miningDamageController.BlockDamageChanged += BlockDamageChangedEventHandler;
            _miningDamageController.WallDamageChanged += WallDamageChangedEventHandler;
        }

        private void OnDisable()
        {
            _worldDataManager.CellDataChanged -= CellDataChangedEventHandler;
            _worldDataManager.CellColliderChanged -= CellColliderChangedEventHandler;
            _miningDamageController.BlockDamageChanged -= UpdateBlockDamageSprite;
            _miningDamageController.WallDamageChanged -= UpdateWallDamageSprite;
        }
        #endregion

        #region Public Methods
        public void AddPositionToCreatePlatform(Vector2Int position, GameObject sender)
        {
            if (!_setsOfRequestedObjectsByPlatformPosition.ContainsKey(position))
            {
                _setsOfRequestedObjectsByPlatformPosition[position] = new();
            }
            _setsOfRequestedObjectsByPlatformPosition[position].Add(sender);
            _needUpdateCompositeCollider = _setOfPlatformPositionsToCreate.Add(position);
        }

        public void AddPositionToRemovePlatform(Vector2Int position, GameObject sender)
        {
            if (_setsOfRequestedObjectsByPlatformPosition.ContainsKey(position))
            {
                _setsOfRequestedObjectsByPlatformPosition[position].Remove(sender);
                if (_setsOfRequestedObjectsByPlatformPosition[position].Count == 0)
                {
                    _setsOfRequestedObjectsByPlatformPosition.Remove(position);
                }
            }
            _needUpdateCompositeCollider = _setOfPlatformPositionsToRemove.Add(position);
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
            _usedPlatformsByPositions = new();
            _setOfPlatformPositionsToCreate = new();
            _setOfPlatformPositionsToRemove = new();
            _setsOfRequestedObjectsByPlatformPosition = new();
            _platformsPool = new(() => Instantiate(_platformPrefab, _platformsContent), _width * _height);
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
                        AddPositionToRemovePlatform(position, gameObject);
                    }
                }
                _needUpdateTiles = true;
            }
            if (_needUpdateTiles)
            {
                _needUpdateTiles = false;
                foreach (Vector2Int position in _currentAreaRect.allPositionsWithin)
                {
                    UpdateTileData(position);
                    AddPositionToCreatePlatform(position, gameObject);
                }
            }
            if (_needUpdateCompositeCollider)
            {
                UpdateCompositeCollider();
                _needUpdateCompositeCollider = false;
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
        }

        private void UpdateBlockSprite(Vector2Int position)
        {
            Vector2Int tilePositon = GetLocalTilePosition(position);
            try
            {
                Sprite sprite = _worldDataManager.GetBlockSprite(position.x, position.y);
                _tiles[tilePositon.x, tilePositon.y].UpdateBlockSprite(sprite);
            }
            catch (IndexOutOfRangeException)
            {
                Debug.LogError(tilePositon.ToString());
            }
        }

        private void UpdateWallSprite(Vector2Int position)
        {
            Vector2Int tilePositon = GetLocalTilePosition(position);
            try
            {
                Sprite sprite = _worldDataManager.GetWallSprite(position.x, position.y);
                _tiles[tilePositon.x, tilePositon.y].UpdateWallSprite(sprite);
            }
            catch (IndexOutOfRangeException)
            {
                Debug.LogError(tilePositon.ToString());
            }
        }

        private void UpdateLiquidSprite(Vector2Int position)
        {
            Vector2Int tilePositon = GetLocalTilePosition(position);
            try
            {
                Sprite sprite = _worldDataManager.GetLiquidSprite(position.x, position.y);
                _tiles[tilePositon.x, tilePositon.y].UpdateLiquidSprite(sprite);
            }
            catch (IndexOutOfRangeException)
            {
                Debug.LogError(tilePositon.ToString());
            }
        }

        private void UpdateBlockDamageSprite(Vector2Int position, float rawDamage)
        {
            Vector2Int tilePositon = GetLocalTilePosition(position);
            try
            {
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
            catch (IndexOutOfRangeException)
            {
                Debug.LogError(tilePositon.ToString());
            }
        }

        private void UpdateWallDamageSprite(Vector2Int position, float rawDamage)
        {
            Vector2Int tilePositon = GetLocalTilePosition(position);
            try
            {
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
            catch (IndexOutOfRangeException)
            {
                Debug.LogError(tilePositon.ToString());
            }
        }

        private void UpdateCompositeCollider()
        {
            foreach (Vector2Int position in _setOfPlatformPositionsToRemove)
            {
                RemovePlatform(position);
            }
            foreach (Vector2Int position in _setOfPlatformPositionsToCreate)
            {
                CreatePlatform(position);
            }
            _compositeCollider.GenerateGeometry();
            _setOfPlatformPositionsToCreate.Clear();
            _setOfPlatformPositionsToRemove.Clear();
        }

        private void CreatePlatform(Vector2Int position)
        {
            int x = position.x;
            int y = position.y;
            _usedPlatformsByPositions.TryGetValue(position, out SolidPlatform platform);
            if (platform == null)
            {
                platform = GetPlatform();
                platform.SetActive();
                platform.transform.position = position + _tilesOffset;
                _usedPlatformsByPositions.Add(position, platform);
            }
            platform.SetPolygonColliderPoints(
                _worldDataManager.GetColliderShape(x, y),
                _worldDataManager.IsColliderHorizontalFlipped(x, y));
        }

        private void RemovePlatform(Vector2Int position)
        {
            _usedPlatformsByPositions.TryGetValue(position, out SolidPlatform platform);
            if (platform != null)
            {
                if (!_setsOfRequestedObjectsByPlatformPosition.ContainsKey(position))
                {
                    _usedPlatformsByPositions.Remove(position);
                    ReleasePlatform(platform);
                }
            }
        }

        private void ReleasePlatform(SolidPlatform platform)
        {
            platform.transform.position = new(-10, -10);
            platform.SetInactive();
            _platformsPool.Release(platform);
        }

        private SolidPlatform GetPlatform()
        {
            return _platformsPool.Get();
        }

        private Vector2Int GetLocalTilePosition(Vector2Int globalPosition)
        {
            return new(globalPosition.x - _currentPosition.x, globalPosition.y - _currentPosition.y);
        }

        private bool IsInRenderArea(int x, int y)
        {
            return _currentAreaRect.Contains(new(x, y));
        }

        private void CellDataChangedEventHandler(int x, int y)
        {
            if (IsInRenderArea(x, y))
            {
                UpdateTileData(new(x, y));
            }
        }

        private void CellColliderChangedEventHandler(int x, int y)
        {
            if (IsInRenderArea(x, y))
            {
                AddPositionToCreatePlatform(new(x, y), gameObject);
            }
        }

        private void BlockDamageChangedEventHandler(Vector2Int position, float damage)
        {
            if (IsInRenderArea(position.x, position.y))
            {
                UpdateBlockDamageSprite(position, damage);
            }
        }

        private void WallDamageChangedEventHandler(Vector2Int position, float damage)
        {
            if (IsInRenderArea(position.x, position.y))
            {
                UpdateWallDamageSprite(position, damage);
            }
        }
        #endregion
    }
}