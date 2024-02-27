using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class UIStorageItemCell : UIItemCell
{
    #region Private fields
    [SerializeField] private TMP_Text _quantityTxt;

    private StringBuilder _stringBuilder = new StringBuilder();
    private char[] _quantityCharArray4Digit = new char[4];
    private char[] _quantityCharArray3Digit = new char[3];
    private char[] _quantityCharArray2Digit = new char[2];
    private char[] _quantityCharArray1Digit = new char[1];
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public override ItemLocations ItemLocation => ItemLocations.Storage;
    #endregion

    #region Methods
    public override void ResetData()
    {
        base.ResetData();
        _quantityTxt.gameObject.SetActive(false);
    }

    public void SetData(Sprite sprite, int quantity)
    {
        _quantityTxt.gameObject.SetActive(true);
        if (quantity < 10)
        {
            Converter.FromIntToCharArray(quantity, _quantityCharArray1Digit);
            _quantityTxt.SetText(_quantityCharArray1Digit);
        }
        else if (quantity < 100)
        {
            Converter.FromIntToCharArray(quantity, _quantityCharArray2Digit);
            _quantityTxt.SetText(_quantityCharArray2Digit);
        }
        else if (quantity < 1000)
        {
            Converter.FromIntToCharArray(quantity, _quantityCharArray3Digit);
            _quantityTxt.SetText(_quantityCharArray3Digit);
        }
        else
        {
            Converter.FromIntToCharArray(quantity, _quantityCharArray4Digit);
            _quantityTxt.SetText(_quantityCharArray4Digit);
        }
        SetData(sprite);
    }
    #endregion
}
