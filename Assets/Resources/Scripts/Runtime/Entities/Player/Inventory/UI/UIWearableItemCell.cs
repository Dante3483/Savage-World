using UnityEngine;
using UnityEngine.UI;

namespace SavageWorld.Runtime.Entities.Player.Inventory.UI
{
    public class UIWearableItemCell : UIItemCell
    {
        #region Private fields
        [SerializeField]
        private Image _silhouette;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        public override void ResetData()
        {
            base.ResetData();
            _silhouette.gameObject.SetActive(true);
        }

        public override void SetSprite(Sprite sprite)
        {
            if (sprite != null)
            {
                _silhouette.gameObject.SetActive(false);
            }
            base.SetSprite(sprite);
        }
        #endregion
    }
}