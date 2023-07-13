using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private GameState _gameState;
    #endregion

    #region Public fields
    public static GameManager Instance;
    #endregion

    #region Properties
    public GameState GameState
    {
        get
        {
            return _gameState;
        }

        set
        {
            _gameState = value;
        }
    }
    #endregion

    #region Methods
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateGameState(global::GameState.GameInitializationState);
    }

    public void UpdateGameState(GameState newState)
    {
        GameState = newState;

        switch (GameState)
        {
            case GameState.GameInitializationState:
                {
                    HandleGameInitialization();
                }
                break;
            case GameState.MainMenuState:
                {
                    HandleMainMenu();
                }
                break;
            case GameState.NewGameState:
                {
                    HandleNewGameState();
                }
                break;
            default:
                break;
        }
    }
    private void HandleGameInitialization()
    {
        Debug.Log("Initialization state");
    }

    private void HandleMainMenu()
    {
        Debug.Log("Menu state");
    }

    private void HandleNewGameState()
    {
        Debug.Log("New game state");
    }
    #endregion
}
