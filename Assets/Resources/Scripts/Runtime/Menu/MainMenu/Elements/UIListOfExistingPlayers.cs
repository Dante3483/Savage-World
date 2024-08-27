using System;
using System.Collections.Generic;
using UnityEngine;

namespace SavageWorld.Runtime.Menu.Elements
{
    public class UIListOfExistingPlayers : MonoBehaviour
    {
        #region Private fields
        [Header("Main")]
        [SerializeField] private RectTransform _content;
        [SerializeField] private UIPlayerCell _cellPrefab;
        [SerializeField] private List<UIPlayerCell> _listOfCells;
        #endregion

        #region Public fields
        public Action OnCreatePlayer;
        public Action<string> OnPlayerSelect, OnPlayerDelete;
        #endregion

        #region Properties

        #endregion

        #region Methods
        public void InitializeUI(int playersCount)
        {
            _listOfCells.Clear();
            foreach (Transform child in _content.transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < playersCount; i++)
            {
                UIPlayerCell playerCell = Instantiate(_cellPrefab, Vector3.zero, Quaternion.identity, _content);
                _listOfCells.Add(playerCell);

                playerCell.OnPlayerSelect += HandlePlayerSelect;
                playerCell.OnPlayerDelete += HandlePlayerDelete;
            }
        }

        public void UpdateCell(int playerIndex, string playerName)
        {
            if (_listOfCells.Count > playerIndex)
            {
                _listOfCells[playerIndex].SetData(playerName);
            }
        }

        public void CreatePlayer()
        {
            OnCreatePlayer?.Invoke();
        }

        private void HandlePlayerSelect(string playerName)
        {
            OnPlayerSelect?.Invoke(playerName);
        }

        private void HandlePlayerDelete(string playerName)
        {
            OnPlayerDelete?.Invoke(playerName);
        }
        #endregion
    }
}