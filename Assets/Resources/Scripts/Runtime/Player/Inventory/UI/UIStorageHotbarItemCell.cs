using TMPro;
using UnityEngine;

public class UIStorageHotbarItemCell : UIStorageItemCell
{
    #region Private fields
    [SerializeField]
    private TMP_Text _hotbarNumberTxt;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public override ItemLocations ItemLocation => ItemLocations.Hotbar;
    #endregion

    #region Methods
    public void SetHotbarNumber(string number)
    {
        _hotbarNumberTxt.text = number;
    }
    #endregion
}
