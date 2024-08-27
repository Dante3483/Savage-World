using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SavageWorld.Runtime.Player.CraftStation.UI
{
    public class UIRecipe : MonoBehaviour, IPointerClickHandler
    {
        #region Private fields
        [Header("Main")]
        [SerializeField] private Image _itemImage;
        [SerializeField] private Image _frameImage;
        [SerializeField] private TMP_Text _nameTxt;
        #endregion

        #region Public fields
        public Action<UIRecipe> OnLeftButtonClick;
        #endregion

        #region Properties

        #endregion

        #region Methods
        public void SetData(Sprite sprite, string name)
        {
            _itemImage.sprite = sprite;
            _nameTxt.text = name;
        }

        public void Select()
        {
            _frameImage.gameObject.SetActive(true);
        }

        public void Deselect()
        {
            _frameImage.gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData pointerData)
        {
            if (pointerData.button == PointerEventData.InputButton.Left)
            {
                OnLeftButtonClick?.Invoke(this);
            }
        }
        #endregion
    }
}