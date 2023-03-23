using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIHotbarCell : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _selectedOutline;
    [SerializeField] private Image _itemImage;
    [SerializeField] private Color _selectedCellColor;
    [SerializeField] private TMP_Text _quantityTxt;
    [SerializeField] private TMP_Text _keyTxt;
    [SerializeField] private bool _isSelected;

    public event Action<UIHotbarCell> OnLeftMouseClick;

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

    public void Awake()
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
        this._itemImage.gameObject.SetActive(false);
    }

    public void SetData(Sprite sprite, int quantity)
    {
        if (sprite == null)
        {
            ResetData();
            return;
        }
        this._itemImage.gameObject.SetActive(true);
        this._itemImage.sprite = sprite;
        this._quantityTxt.text = quantity + "";
    }

    public void Select()
    {
        //_itemCell.color = _selectedCellColor;
        //_itemImage.color = new Color(_itemImage.color.r, _itemImage.color.g, _itemImage.color.b, 1f);
        _selectedOutline.gameObject.SetActive(true);
        IsSelected = true;
    }

    public void Deselect()
    {
        //_itemCell.color = Color.white;
        //_itemImage.color = new Color(_itemImage.color.r, _itemImage.color.g, _itemImage.color.b, 0.8f);
        _selectedOutline.gameObject.SetActive(false);
        IsSelected = false;
    }
}
