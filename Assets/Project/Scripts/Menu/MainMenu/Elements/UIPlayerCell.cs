using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SavageWorld.Runtime.Menu.Elements
{
    public class UIPlayerCell : MonoBehaviour, IPointerClickHandler
    {
        #region Private fields
        [Header("Main")]
        [SerializeField]
        private Button _button;
        [SerializeField]
        private TMP_Text _playerName;
        #endregion

        #region Public fields
        public Action<string> OnPlayerSelect, OnPlayerDelete;
        #endregion

        #region Properties

        #endregion

        #region Methods
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(() =>
            {
                OnPlayerSelect?.Invoke(_playerName.text);
            });
        }

        public void SetData(string playerName)
        {
            _playerName.text = playerName;
        }

        public void OnPointerClick(PointerEventData pointerData)
        {
            if (pointerData.button == PointerEventData.InputButton.Right)
            {
                OnPlayerDelete?.Invoke(_playerName.text);
            }
        }
        #endregion
    }
}