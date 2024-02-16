using System.IO;
using UnityEngine;

public class ListOfExistingWorldsController : MonoBehaviour
{
    #region Private fields
    [SerializeField] private UIListOfExistingWorlds _listOfExistingWorldsUI;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public static ListOfExistingWorldsController Instance;
    #endregion

    #region Methods
    private void Awake()
    {
        Instance = this;
    }

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
        GameManager.Instance.WorldName = $"World {GameManager.Instance.WorldNames.Count + 1}";
        UIManager.Instance.MainMenuWorldsUI.IsActive = false;
        GameManager.Instance.UpdateGameState(GameState.NewGameState);
    }

    private void HandleWorldSelect(string worldName)
    {
        GameManager.Instance.WorldName = worldName;
        UIManager.Instance.MainMenuWorldsUI.IsActive = false;
        GameManager.Instance.UpdateGameState(GameState.LoadGameState);
    }

    private void HandleWorldDelete(string worldName)
    {
        string worldPath = StaticInfo.WorldsDirectory + worldName;
        Directory.Delete(worldPath);
        GameManager.Instance.WorldNames.Remove(worldName);
        UpdateUI();
    }
    #endregion
}
