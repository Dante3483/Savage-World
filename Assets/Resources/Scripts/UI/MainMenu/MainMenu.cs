using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
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
}
