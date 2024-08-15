using Items;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractions : MonoBehaviour
{
    #region Fields
    private GameManager _gameManager;
    private WorldDataManager _worldDataManager;
    [Header("Interaction area")]
    [SerializeField]
    private bool _isAreaDrawing;
    [SerializeField]
    private Vector2 _areaSize;
    [SerializeField]
    private bool _isMouseInsideArea;

    [Header("Mining")]
    [Min(0.001f)]
    [SerializeField]
    private float _wallDamageMultiplier;

    [Header("Placement")]
    [SerializeField]
    private float _placementSpeed;
    [SerializeField]
    private LayerMask _playerLayerMask;
    private BoxCastUtil _checkPlayerBoxCast;

    private InventoryModel _inventory;
    [SerializeField]
    private bool _isActive;
    private bool _isUsingItemFromHotbar;
    private bool _isBreakingWall;

    private PlaceBlockAction _placeBlockAction;
    private BreakBlockAction _breakBlockAction;
    private BreakWallAction _breakWallAction;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods
    private void Awake()
    {
        _gameManager = GameManager.Instance;
        _worldDataManager = WorldDataManager.Instance;
        _isActive = false;
    }

    private void FixedUpdate()
    {
        if (!_isActive)
        {
            return;
        }
        MouseInsideAreaCheck();
        if (_isUsingItemFromHotbar)
        {
            UseItemFromHotbar();
        }
        if (_isBreakingWall)
        {
            _breakWallAction.Configure(ClickPositionToWorldPosition(), _wallDamageMultiplier, 1);
            _breakWallAction.Execute();
        }
        if (_isMouseInsideArea)
        {
            CreateWater();
            CreateTorch();
        }
    }
    #endregion

    #region Public Methods
    public void Initialize(InventoryModel inventory)
    {
        _inventory = inventory;
        _inventory.InventoryOverflowed += InventoryOverflowedEventHandler;
        _checkPlayerBoxCast.OriginOffset = new Vector2(0.5f, 0.5f);
        _checkPlayerBoxCast.Size = new Vector2(1f, 1f);
        _checkPlayerBoxCast.LayerMask = _playerLayerMask;

        PlayerInputActions playerInputActions = GameManager.Instance.PlayerInputActions;
        playerInputActions.Interactions.Enable();
        playerInputActions.Interactions.UseItemFromHotbar.performed += UseItemFromHotbarPerformed;
        playerInputActions.Interactions.UseItemFromHotbar.canceled += UseItemFromHotbarCanceled;
        playerInputActions.Interactions.BreakBlock.performed += BreakWallPerformed;
        playerInputActions.Interactions.BreakBlock.canceled += BreakWallCanceled;
        playerInputActions.Interactions.ThrowItem.performed += ThrowSelectedItem;

        EventManager.BookOpened += Disable;
        EventManager.BookClosed += Enable;

        _placeBlockAction = new();
        _breakBlockAction = new();
        _breakWallAction = new();
        _isActive = true;
    }

    public bool IsEnoughSpaceToTakeDrop(Drop drop)
    {
        return !_inventory.IsEnoughSpaceForItem(drop.Item, ItemLocations.Hotbar) || !_inventory.IsEnoughSpaceForItem(drop.Item, ItemLocations.Storage);
    }
    #endregion

    #region Private Methods
    private void Disable()
    {
        _isActive = false;
    }

    private void Enable()
    {
        _isActive = true;
    }

    private void UseItemFromHotbar()
    {
        switch (_inventory.GetSelectedItemData())
        {
            case BlockItemSO blockItem:
                {
                    Vector2Int blockPosition = ClickPositionToWorldPosition();
                    _checkPlayerBoxCast.BoxCast(blockPosition);
                    if (_isMouseInsideArea && !_checkPlayerBoxCast.Result)
                    {
                        _placeBlockAction.Configure(blockItem.BlockToPlace, blockPosition, _placementSpeed);
                        _placeBlockAction.Execute();
                    }
                }
                break;
            case PickaxeItemSO pickaxeItem:
                {
                    _breakBlockAction.Configure(ClickPositionToWorldPosition(), pickaxeItem.MiningDamage, pickaxeItem.MiningSpeed);
                    _breakBlockAction.Execute();
                }
                break;
            default:
                break;
        }
    }

    //MOVE TO DROP MANAGER
    public void TakeDrop(Drop drop)
    {
        if (drop.Quantity == 0)
        {
            return;
        }
        int remainder = _inventory.AddItem(drop.Item, drop.Quantity, ItemLocations.Hotbar);
        drop.Quantity = remainder;

        if (remainder != 0)
        {
            remainder = _inventory.AddItem(drop.Item, drop.Quantity, ItemLocations.Storage);
            drop.Quantity = remainder;
        }
    }

    private void ThrowSelectedItem(InputAction.CallbackContext _)
    {
        ItemSO data = _inventory.GetSelectedItemData();
        int quantity = _inventory.GetSelectedItemQuantity();
        Drop drop = DropManager.Instance.CreateDrop(transform.position, data, quantity);
        if (drop != null)
        {
            DropPhysics dropPhysics = drop.GetComponent<DropPhysics>();
            dropPhysics.AddForce();
            _inventory.RemoveQuantityFromSelectedItem(quantity);
        }
    }

    private void MouseInsideAreaCheck()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float distanceX = Mathf.Abs(mousePosition.x - transform.position.x);
        float distanceY = Mathf.Abs(mousePosition.y - transform.position.y);
        _isMouseInsideArea = distanceX < _areaSize.x && distanceY < _areaSize.y;
        if (_isAreaDrawing)
        {
            DebugUtil.DrawBox(transform.position, _areaSize, _isMouseInsideArea ? Color.green : Color.red);
        }
    }

    private Vector2Int ClickPositionToWorldPosition()
    {
        Vector3 clickPosition = Input.mousePosition;
        return Vector2Int.FloorToInt(Camera.main.ScreenToWorldPoint(clickPosition));
    }

    private void InventoryOverflowedEventHandler(ItemSO data, int quantity)
    {
        DropManager.Instance.CreateDrop(transform.position, data, quantity);
    }

    private void UseItemFromHotbarPerformed(InputAction.CallbackContext _)
    {
        _isUsingItemFromHotbar = true;
    }

    private void UseItemFromHotbarCanceled(InputAction.CallbackContext _)
    {
        _isUsingItemFromHotbar = false;
    }

    //REMOVE
    private void UpdateNeighboringBlocks(Vector2Int blockPosition)
    {
        //_gameManager.Terrain.NeedToUpdate.Add(new Vector2Ushort(blockPosition.x, blockPosition.y));
        //_gameManager.Terrain.NeedToUpdate.Add(new Vector2Ushort(blockPosition.x, blockPosition.y + 1));
        //_gameManager.Terrain.NeedToUpdate.Add(new Vector2Ushort(blockPosition.x, blockPosition.y - 1));
        //_gameManager.Terrain.NeedToUpdate.Add(new Vector2Ushort(blockPosition.x - 1, blockPosition.y));
        //_gameManager.Terrain.NeedToUpdate.Add(new Vector2Ushort(blockPosition.x + 1, blockPosition.y));
    }

    //REMOVE
    private void CreateWater()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Vector2Int waterPosition = ClickPositionToWorldPosition();
            _worldDataManager.SetLiquidData(waterPosition.x, waterPosition.y, _gameManager.BlocksAtlas.Water);
        }
    }

    //REMOVE
    private void CreateTorch()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Vector2Int torchPosition = ClickPositionToWorldPosition();
            _worldDataManager.SetBlockData(torchPosition.x, torchPosition.y, _gameManager.BlocksAtlas.GetBlockById(FurnitureBlocksID.Torch));
        }
    }

    //REMOVE
    private void BreakWallPerformed(InputAction.CallbackContext _)
    {
        _isBreakingWall = true;
    }

    //REMOVE
    private void BreakWallCanceled(InputAction.CallbackContext _)
    {
        _isBreakingWall = false;
    }
    #endregion
}
