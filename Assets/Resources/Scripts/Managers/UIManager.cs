using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region Private fields
    [Header("Main menu")]
    [SerializeField] private UIPage _mainMenuUI;
    [SerializeField] private UIPage _mainMenuProgressBarUI;
    [SerializeField] private UIPage _mainMenuPlayersUI;
    [SerializeField] private UIPage _mainMenuWorldsUI;

    [Header("Debug")]
    [SerializeField] private UIPage _debugPhasesInfoUI;
    [SerializeField] private UIPage _debugBlockInfoUI;

    #endregion

    #region Public fields
    public static UIManager Instance;
    #endregion

    #region Properties
    public UIPage MainMenuUI
    {
        get
        {
            return _mainMenuUI;
        }
    }

    public UIPage MainMenuProgressBarUI
    {
        get
        {
            return _mainMenuProgressBarUI;
        }
    }

    public UIPage MainMenuPlayersUI
    {
        get
        {
            return _mainMenuPlayersUI;
        }
    }

    public UIPage MainMenuWorldsUI
    {
        get
        {
            return _mainMenuWorldsUI;
        }
    }
    #endregion

    #region Methods
    private void Awake()
    {
        Instance = this;

        _mainMenuUI.IsActive = false;
        _mainMenuProgressBarUI.IsActive = false;
        _mainMenuPlayersUI.IsActive = false;
        _mainMenuWorldsUI.IsActive = false;

        _debugPhasesInfoUI.IsActive = false;
        _debugBlockInfoUI.IsActive = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            _debugPhasesInfoUI.IsActive = !_debugPhasesInfoUI.IsActive;
            _debugBlockInfoUI.IsActive = !_debugBlockInfoUI.IsActive;
        }
    }
    #endregion
}