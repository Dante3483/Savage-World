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
        #endregion

        #region Public fields

        #endregion

        #region Properties
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
        private void Start()
        {
            _blockLayer.UpdateOrderInLayer(_tilemap.OrderInBlockLayer);
            _backgroundLayer.UpdateOrderInLayer(_tilemap.OrderInBackgroundLayer);
            _liquidLayer.UpdateOrderInLayer(_tilemap.OrderInLiquidLayer);

            _liquidLayerSpriteRenderer.sharedMaterial = _tilemap.LiquidMaterial;
        }

        public void UpdateSprite(TileSprites tileSprites)
        {
            _blockLayer.UpdateSprite(tileSprites.BlockSprite);
            _backgroundLayer.UpdateSprite(tileSprites.BackgroundSprite);
            _liquidLayer.UpdateSprite(tileSprites.LiquidSprite);
        }
        #endregion
    }
}
