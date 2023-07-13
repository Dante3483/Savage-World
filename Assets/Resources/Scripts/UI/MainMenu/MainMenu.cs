using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    #region Private fields

    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public void NewGame()
    {
        GameManager.Instance.UpdateGameState(GameState.NewGameState);
    }

    public void LoadGame()
    {
        //GameManager.Instance.UpdateGameState(GameState.LoadLevel);
    }

    public void Quit()
    {
        //GameManager.Instance.UpdateGameState(GameState.Quit);
    }
    #endregion
}
