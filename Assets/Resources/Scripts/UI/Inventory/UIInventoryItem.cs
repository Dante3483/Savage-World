using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIInventoryItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, 
    IPointerExitHandler
{
    #region Private Fields
    [SerializeField] private ItemType _itemType;
    [SerializeField] private Image _itemImage;
    [SerializeField] private TMP_Text _quantityTxt;
    [SerializeField] private TMP_Text _keyTxt;
    [SerializeField] private bool _isMouseOver;
    [SerializeField] private bool _isRightMouseDown;
    #endregion

    #region Public Fields
    public event Action<UIInventoryItem> OnItemChangeCell,
        OnMouseHover, OnMouseLeave, OnRightMouseDown, OnRightMouseUp;
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
    #endregion

    #region Methods
    public void Awake()
    {
        ResetData();
    }

    private void Update()
    {
        if (_isMouseOver && _itemType != ItemType.Armor)
        {
            if (Input.GetMouseButton(1))
            {
                _isRightMouseDown = true;
                OnRightMouseClamped();
            }
            else
            {
                _isRightMouseDown = false;
                OnRightMouseStopClamped();
            }
        }
        else if (_isRightMouseDown)
        {
            _isRightMouseDown = false;
            OnRightMouseStopClamped();
        }
    }

    public void ResetData()
    {
        this._itemImage.gameObject.SetActive(false);
    }

    public void SetData(Sprite sprite, int quantity, ItemType type)
    {
        if (sprite == null)
        {
            ResetData();
            return;
        }
        this._itemImage.gameObject.SetActive(true);
        this._itemImage.sprite = sprite;
        this._quantityTxt.text = quantity + "";
        this._itemType = type;
    }

    public void OnPointerClick(PointerEventData pointerData)
    {
        if (pointerData.button == PointerEventData.InputButton.Left)
        {
            OnItemChangeCell?.Invoke(this);
        }
        if (pointerData.button == PointerEventData.InputButton.Right &&
            _itemType == ItemType.Armor)
        {
            OnRightMouseDown?.Invoke(this);
        }
    }

    public void OnPointerEnter(PointerEventData pointerData)
    {
        _isMouseOver = true;
        OnMouseHover?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData pointerData)
    {
        _isMouseOver = false;
        OnMouseLeave?.Invoke(this);
    }

    public void OnRightMouseClamped()
    {
        OnRightMouseDown?.Invoke(this);
    }

    public void OnRightMouseStopClamped()
    {
        OnRightMouseUp?.Invoke(this);
    }
    #endregion
}
