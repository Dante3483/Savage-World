using SavageWorld.Runtime.Enums.Others;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SavageWorld.Runtime.Player.Inventory.UI
{
    public abstract class UIItemCell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler,
            IPointerExitHandler
    {
        #region Private fields
        [Header("Main")]
        [SerializeField]
        private Image _itemImage;
        private bool _isMouseAbove;
        #endregion

        #region Public fields
        public event Action<UIItemCell> LeftButtonClicked,
            MouseEntered, RightMouseDowned;
        public event Action MouseLeft, RightMouseUpped;
        #endregion

        #region Properties
        public virtual ItemLocations ItemLocation => ItemLocations.None;
        #endregion

        #region Methods
        private void Awake()
        {
            ResetData();
        }

        private void Update()
        {
            if (Input.GetMouseButton(1))
            {
                if (_isMouseAbove)
                {
                    RightMouseDowned?.Invoke(this);
                }
            }
            else
            {
                RightMouseUpped?.Invoke();
            }
        }

        private void OnDisable()
        {
            _isMouseAbove = false;
        }

        public virtual void ResetData()
        {
            _itemImage.gameObject.SetActive(false);
        }

        public virtual void SetSprite(Sprite sprite)
        {
            if (sprite == null)
            {
                ResetData();
                return;
            }
            _itemImage.gameObject.SetActive(true);
            _itemImage.sprite = sprite;
        }

        public virtual void SetQuantity(int quantity)
        {

        }

        public void OnPointerClick(PointerEventData pointerData)
        {
            if (pointerData.button == PointerEventData.InputButton.Left)
            {
                LeftButtonClicked?.Invoke(this);
            }
        }

        public void OnPointerEnter(PointerEventData pointerData)
        {
            _isMouseAbove = true;
            MouseEntered?.Invoke(this);
        }

        public void OnPointerExit(PointerEventData pointerData)
        {
            _isMouseAbove = false;
            MouseLeft?.Invoke();
        }
        #endregion
    }
}