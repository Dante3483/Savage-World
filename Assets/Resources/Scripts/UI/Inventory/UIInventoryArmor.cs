using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory
{
    public class UIInventoryArmor : MonoBehaviour, IPointerClickHandler
    {
        #region Private fields
        [SerializeField] private ArmorTypes _armorType;
        [SerializeField] private int _priority;
        [SerializeField] private Image _content;
        [SerializeField] private Image _silhouette;
        #endregion

        #region Public fields
        public event Action<ArmorTypes> OnLeftMouseButtonClicked, OnRightMouseButtonClicked;
        #endregion

        #region Properties

        #endregion

        #region Methods
        private void Awake()
        {
            ResetData();
        }

        public void ResetData()
        {
            _content.gameObject.SetActive(false);
            _silhouette.gameObject.SetActive(true);
        }

        public void SetData(Sprite contentSprite)
        {
            _content.gameObject.SetActive(true);
            _silhouette.gameObject.SetActive(false);
            _content.sprite = contentSprite;
        }

        public void OnPointerClick(PointerEventData pointerData)
        {
            if (pointerData.button == PointerEventData.InputButton.Left)
            {
                OnLeftMouseButtonClicked?.Invoke(_armorType);
            }
            if (pointerData.button == PointerEventData.InputButton.Right)
            {
                OnRightMouseButtonClicked?.Invoke(_armorType);
            }
        }
        #endregion
    }
}