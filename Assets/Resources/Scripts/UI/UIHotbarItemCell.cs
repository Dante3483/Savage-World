using TMPro;
using UnityEngine;

public class UIHotbarItemCell : UIStorageItemCell
{
    #region Private fields
    [SerializeField] private TMP_Text _hotbarNumberTxt;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public TMP_Text HotbarNumberTxt
    {
        get
        {
            return _hotbarNumberTxt;
        }

        set
        {
            _hotbarNumberTxt = value;
        }
    }

    public override ItemLocations ItemLocation => ItemLocations.Hotbar;
    #endregion

    #region Methods

    #endregion
}
