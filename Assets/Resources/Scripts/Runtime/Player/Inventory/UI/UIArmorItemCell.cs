using SavageWorld.Runtime.Enums.Others;
using SavageWorld.Runtime.Enums.Types;
using UnityEngine;

namespace SavageWorld.Runtime.Player.Inventory.UI
{
    public class UIArmorItemCell : UIWearableItemCell
    {
        #region Private fields
        [SerializeField]
        private ArmorTypes _armorType;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public override ItemLocations ItemLocation => ItemLocations.Armor;

        public ArmorTypes ArmorType
        {
            get
            {
                return _armorType;
            }
        }
        #endregion

        #region Methods

        #endregion
    }
}