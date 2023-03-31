using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameState _gameState;
    [Header("Initialization")]
    [SerializeField] private int _maxXWorldSize;
    [SerializeField] private int _maxYWorldSize;

    [Header("Level")]
    [SerializeField] private GameObject _terrainPrefab;
    [SerializeField] private GameObject _playerPrefab;

    [Header("UI")]
    [SerializeField] private Canvas _mainMenuCanvas;
    [SerializeField] private Canvas _inGameCanvas;

    public static GameManager Instance;
    public ObjectData[,] ObjectsData;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateGameState(GameState.FirstInitialization);
    }

    public void UpdateGameState(GameState newState)
    {
        _gameState = newState;

        switch (_gameState)
        {
            case GameState.FirstInitialization:
                {
                    HandleFirstInitializationState();
                }
                break;
            case GameState.MainMenu:
                {
                    HandleMainMenuState();
                }
                break;
            case GameState.NewLevel:
                {
                    HandleNewLevelState();
                }
                break;
            case GameState.LoadLevel:
                {
                    HandleLoadLevelState();
                }
                break;
            case GameState.Quit:
                {
                    HandleQuitState();
                }
                break;
            default:
                break;
        }
    }

    private void HandleQuitState()
    {

    }

    private void HandleLoadLevelState()
    {
        _mainMenuCanvas.gameObject.SetActive(false);
        _inGameCanvas.gameObject.SetActive(true);
    }

    private void HandleNewLevelState()
    {
        GameObject terrain = Instantiate(_terrainPrefab);
        GameObject player = Instantiate(_playerPrefab);

        _mainMenuCanvas.gameObject.SetActive(false);
        _inGameCanvas.gameObject.SetActive(true);
    }

    private void HandleMainMenuState()
    {
        _mainMenuCanvas.gameObject.SetActive(true);
        _inGameCanvas.gameObject.SetActive(false);
    }

    private void HandleFirstInitializationState()
    {
        ObjectsData = new ObjectData[_maxXWorldSize, _maxYWorldSize];
        for (int x = 0; x < _maxXWorldSize; x++)
        {
            for (int y = 0; y < _maxYWorldSize; y++)
            {
                ObjectsData[x, y] = new ObjectData(new Vector3Int(x, y));
            }
        }
        UpdateGameState(GameState.MainMenu);
    }
}
