using SavageWorld.Runtime.GameSession;
using SavageWorld.Runtime.Menu.Elements;
using SavageWorld.Runtime.Utilities.Others;
using System;
using UnityEngine;

namespace SavageWorld.Runtime.Menu
{
    public class ListOfExistingPlayersController : Singleton<ListOfExistingPlayersController>
    {
        #region Private fields
        [SerializeField] private UIListOfExistingPlayers _listOfExistingPlayersUI;
        #endregion

        #region Public fields
        public event Action OnPlayerCreated;
        public event Action<string> OnPlayerSelected, OnPlayerDeleted;
        #endregion

        #region Properties

        #endregion

        #region Methods
        private void Start()
        {
            PrepareUI();
        }

        private void PrepareUI()
        {
            _listOfExistingPlayersUI.OnCreatePlayer += HandleCreatePlayer;
            _listOfExistingPlayersUI.OnPlayerSelect += HandlePlayerSelect;
            _listOfExistingPlayersUI.OnPlayerDelete += HandlePlayerDelete;
        }

        public void UpdateUI()
        {
            _listOfExistingPlayersUI.InitializeUI(GameManager.Instance.PlayerNames.Count);

            for (int i = 0; i < GameManager.Instance.PlayerNames.Count; i++)
            {
                _listOfExistingPlayersUI.UpdateCell(i, GameManager.Instance.PlayerNames[i]);
            }
        }

        private void HandleCreatePlayer()
        {
            OnPlayerCreated?.Invoke();
            UpdateUI();
        }

        private void HandlePlayerSelect(string playerName)
        {
            OnPlayerSelected?.Invoke(playerName);
        }

        private void HandlePlayerDelete(string playerName)
        {
            OnPlayerDeleted?.Invoke(playerName);
            UpdateUI();
        }
        #endregion
    }
}