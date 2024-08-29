using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SavageWorld.Runtime.Entities.Player.CraftStation.UI
{
    public class UISearchRecipes : MonoBehaviour, IPointerClickHandler
    {
        #region Private fields
        [SerializeField] private TMP_InputField _searchInputField;
        #endregion

        #region Public fields
        public Action<string> OnNeedSearch;
        #endregion

        #region Properties

        #endregion

        #region Methods
        public void ResetData()
        {
            _searchInputField.text = "";
        }

        public void SearchRecipes()
        {
            OnNeedSearch?.Invoke(_searchInputField.text.ToLower());
        }

        public void OnPointerClick(PointerEventData pointerData)
        {
            if (pointerData.button == PointerEventData.InputButton.Left)
            {
                ResetData();
            }
        }
        #endregion
    }
}