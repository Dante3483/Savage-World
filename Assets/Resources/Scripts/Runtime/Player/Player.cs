using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    #region Fields
    [SerializeField]
    private string _name;
    [SerializeField]
    private PlayerStats _stats;
    [SerializeField]
    private InventoryModel _inventory;
    [SerializeField]
    private CraftStationModelSO _handCraftStation;
    [SerializeField]
    private PlayerNetwork _playerNetwork;
    [SerializeField]
    private PlayerInteractions _playerInteractions;
    [SerializeField]
    private bool _isOwner;
    #endregion

    #region Properties
    public InventoryModel Inventory
    {
        get
        {
            return _inventory;
        }
    }

    public PlayerStats Stats
    {
        get
        {
            return _stats;
        }
    }

    public PlayerNetwork PlayerNetwork
    {
        get
        {
            return _playerNetwork;
        }
    }
    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods
    private void Awake()
    {
        _playerNetwork = GetComponent<PlayerNetwork>();
        _playerInteractions = GetComponent<PlayerInteractions>();
    }
    #endregion

    #region Public Methods
    public void Initialize()
    {
        PlayerInputActions playerInputActions = GameManager.Instance.PlayerInputActions;
        playerInputActions.UI.Enable();
        playerInputActions.UI.OpenCloseInventory.performed += ToggleInventoryEventHandler;
        playerInputActions.UI.OpenCloseCraftStation.performed += ToggleCraftStationEventHandler;
        playerInputActions.UI.OpenCloseResearch.performed += ToggleResearchEventHandler;
        _stats.Reset();
        _inventory = new();
        BookManager.Instance.InitializeInventory(_inventory);
        BookManager.Instance.InitializeHotbar(_inventory);
        BookManager.Instance.InitializeCraftStation(_handCraftStation, _inventory);
        BookManager.Instance.InitializeResearches(GameManager.Instance.GetResearches(), _inventory);
        _playerInteractions.Initialize(_inventory);
    }

    public void DisableMovement()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<PlayerMovementNew>().enabled = false;
    }

    public void EnableMovement()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GetComponent<PlayerMovementNew>().enabled = true;
    }
    #endregion

    #region Private Methods
    private void ToggleInventoryEventHandler(InputAction.CallbackContext context)
    {
        _playerInteractions.SetActive(BookManager.Instance.TogglePresenter(BookManager.Instance.InventoryPresenter));
    }

    private void ToggleCraftStationEventHandler(InputAction.CallbackContext context)
    {
        _handCraftStation.SelectCraftStation();
        _playerInteractions.SetActive(BookManager.Instance.TogglePresenter(BookManager.Instance.CraftStationPresenter));
    }

    private void ToggleResearchEventHandler(InputAction.CallbackContext context)
    {
        _playerInteractions.SetActive(BookManager.Instance.TogglePresenter(BookManager.Instance.ResearchesPresenter));
    }
    #endregion
}
