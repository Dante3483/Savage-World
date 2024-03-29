using UnityEngine;
using UnityEngine.Tilemaps;

namespace CustomTilemap
{
    public class Tilemap : MonoBehaviour
    {
        #region Private fields
        [Header("Main")]
        [SerializeField] private int _width;
        [SerializeField] private int _height;

        [Header("Solid tilemap")]
        [SerializeField] private UnityEngine.Tilemaps.Tilemap _solidTilemap;
        [SerializeField] private SolidRuleTile _solidRuleTIle;
        [SerializeField] private CornerRuleTile _cornerRuleTile;

        [Header("Blocks tilemap")]
        [SerializeField] private Tile _tilePrefab;
        [SerializeField] private Transform _blocksTilemap;
        [SerializeField] private Vector2 _tilesOffset = new Vector2(0.5f, 0.5f);

        [Header("Tile damage")]
        [SerializeField] private Sprite[] _blockDamageSprites;
        [SerializeField] private Sprite[] _wallDamageSprites;

        private Vector3Int _currentPosition;
        private Vector3Int _prevPosition;
        private RectInt _currentAreaRect;
        private RectInt _prevAreaRect;
        private TileSprites _tileSprites;
        private Vector3Int[] _solidTilesCoords;
        private TileBase[] _solidTiles;
        private Tile[,] _tiles;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        private void Awake()
        {
            InitializeTiles();
            _solidTilesCoords = new Vector3Int[_width * _height * 2];
            _solidTiles = new TileBase[_width * _height * 2];
            _prevAreaRect = new RectInt(new Vector2Int(0, 0), new Vector2Int(_width, _height));
            _currentAreaRect = new RectInt(new Vector2Int(0, 0), new Vector2Int(_width, _height));
            _prevPosition = Vector3Int.FloorToInt(Camera.main.transform.position) - new Vector3Int(_width / 2, _height / 2, -30);
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

        private void UpdateTilemap()
        {
            int differenceX = _currentPosition.x - _prevPosition.x;
            int differenceY = _currentPosition.y - _prevPosition.y;

            if (differenceX != 0 || differenceY != 0)
            {
                ClearUnnecessarySolidTiles();
            }

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    UpdateTileData(x, y);
                }
            }

            _solidTilemap.SetTiles(_solidTilesCoords, _solidTiles);
        }

        private void ClearUnnecessarySolidTiles()
        {
            int length = _width * _height;
            int i = 0;
            _prevAreaRect.position = new Vector2Int(_prevPosition.x, _prevPosition.y);
            _currentAreaRect.position = new Vector2Int(_currentPosition.x, _currentPosition.y);

            foreach (Vector2Int position in _prevAreaRect.allPositionsWithin)
            {
                if (!_currentAreaRect.Contains(position))
                {
                    _solidTilesCoords[length + i].x = position.x;
                    _solidTilesCoords[length + i].y = position.y;
                }
                else
                {
                    _solidTilesCoords[length + i].x = -1;
                    _solidTilesCoords[length + i].y = -1;
                }
                i++;
            }
        }

        private void UpdateTileData(int x, int y)
        {
            Vector3Int blockPosition = new Vector3Int();
            byte blockDamage;
            byte wallDamage;

            blockPosition.x = _currentPosition.x + x;
            blockPosition.y = _currentPosition.y + y;
            ref WorldCellData blockData = ref WorldDataManager.Instance.GetWorldCellData(blockPosition.x, blockPosition.y);

            _tileSprites.BlockSprite = blockData.GetBlockSprite();

            _tileSprites.WallSprite = blockData.GetWallSprite();

            _tileSprites.LiquidSprite = null;
            if (blockData.IsEmptyForLiquid() && blockData.IsLiquid())
            {
                _tileSprites.LiquidSprite = blockData.GetLiquidSprite();
            }

            _tileSprites.BlockDamageSprite = null;
            blockDamage = blockData.BlockDamagePercent;
            if (blockDamage != 0)
            {
                int index = Mathf.CeilToInt(blockDamage / (100f / _blockDamageSprites.Length)) - 1;
                _tileSprites.BlockDamageSprite = _blockDamageSprites[index];
            }

            _tileSprites.WallDamageSprite = null;
            wallDamage = blockData.WallDamagePercent;
            if (wallDamage != 0)
            {
                int index = Mathf.CeilToInt(wallDamage / (100f / _wallDamageSprites.Length)) - 1;
                _tileSprites.WallDamageSprite = _wallDamageSprites[index];
            }

            _tiles[x, y].UpdateSprites(_tileSprites);

            _solidTilesCoords[x * _height + y].x = blockPosition.x;
            _solidTilesCoords[x * _height + y].y = blockPosition.y;
            _solidTiles[x * _height + y] = null;
            if (blockData.IsSolid())
            {
                _solidTiles[x * _height + y] = _solidRuleTIle;
            }
            else if (WorldDataManager.Instance.IsSolid(blockPosition.x, blockPosition.y - 1))
            {
                bool isLeftSolid = WorldDataManager.Instance.IsSolid(blockPosition.x - 1, blockPosition.y);
                bool isRightSolid = WorldDataManager.Instance.IsSolid(blockPosition.x + 1, blockPosition.y);
                if ((isLeftSolid && !isRightSolid) || (!isLeftSolid && isRightSolid))
                {
                    _solidTiles[x * _height + y] = _cornerRuleTile;
                }
            }
        }
        #endregion
    }
}