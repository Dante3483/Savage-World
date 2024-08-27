using UnityEngine;

namespace SavageWorld.Runtime.WorldMap
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
        public void UpdateBlockSprite(Sprite sprite)
        {
            _blockLayer.UpdateSprite(sprite);
        }

        public void UpdateWallSprite(Sprite sprite)
        {
            _wallLayer.UpdateSprite(sprite);
        }

        public void UpdateLiquidSprite(Sprite sprite)
        {
            _liquidLayer.UpdateSprite(sprite);
        }

        public void UpdateBlockDamage(Sprite sprite)
        {
            _blockLayer.UpdateDamage(sprite);
        }

        public void UpdateWallDamage(Sprite sprite)
        {
            _wallLayer.UpdateDamage(sprite);
        }
        #endregion
    }
}
