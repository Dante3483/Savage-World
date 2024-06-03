using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHotbarItemCell : MonoBehaviour
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private RectTransform _content;
    [SerializeField] private Image _frameImage;
    [SerializeField] private Image _itemImage;
    [SerializeField] private TMP_Text _quantityTxt;
    [SerializeField] private TMP_Text _hotbarNumberTxt;
    [SerializeField] private Vector2 _selectedCellSize;
    [SerializeField] private Vector2 _unselectedCellSize;

    private char[] _quantityCharArray4Digit = new char[4];
    private char[] _quantityCharArray3Digit = new char[3];
    private char[] _quantityCharArray2Digit = new char[2];
    private char[] _quantityCharArray1Digit = new char[1];
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
    #endregion

    #region Methods
    private void Awake()
    {
        ResetData();
    }

    public void ResetData()
    {
        _itemImage.gameObject.SetActive(false);
        _quantityTxt.gameObject.SetActive(false);
    }

    public void SetSprite(Sprite sprite)
    {
        if (sprite == null)
        {
            ResetData();
            return;
        }
        _itemImage.gameObject.SetActive(true);
        _itemImage.sprite = sprite;
    }

    public void SetQuantity(int quantity)
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
    }

    public void Select()
    {
        _frameImage.gameObject.SetActive(true);
        _content.sizeDelta = _selectedCellSize;
    }

    public void Deselect()
    {
        _frameImage.gameObject.SetActive(false);
        _content.sizeDelta = _unselectedCellSize;
    }
    #endregion
}
