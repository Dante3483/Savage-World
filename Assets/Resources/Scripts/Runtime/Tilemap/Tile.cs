using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap
{
    public class Tile : MonoBehaviour
    {
        #region Private fields
        [SerializeField] private BreakableTileLayer _blockLayer;
        [SerializeField] private BreakableTileLayer _wallLayer;
        [SerializeField] private TileLayer _liquidLayer;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        public void UpdateSprites(TileSprites tileSprites)
        {
            _blockLayer.UpdateSprite(tileSprites.BlockSprite, tileSprites.BlockDamageSprite);
            _wallLayer.UpdateSprite(tileSprites.WallSprite, tileSprites.WallDamageSprite);
            _liquidLayer.UpdateSprite(tileSprites.LiquidSprite);
        }
        #endregion
    }
}
