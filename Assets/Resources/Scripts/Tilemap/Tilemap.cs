using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap
{
    public class Tilemap : MonoBehaviour
    {
        #region Private fields
        [Header("Main")]
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private Vector2 _tilesOffset = new Vector2(0.5f, 0.5f);

        [Header("Tiles")]
        [SerializeField] private Tile _tilePrefab;
        [SerializeField] private int _orderInBlockLayer;
        [SerializeField] private int _orderInBackgroundLayer;
        [SerializeField] private int _orderInLiquidLayer;
        private Tile[,] _tiles;

        [Header("Block materials")]
        [SerializeField] private Material _liquidMaterial;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public int OrderInBlockLayer
        {
            get
            {
                return _orderInBlockLayer;
            }

            set
            {
                _orderInBlockLayer = value;
            }
        }

        public int OrderInBackgroundLayer
        {
            get
            {
                return _orderInBackgroundLayer;
            }

            set
            {
                _orderInBackgroundLayer = value;
            }
        }

        public int OrderInLiquidLayer
        {
            get
            {
                return _orderInLiquidLayer;
            }

            set
            {
                _orderInLiquidLayer = value;
            }
        }

        public Material LiquidMaterial
        {
            get
            {
                return _liquidMaterial;
            }

            set
            {
                _liquidMaterial = value;
            }
        }
        #endregion

        #region Methods
        private void Awake()
        {
            _tiles = new Tile[_width, _height];
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Tile tile = Instantiate(_tilePrefab, new Vector2(x, y) + _tilesOffset, Quaternion.identity, transform);
                    tile.name = "Tile";
                    tile.Tilemap = this;
                    _tiles[x, y] = tile;
                }
            }
        }

        private void FixedUpdate()
        {
            transform.position = Vector3Int.FloorToInt(Camera.main.transform.position) - new Vector3Int(_width / 2, _height / 2, -30);
        }

        public void SetTile(Vector3 position, TileSprites tileSprites)
        {
            WorldToLocal(position, out int x, out int y);
            if (x < 0 || y < 0 || x >= _width || y >= _height)
            {
                return;
            }
            _tiles[x, y].UpdateSprite(tileSprites);
        }

        private void WorldToLocal(Vector3 position, out int x, out int y)
        {
            Vector3 result = transform.InverseTransformPoint(position);
            x = (int)result.x;
            y = (int)result.y;
        }
        #endregion
    }
}