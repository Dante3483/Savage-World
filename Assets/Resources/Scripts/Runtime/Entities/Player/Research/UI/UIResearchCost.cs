using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SavageWorld.Runtime.Entities.Player.Research.UI
{
    public class UIResearchCost : MonoBehaviour, IPointerEnterHandler,
            IPointerExitHandler
    {
        #region Private fields
        private bool _isMouseAbove;
        [SerializeField]
        private Image _image;
        [SerializeField]
        private TMP_Text _quantity;
        #endregion

        #region Public fields
        public event Action<UIResearchCost> OnMouseEnter;
        public event Action OnMouseLeave;
        #endregion

        #region Properties
        public Image Image { get => _image; set => _image = value; }
        public TMP_Text Quantity { get => _quantity; set => _quantity = value; }
        #endregion

        #region Methods
        public void OnPointerEnter(PointerEventData pointerData)
        {
            _isMouseAbove = true;
            OnMouseEnter?.Invoke(this);
        }

        public void OnPointerExit(PointerEventData pointerData)
        {
            _isMouseAbove = false;
            OnMouseLeave?.Invoke();
        }
        #endregion
    }
}