using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ListOfExistingPlayersController : MonoBehaviour
{
    #region Private fields
    [SerializeField] private UIListOfExistingPlayers _listOfExistingPlayersUI;
    #endregion

    #region Public fields

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
        string playerName = $"Player {GameManager.Instance.PlayerNames.Count + 1}";
        string playerPath = StaticInfo.PlayersDirectory + $"/{playerName}.sw.player";

        using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(playerPath, FileMode.Create)))
        {
            binaryWriter.Write(playerName);
        }

        GameManager.Instance.PlayerNames.Add(playerName);

        UpdateUI();
    }

    private void HandlePlayerSelect(string playerName)
    {
        GameManager.Instance.PlayerName = playerName;
        UIManager.Instance.MainMenuPlayersUI.IsActive = false;
        UIManager.Instance.MainMenuWorldsUI.IsActive = true;
        ListOfExistingWorldsController.Instance.UpdateUI();
    }

    private void HandlePlayerDelete(string playerName)
    {
        string playerPath = StaticInfo.PlayersDirectory + $"/{playerName}.sw.player";
        File.Delete(playerPath);
        GameManager.Instance.PlayerNames.Remove(playerName);
        UpdateUI();
    }
    #endregion
}
