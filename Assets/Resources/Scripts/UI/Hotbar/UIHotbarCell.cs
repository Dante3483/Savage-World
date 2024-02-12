using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIHotbarCell : MonoBehaviour, IPointerClickHandler
{
    #region Private fields
    [SerializeField] private Image _selectedOutline;
    [SerializeField] private Image _itemImage;
    [SerializeField] private Color _selectedCellColor;
    [SerializeField] private TMP_Text _quantityTxt;
    [SerializeField] private TMP_Text _keyTxt;
    [SerializeField] private bool _isSelected;
    #endregion

    #region Public fields
    public event Action<UIHotbarCell> OnLeftMouseClick;
    #endregion

    #region Properties
    public TMP_Text KeyTxt
    {
        get
        {
            return _keyTxt;
        }

        set
        {
            _keyTxt = value;
        }
    }

    public bool IsSelected
    {
        get
        {
            return _isSelected;
        }

        set
        {
            _isSelected = value;
        }
    }
    #endregion

    #region Methods
    private void Awake()
    {
        ResetData();
    }

    public void OnPointerClick(PointerEventData pointerData)
    {
        if (pointerData.button == PointerEventData.InputButton.Left)
        {
            OnLeftMouseClick?.Invoke(this);
        }
    }

    public void ResetData()
    {
        _itemImage.gameObject.SetActive(false);
    }

    public void SetData(Sprite sprite, int quantity)
    {
        if (sprite == null)
        {
            ResetData();
            return;
        }
        _itemImage.gameObject.SetActive(true);
        _itemImage.sprite = sprite;
        _quantityTxt.text = quantity + "";
    }

    public void Select()
    {
        _selectedOutline.gameObject.SetActive(true);
        IsSelected = true;
    }

    public void Deselect()
    {
        _selectedOutline.gameObject.SetActive(false);
        IsSelected = false;
    }
    #endregion
}
