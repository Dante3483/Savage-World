using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] private UIPage _craftStationUI;
    [SerializeField] private UIPage _hotbarUI;
    [SerializeField] private UIPage _researchUI;
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

    public UIPage CraftStationUI
    {
        get
        {
            return _craftStationUI;
        }
    }

    public UIPage ResearchUI { get => _researchUI; set => _researchUI = value; }
    #endregion

    #region Methods
    private void Awake()
    {
        Instance = this;
        PlayerInputActions inputActions = new();
        inputActions.UI.Enable();
        inputActions.UI.OpenCloseDebug.performed += ResetDebugUI;
    }

    private void ResetDebugUI(InputAction.CallbackContext context)
    {
        _debugPhasesInfoUI.ReverseActivity();
        _debugBlockInfoUI.ReverseActivity();
    }
    #endregion
}