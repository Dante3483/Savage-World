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
        [SerializeField] private SpriteMask _spriteMask;

        [Header("Tiles")]
        [SerializeField] private int _orderInBlockLayer;
        [SerializeField] private int _orderInBackgroundLayer;
        [SerializeField] private int _orderInLiquidLayer;
        private Tile[,] _tiles;

        [Header("Mask")]
        [SerializeField] private Sprite _maskSprite;
        [SerializeField] private Texture2D _maskTexture;
        private Color32[] _emptyTile;
        private Color32 _alphaZero = new Color32(0, 0, 0, 0);

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
            _spriteMask = GetComponent<SpriteMask>();

            _tiles = new Tile[_width, _height];
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    GameObject gameObject = new GameObject("Tile", typeof(Tile));
                    gameObject.transform.parent = transform;
                    gameObject.transform.position = new Vector2(x, y) + _tilesOffset;
                    Tile tile = gameObject.GetComponent<Tile>();
                    tile.Tilemap = this;
                    _tiles[x, y] = tile;
                }
            }

            InitializeMask();
        }

        private void FixedUpdate()
        {
            transform.position = Vector3Int.FloorToInt(Camera.main.transform.position) - new Vector3Int(_width / 2, _height / 2, -30);
        }

        private void LateUpdate()
        {
            UpdateMask();
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

        private void InitializeMask()
        {
            _emptyTile = new Color32[16 * 16];

            for (int i = 0; i < 16 * 16; i++)
            {
                _emptyTile[i] = _alphaZero;
            }

            _maskTexture = new Texture2D(_width * 16, _height * 16, TextureFormat.Alpha8, false);
            _maskTexture.wrapMode = TextureWrapMode.Clamp;
            _maskTexture.filterMode = FilterMode.Point;

            _maskSprite = Sprite.Create(_maskTexture, new Rect(0, 0, _width * 16, _height * 16), Vector2.zero, 16);

            _spriteMask.sprite = _maskSprite;
        }

        public void UpdateMask()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    if (_tiles[x, y].SpriteForMask != null)
                    {
                        _maskTexture.SetPixels32(x * 16, y * 16, 16, 16, BlocksAtlas.BlocksColorArrayBySprite[_tiles[x, y].SpriteForMask]);
                    }
                    else
                    {
                        _maskTexture.SetPixels32(x * 16, y * 16, 16, 16, _emptyTile);
                    }
                }
            }
            _maskTexture.Apply();
        }
        #endregion
    }
}