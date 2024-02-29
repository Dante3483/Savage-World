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

    [Header("InGame")]
    [SerializeField] private UIPage _inventoryUI;
    [SerializeField] private UIPage _hotbarUI;

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

    public UIPage InventoryUI
    {
        get
        {
            return _inventoryUI;
        }

        set
        {
            _inventoryUI = value;
        }
    }

    public UIPage HotbarUI
    {
        get
        {
            return _hotbarUI;
        }

        set
        {
            _hotbarUI = value;
        }
    }
    #endregion

    #region Methods
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            _debugPhasesInfoUI.ReverseActivity();
            _debugBlockInfoUI.ReverseActivity();
        }
    }
    #endregion
}