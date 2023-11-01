using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inventory
{
    public class UIInventoryDrop : MonoBehaviour, IPointerClickHandler
    {
        #region Private fields

        #endregion

        #region Public fields
        public event Action OnRightMouseClick;
        #endregion

        #region Properties

        #endregion

        #region Methods
        public void OnPointerClick(PointerEventData pointerData)
        {
            if (pointerData.button == PointerEventData.InputButton.Right)
            {
                OnRightMouseClick?.Invoke();
            }
        }
        #endregion
    }
}