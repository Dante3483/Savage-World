using SavageWorld.Runtime.Network.Objects;
using System.Collections.Generic;
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
    private PlayerInteractions _playerInteractions;
    [SerializeField]
    private NetworkObject _networkObject;
    [Header("Debug")]
    [SerializeField]
    private List<ItemQuantity> _starterItems;

    private WorldDataManager _worldDataManager;
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

    public NetworkObject NetworkObject
    {
        get
        {
            return _networkObject;
        }
    }
    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods
    private void Awake()
    {
        _worldDataManager = WorldDataManager.Instance;
        _playerInteractions = GetComponent<PlayerInteractions>();
        _networkObject = GetComponent<NetworkObject>();
    }
    #endregion

    #region Public Methods
    public void Initialize(bool isMultiplayer, bool isOwner)
    {
        if (isMultiplayer)
        {
            _networkObject.IsOwner = isOwner;
            if (isOwner)
            {
                InitializeAsOwner();
            }
            else
            {
                InitializeAsNotOwner();
            }
        }
        else
        {
            InitializeAsOwner();
        }
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
    private void InitializeAsOwner()
    {
        _stats.Reset();
        _inventory = new();
        PlayerInputActions playerInputActions = GameManager.Instance.PlayerInputActions;
        playerInputActions.UI.Enable();
        playerInputActions.UI.OpenCloseInventory.performed += ToggleInventoryEventHandler;
        playerInputActions.UI.OpenCloseCraftStation.performed += ToggleCraftStationEventHandler;
        playerInputActions.UI.OpenCloseResearch.performed += ToggleResearchEventHandler;
        BookManager.Instance.InitializeInventory(_inventory);
        BookManager.Instance.InitializeHotbar(_inventory);
        BookManager.Instance.InitializeCraftStation(_handCraftStation, _inventory);
        BookManager.Instance.InitializeResearches(GameManager.Instance.GetResearches(), _inventory);
        _playerInteractions.Initialize(_inventory);
        foreach (ItemQuantity item in _starterItems)
        {
            _inventory.AddItem(item.Item, item.Quantity);
        }
        DisableMovement();
    }

    private void InitializeAsNotOwner()
    {
        DisableMovement();
    }

    private void ToggleInventoryEventHandler(InputAction.CallbackContext context)
    {
        BookManager.Instance.TogglePresenter(BookManager.Instance.InventoryPresenter);
    }

    private void ToggleCraftStationEventHandler(InputAction.CallbackContext context)
    {
        _handCraftStation.SelectCraftStation();
        BookManager.Instance.TogglePresenter(BookManager.Instance.CraftStationPresenter);
    }

    private void ToggleResearchEventHandler(InputAction.CallbackContext context)
    {
        BookManager.Instance.TogglePresenter(BookManager.Instance.ResearchesPresenter);
    }
    #endregion
}
