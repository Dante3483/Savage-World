using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    #region Private fields
    [SerializeField]
    private string _name;
    [SerializeField]
    private PlayerStats _stats;
    [SerializeField]
    private InventoryModel _inventory;
    [SerializeField]
    private PlayerNetwork _playerNetwork;
    [SerializeField]
    private bool _isOwner;
    #endregion

    #region Public fields

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

        set
        {
            _stats = value;
        }
    }

    public PlayerNetwork PlayerNetwork
    {
        get
        {
            return _playerNetwork;
        }
    }

    public bool IsOwner
    {
        get
        {
            return _isOwner;
        }

        set
        {
            _isOwner = value;
        }
    }
    #endregion

    #region Methods
    private void Awake()
    {
        _playerNetwork = GetComponent<PlayerNetwork>();
    }

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

    private void ToggleInventoryEventHandler(InputAction.CallbackContext context)
    {
        BookManager.Instance.TogglePresenter(BookManager.Instance.InventoryPresenter);
    }

    private void ToggleCraftStationEventHandler(InputAction.CallbackContext context)
    {

    }

    private void ToggleResearchEventHandler(InputAction.CallbackContext context)
    {

    }
    #endregion
}
