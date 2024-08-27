using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SavageWorld.Runtime.Player.Research.UI
{
    public class UIResearchReward : MonoBehaviour, IPointerEnterHandler,
            IPointerExitHandler
    {
        #region Private fields
        private bool _isMouseAbove;
        [SerializeField]
        private Image _image;
        #endregion

        #region Public fields
        public event Action<UIResearchReward> OnMouseEnter;
        public event Action OnMouseLeave;
        #endregion

        #region Properties
        public Image Image { get => _image; set => _image = value; }
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