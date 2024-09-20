using System;
using System.Collections.Generic;
using UnityEngine;

namespace SavageWorld.Runtime.Menu.Elements
{
    public class UIListOfExistingWorlds : MonoBehaviour
    {
        #region Private fields
        [Header("Main")]
        [SerializeField] private RectTransform _content;
        [SerializeField] private UIWorldCell _cellPrefab;
        [SerializeField] private List<UIWorldCell> _listOfCells;
        #endregion

        #region Public fields
        public Action OnCreateWorld;
        public Action<string> OnWorldSelect, OnWorldDelete;
        #endregion

        #region Properties

        #endregion

        #region Methods
        public void InitializeUI(int worldsCount)
        {
            _listOfCells.Clear();
            foreach (Transform child in _content.transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < worldsCount; i++)
            {
                UIWorldCell worldCell = Instantiate(_cellPrefab, Vector3.zero, Quaternion.identity, _content);
                _listOfCells.Add(worldCell);

                worldCell.OnWorldSelect += HandleWorldSelect;
                worldCell.OnWorldDelete += HandleWorldDelete;
            }
        }

        public void UpdateCell(int worldIndex, string worldName)
        {
            if (_listOfCells.Count > worldIndex)
            {
                _listOfCells[worldIndex].SetData(worldName);
            }
        }

        public void CreateWorld()
        {
            OnCreateWorld?.Invoke();
        }

        private void HandleWorldSelect(string worldName)
        {
            OnWorldSelect?.Invoke(worldName);
        }

        private void HandleWorldDelete(string worldName)
        {
            OnWorldDelete?.Invoke(worldName);
        }
        #endregion
    }
}