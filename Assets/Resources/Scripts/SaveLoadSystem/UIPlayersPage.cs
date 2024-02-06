using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayersPage : MonoBehaviour
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private UIPlayerCell _playerCellPrefab;
    [SerializeField] private RectTransform _playersContent;
    [SerializeField] private List<UIPlayerCell> _listOfPlayerCells;
    #endregion

    #region Public fields
    public Action OnCreatePlayer;
    public Action<string> OnPlayerSelect, OnPlayerDelete;
    #endregion

    #region Properties

    #endregion

    #region Methods
    public void InitializePlayersUI(int playersCount)
    {
        _listOfPlayerCells.Clear();
        foreach (Transform child in _playersContent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < playersCount; i++)
        {
            UIPlayerCell playerCell = Instantiate(_playerCellPrefab, Vector3.zero, Quaternion.identity, _playersContent);
            _listOfPlayerCells.Add(playerCell);

            playerCell.OnPlayerSelect += HandlePlayerSelect;
            playerCell.OnPlayerDelete += HandlePlayerDelete;
        }
    }

    public void UpdatePlayerCell(int playerIndex, string playerName)
    {
        if (_listOfPlayerCells.Count > playerIndex)
        {
            _listOfPlayerCells[playerIndex].SetData(playerName);
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
