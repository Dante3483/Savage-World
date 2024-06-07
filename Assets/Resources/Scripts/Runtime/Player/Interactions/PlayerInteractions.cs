using Items;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractions : MonoBehaviour
{
    #region Fields
    [Header("Main")]
    [SerializeField]
    private Drop _dropPrefab;

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
    private float _blockDamageMultiplier;
    [Min(0.001f)]
    [SerializeField]
    private float _wallDamageMultiplier;
    [SerializeField]
    private MiningDamageController _miningDamageController;

    [Header("Placement")]
    [SerializeField]
    private float _placementCooldown;
    [SerializeField]
    private LayerMask _playerLayerMask;
    private BoxCastUtil _checkPlayerBoxCast;

    private InventoryModel _inventory;
    [SerializeField]
    private bool _isActive;
    private bool _isUsingItemFromHotbar;
    private bool _isBreakingBlock;
    private bool _isBreakingWall;

    private PlaceBlockAction _placeBlockAction;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods
    private void Awake()
    {
        EventManager.PlayerSpawnedAsNotOwner += Disable;
        EventManager.BookOpened += Disable;
        EventManager.BookClosed += Enable;
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
        if (_isBreakingBlock)
        {
            BreakBlock();
        }
        if (_isBreakingWall)
        {
            BreakWall();
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
        _miningDamageController = MiningDamageController.Instance;
        _isActive = true;

        PlayerInputActions playerInputActions = GameManager.Instance.PlayerInputActions;
        playerInputActions.Interactions.Enable();
        playerInputActions.Interactions.UseItemFromHotbar.performed += UseItemFromHotbarPerformed;
        playerInputActions.Interactions.UseItemFromHotbar.canceled += UseItemFromHotbarCanceled;
        playerInputActions.Interactions.BreakBlock.performed += BreakBlockPerformed;
        playerInputActions.Interactions.BreakBlock.canceled += BreakBlockCanceled;
        playerInputActions.Interactions.ThrowItem.performed += ThrowSelectedItem;

        _placeBlockAction = new(_placementCooldown);
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
                        _placeBlockAction.Configure(blockItem.BlockToPlace, blockPosition);
                        _placeBlockAction.Execute();
                    }
                }
                break;
            default:
                break;
        }
    }

    private void BreakBlock()
    {
        Vector2Int blockPosition = ClickPositionToWorldPosition();
        if (!WorldDataManager.Instance.IsBreakable(blockPosition.x, blockPosition.y))
        {
            return;
        }
        if (!WorldDataManager.Instance.IsEmpty(blockPosition.x, blockPosition.y))
        {
            BlockSO data = WorldDataManager.Instance.GetCellBlockData(blockPosition.x, blockPosition.y);
            _miningDamageController.AddDamageToBlock(blockPosition, _blockDamageMultiplier);
            if (_miningDamageController.IsBlockDamageReachedMaximum(blockPosition, data.DamageToBreak))
            {
                _miningDamageController.RemoveDamageFromBlocks(blockPosition);
                CreateDrop(new Vector3(blockPosition.x + 0.5f, blockPosition.y + 0.5f), data.Drop, 1);
                WorldDataManager.Instance.SetBlockData(blockPosition.x, blockPosition.y, GameManager.Instance.BlocksAtlas.Air);
                UpdateNeighboringBlocks(blockPosition);
            }
        }
    }

    public void BreakWall()
    {
        Vector2Int wallPosition = ClickPositionToWorldPosition();
        if (!WorldDataManager.Instance.IsBreakable(wallPosition.x, wallPosition.y))
        {
            return;
        }
        if (WorldDataManager.Instance.IsWall(wallPosition.x, wallPosition.y))
        {
            BlockSO data = WorldDataManager.Instance.GetCellWallData(wallPosition.x, wallPosition.y);
            _miningDamageController.AddDamageToWall(wallPosition, _wallDamageMultiplier);
            if (_miningDamageController.IsWallDamageReachedMaximum(wallPosition, data.DamageToBreak))
            {
                _miningDamageController.RemoveDamageFromWalls(wallPosition);
                CreateDrop(new Vector3(wallPosition.x + 0.5f, wallPosition.y + 0.5f), data.Drop, 1);
                GameManager.Instance.Terrain.CreateWall(wallPosition.x, wallPosition.y, GameManager.Instance.BlocksAtlas.AirWall);
            }
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

    //MOVE TO DROP MANAGER
    private Drop CreateDrop(Vector3 position, ItemSO item, int quantity)
    {
        if (item == null)
        {
            return null;
        }
        Drop drop = Instantiate(_dropPrefab, position, Quaternion.identity);
        drop.Quantity = quantity;
        drop.Item = item;
        return drop;
    }

    private void ThrowSelectedItem(InputAction.CallbackContext _)
    {
        ItemSO data = _inventory.GetSelectedItemData();
        int quantity = _inventory.GetSelectedItemQuantity();
        Drop drop = CreateDrop(transform.position, data, quantity);
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
        CreateDrop(transform.position, data, quantity);
    }

    private void UseItemFromHotbarPerformed(InputAction.CallbackContext context)
    {
        _isUsingItemFromHotbar = true;
    }

    private void UseItemFromHotbarCanceled(InputAction.CallbackContext context)
    {
        _isUsingItemFromHotbar = false;
    }

    private void BreakBlockPerformed(InputAction.CallbackContext context)
    {
        _isBreakingBlock = true;
    }

    private void BreakBlockCanceled(InputAction.CallbackContext context)
    {
        _isBreakingBlock = false;
    }

    //REMOVE
    private void UpdateNeighboringBlocks(Vector2Int blockPosition)
    {
        GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(blockPosition.x, blockPosition.y));
        GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(blockPosition.x, blockPosition.y + 1));
        GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(blockPosition.x, blockPosition.y - 1));
        GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(blockPosition.x - 1, blockPosition.y));
        GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(blockPosition.x + 1, blockPosition.y));
    }

    //REMOVE
    private void CreateWater()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Vector2Int waterPosition = ClickPositionToWorldPosition();
            GameManager.Instance.Terrain.CreateLiquidBlock((ushort)waterPosition.x, (ushort)waterPosition.y, (byte)GameManager.Instance.BlocksAtlas.Water.GetId());
            GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(waterPosition.x, waterPosition.y));
        }
    }

    //REMOVE
    private void CreateTorch()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Vector2Int torchPosition = ClickPositionToWorldPosition();
            WorldDataManager.Instance.SetBlockData((ushort)torchPosition.x, (ushort)torchPosition.y, GameManager.Instance.BlocksAtlas.GetBlockById(FurnitureBlocksID.Torch));
            UpdateNeighboringBlocks(torchPosition);
        }
    }
    #endregion
}
