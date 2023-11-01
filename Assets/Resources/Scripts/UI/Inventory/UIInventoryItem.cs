using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory
{
    public class UIInventoryItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler,
        IPointerExitHandler
    {
        #region Private fields
        [SerializeField] private ItemTypes _itemType;
        [SerializeField] private Image _itemImage;
        [SerializeField] private TMP_Text _quantityTxt;
        [SerializeField] private TMP_Text _keyTxt;
        [SerializeField] private bool _isMouseOver;
        [SerializeField] private bool _isRightMouseDown;
        #endregion

        #region Public fields
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
            if (_isMouseOver && _itemType != ItemTypes.Armor)
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
            _itemImage.gameObject.SetActive(false);
        }

        public void SetData(Sprite sprite, int quantity, ItemTypes type)
        {
            if (sprite == null)
            {
                ResetData();
                return;
            }
            _itemImage.gameObject.SetActive(true);
            _itemImage.sprite = sprite;
            _quantityTxt.text = quantity + "";
            _itemType = type;
        }

        public void OnPointerClick(PointerEventData pointerData)
        {
            if (pointerData.button == PointerEventData.InputButton.Left)
            {
                OnItemChangeCell?.Invoke(this);
            }
            if (pointerData.button == PointerEventData.InputButton.Right &&
                _itemType == ItemTypes.Armor)
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
}