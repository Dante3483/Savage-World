using SavageWorld.Runtime.GameSession;
using SavageWorld.Runtime.Menu.Elements;
using SavageWorld.Runtime.Utilities.Others;
using System;
using UnityEngine;

namespace SavageWorld.Runtime.Menu
{
    public class ListOfExistingWorldsController : Singleton<ListOfExistingWorldsController>
    {
        #region Private fields
        [SerializeField] private UIListOfExistingWorlds _listOfExistingWorldsUI;
        #endregion

        #region Public fields
        public event Action OnWorldCreated;
        public event Action<string> OnWorldSelected, OnWorldDeleted;
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
            _listOfExistingWorldsUI.OnCreateWorld += HandleCreateWorld;
            _listOfExistingWorldsUI.OnWorldSelect += HandleWorldSelect;
            _listOfExistingWorldsUI.OnWorldDelete += HandleWorldDelete;
        }

        public void UpdateUI()
        {
            _listOfExistingWorldsUI.InitializeUI(GameManager.Instance.WorldNames.Count);

            for (int i = 0; i < GameManager.Instance.WorldNames.Count; i++)
            {
                _listOfExistingWorldsUI.UpdateCell(i, GameManager.Instance.WorldNames[i]);
            }
        }

        private void HandleCreateWorld()
        {
            OnWorldCreated?.Invoke();
        }

        private void HandleWorldSelect(string worldName)
        {
            OnWorldSelected?.Invoke(worldName);
        }

        private void HandleWorldDelete(string worldName)
        {
            OnWorldDeleted?.Invoke(worldName);
            UpdateUI();
        }
        #endregion
    }
}