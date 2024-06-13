using UnityEngine;

namespace CustomTilemap
{
    public class BreakableTileLayer : TileLayer
    {
        #region Private fields
        [SerializeField] private SpriteRenderer _damagerSpriteRenderer;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        public void UpdateDamage(Sprite damageSprite)
        {
            _damagerSpriteRenderer.sprite = damageSprite;
        }
        #endregion
    }
}