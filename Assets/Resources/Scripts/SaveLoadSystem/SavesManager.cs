using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SavesManager : MonoBehaviour
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private UIPlayersPage _playersUI;
    [SerializeField] private UIWorldsPage _worldsUI;

    private string _directoryPathPlayers;
    private string _directoryPathWorlds;
    private List<string> _playerNames;
    private List<string> _worldNames;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        _directoryPathPlayers = Application.dataPath + "/Saves" + "/Players";
        _directoryPathWorlds = Application.dataPath + "/Saves" + "/Worlds";
        _playerNames = new List<string>();
        _worldNames = new List<string>();

        PreparePlayersData();
        PreparePlayersUI();
        PrepareWorldsData();
        PrepareWorldsUI();
    }

    private void PreparePlayersData()
    {
        GetAllPlayerNames();
    }

    private void PreparePlayersUI()
    {
        UpdatePlayersUI();

        _playersUI.OnCreatePlayer += HandleCreatePlayer;
        _playersUI.OnPlayerSelect += HandlePlayerSelect;
        _playersUI.OnPlayerDelete += HandlePlayerDelete;
    }

    private void PrepareWorldsData()
    {
        GetAllWorldNames();
    }

    private void PrepareWorldsUI()
    {
        UpdateWorldsUI();

        _worldsUI.OnCreateWorld += HandleCreateWorld;
        _worldsUI.OnWorldSelect += HandleWorldSelect;
        _worldsUI.OnWorldDelete += HandleWorldDelete;
    }

    private void GetAllPlayerNames()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(_directoryPathPlayers);
        FileInfo[] filesInfo = directoryInfo.GetFiles("*.sw.player");
        foreach (FileInfo fileInfo in filesInfo)
        {
            _playerNames.Add(fileInfo.Name.Replace(".sw.player", ""));
        }
    }

    private void GetAllWorldNames()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(_directoryPathWorlds);
        DirectoryInfo[] directoriesInfo = directoryInfo.GetDirectories();
        foreach (DirectoryInfo directoryIndo in directoriesInfo)
        {
            _worldNames.Add(directoryIndo.Name);
        }
    }

    private void UpdatePlayersUI()
    {
        _playersUI.InitializePlayersUI(_playerNames.Count);

        for (int i = 0; i < _playerNames.Count; i++)
        {
            _playersUI.UpdatePlayerCell(i, _playerNames[i]);
        }
    }

    private void UpdateWorldsUI()
    {
        _worldsUI.InitializeWorldsUI(_worldNames.Count);

        for (int i = 0; i < _worldNames.Count; i++)
        {
            _worldsUI.UpdateWorldCell(i, _worldNames[i]);
        }
    }

    private void HandleCreatePlayer()
    {
        string playerName = $"Player {_playerNames.Count + 1}";
        string playerPath = _directoryPathPlayers + $"/{playerName}.sw.player";

        using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(playerPath, FileMode.Create)))
        {
            binaryWriter.Write(playerName);
        }

        _playerNames.Add(playerName);

        UpdatePlayersUI();
    }

    private void HandlePlayerSelect(string playerName)
    {
        GameManager.Instance.PlayerName = playerName;
        _playersUI.gameObject.SetActive(false);
        _worldsUI.gameObject.SetActive(true);
    }

    private void HandlePlayerDelete(string playerName)
    {
        string playerPath = _directoryPathPlayers + $"/{playerName}.sw.player";
        File.Delete(playerPath);
        _playerNames.Remove(playerName);
        UpdatePlayersUI();
    }

    private void HandleCreateWorld()
    {
        GameManager.Instance.WorldName = $"World {_worldNames.Count + 1}";
        GameManager.Instance.UpdateGameState(GameState.NewGameState);
    }

    private void HandleWorldSelect(string worldName)
    {
        GameManager.Instance.WorldName = worldName;
        GameManager.Instance.UpdateGameState(GameState.LoadGameState);
    }

    private void HandleWorldDelete(string worldName)
    {
        string worldPath = _directoryPathWorlds + worldName;
        Directory.Delete(worldPath);
        _worldNames.Remove(worldName);
        UpdateWorldsUI();
    }
    #endregion
}
