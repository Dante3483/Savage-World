using UnityEngine;

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
