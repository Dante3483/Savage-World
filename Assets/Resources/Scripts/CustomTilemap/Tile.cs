using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap
{
    public class Tile : MonoBehaviour
    {
        #region Private fields
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private TileLayer _blockLayer;
        [SerializeField] private TileLayer _backgroundLayer;
        [SerializeField] private TileLayer _liquidLayer;
        [SerializeField] private SpriteRenderer _blockLayerSpriteRenderer;
        [SerializeField] private SpriteRenderer _backgroundLayerSpriteRenderer;
        [SerializeField] private SpriteRenderer _liquidLayerSpriteRenderer;

        private Sprite _spriteForMask;
        private bool _firstInitialization;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public Sprite SpriteForMask
        {
            get
            {
                return _spriteForMask;
            }

            set
            {
                _spriteForMask = value;
            }
        }

        public Tilemap Tilemap
        {
            get
            {
                return _tilemap;
            }

            set
            {
                _tilemap = value;
            }
        }
        #endregion

        #region Methods
        private void Awake()
        {
            GameObject blockLayerGameObject = new GameObject("Block", typeof(SpriteRenderer), typeof(TileLayer));
            GameObject backgroundLayerGameObject = new GameObject("Background", typeof(SpriteRenderer), typeof(TileLayer));
            GameObject liquidLayerGameObject = new GameObject("Liquid", typeof(SpriteRenderer), typeof(TileLayer));

            blockLayerGameObject.transform.parent = transform;
            backgroundLayerGameObject.transform.parent = transform;
            liquidLayerGameObject.transform.parent = transform;

            _blockLayer = blockLayerGameObject.GetComponent<TileLayer>();
            _backgroundLayer = backgroundLayerGameObject.GetComponent<TileLayer>();
            _liquidLayer = liquidLayerGameObject.GetComponent<TileLayer>();

            _blockLayerSpriteRenderer = blockLayerGameObject.GetComponent<SpriteRenderer>();
            _backgroundLayerSpriteRenderer = backgroundLayerGameObject.GetComponent<SpriteRenderer>();
            _liquidLayerSpriteRenderer = liquidLayerGameObject.GetComponent<SpriteRenderer>();

            _firstInitialization = true;
        }

        private void Start()
        {
            if (_firstInitialization)
            {
                _blockLayer.UpdateOrderInLayer(_tilemap.OrderInBlockLayer);
                _backgroundLayer.UpdateOrderInLayer(_tilemap.OrderInBackgroundLayer);
                _liquidLayer.UpdateOrderInLayer(_tilemap.OrderInLiquidLayer);
                _firstInitialization = false;
            }
        }

        public void UpdateSprite(TileSprites tileSprites)
        {
            _blockLayer.UpdateSprite(tileSprites.BlockSprite);
            _backgroundLayer.UpdateSprite(tileSprites.BackgroundSprite);
            _liquidLayer.UpdateSprite(tileSprites.LiquidSprite);
            SetSpriteForMask(tileSprites);
        }

        private void SetSpriteForMask(TileSprites tileSprites)
        {
            if (tileSprites.BackgroundSprite != null)
            {
                SpriteForMask = tileSprites.BackgroundSprite;
                return;
            }
            if (tileSprites.LiquidSprite != null)
            {
                SpriteForMask = tileSprites.LiquidSprite;
                return;
            }
            if (tileSprites.BlockSprite != null)
            {
                SpriteForMask = tileSprites.BlockSprite;
                return;
            }
            SpriteForMask = null;
        }
        #endregion
    }
}
