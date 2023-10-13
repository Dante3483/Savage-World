using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace CustomTilemap
{
    public class Tilemap : MonoBehaviour
    {
        #region Private fields
        [Header("Main")]
        [SerializeField] private int _size;
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
        private Color[] _currentTile;
        private Color[] _emptyTile;
        private Color _alphaZero = new Color(0, 0, 0, 0);
        private PoolForStaticArraysGeneric<Sprite, Color> _rawTexturesPool;
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
        #endregion

        #region Methods
        private void Awake()
        {
            _spriteMask = GetComponent<SpriteMask>();

            _tiles = new Tile[_size, _size];
            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
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
            transform.position = Vector3Int.FloorToInt(Camera.main.transform.position) - new Vector3Int(_size / 2, _size / 2, -30);
        }

        public void SetTile(Vector2 position, TileSprites tileSprites)
        {
            GetWorldToLocal(position, out int x, out int y);
            if (x < 0 || y < 0 || x >= _size || y >= _size)
            {
                return;
            }
            _tiles[x, y].UpdateSprite(tileSprites);
        }

        public bool SetTiles(List<Vector2> positions, List<TileSprites> tilesSprites)
        {
            if (positions.Count != tilesSprites.Count)
            {
                return false;
            }
            for (int i = 0; i < positions.Count; i++)
            {
                SetTile(positions[i], tilesSprites[i]);
            }
            UpdateMask();
            return true;
        }

        public Vector3 GetLocalToWorld(int x, int y)
        {
            return transform.TransformPoint(new Vector2(x, y));
        }

        public void GetWorldToLocal(Vector2 position, out int x, out int y)
        {
            Vector3 result = transform.InverseTransformPoint(position);
            x = (int)result.x;
            y = (int)result.y;
        }

        public void InitializeMask()
        {
            _emptyTile = new Color[16 * 16];
            _currentTile = new Color[16 * 16];

            for (int i = 0; i < 16 * 16; i++)
            {
                _emptyTile[i] = _alphaZero;
            }

            _rawTexturesPool = new PoolForStaticArraysGeneric<Sprite, Color>();

            _maskTexture = new Texture2D(_size * 16, _size * 16);
            _maskTexture.filterMode = FilterMode.Point;

            _maskSprite = Sprite.Create(_maskTexture, new Rect(0, 0, _size * 16, _size * 16), Vector2.zero, 16);

            _spriteMask.sprite = _maskSprite;
        }

        public void UpdateMask()
        {
            for (int x = 0; x < _size; x++)
            {
                for (int y = 0; y < _size; y++)
                {
                    if (_tiles[x, y].SpriteForMask != null)
                    {
                        bool result = _rawTexturesPool.GetArray(_tiles[x, y].SpriteForMask, ref _currentTile);
                        if (!result)
                        {
                            _rawTexturesPool.SetArray(_tiles[x, y].SpriteForMask, _tiles[x, y].SpriteForMask.texture.GetPixels());
                            _rawTexturesPool.GetArray(_tiles[x, y].SpriteForMask, ref _currentTile);
                        }
                        _maskTexture.SetPixels(x * 16, y * 16, 16, 16, _currentTile);
                    }
                    else
                    {
                        _maskTexture.SetPixels(x * 16, y * 16, 16, 16, _emptyTile);
                    }
                }
            }
            _maskTexture.Apply();
        }
        #endregion
    }
}