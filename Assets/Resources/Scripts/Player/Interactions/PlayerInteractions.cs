using Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    delegate void SetDamageDelegate(ref WorldCellData block, float damage);

    #region Private fields
    [Header("Main")]
    [SerializeField] private Drop _dropPrefab;
    [SerializeField] private ItemSO _item;
    [SerializeField] private InventorySO _inventoryData;

    [Header("Interaction area")]
    [SerializeField] private bool _isAreaDrawing;
    [SerializeField] private Vector2 _areaSize;
    [SerializeField] private bool _isMouseInsideArea;

    [Header("Mining")]
    [Min(0.001f)][SerializeField] private float _blockDamageMultiplier;
    [Min(0.001f)][SerializeField] private float _wallDamageMultiplier;
    [SerializeField] private MiningDamageController _miningDamageController;

    [Header("Placing")]
    [SerializeField] private float _placingCooldown;
    [SerializeField] private bool _isPlacingAllowed;
    [SerializeField] private LayerMask _playerLayerMask;
    private BoxCastUtil _checkPlayerBoxCast;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        _inventoryData.OnInventoryFull += HandleThrowItem;
        _checkPlayerBoxCast.OriginOffset = new Vector2(0.5f, 0.5f);
        _checkPlayerBoxCast.Size = new Vector2(1f, 1f);
        _checkPlayerBoxCast.LayerMask = _playerLayerMask;
        _isPlacingAllowed = true;
    }

    private void FixedUpdate()
    {
        IsMouseInsideArea();
        if (GameManager.Instance.IsGameSession)
        {
            if (!GameManager.Instance.IsInputTextInFocus && Input.GetMouseButton((int)MouseButton.Left))
            {
                InteractWithItemInHotbar();
            }
            if (!GameManager.Instance.IsInputTextInFocus && Input.GetKeyDown(KeyCode.T))
            {
                ThrowSelectedItem();
            }
            if (_isMouseInsideArea && !GameManager.Instance.IsInputTextInFocus)
            {
                BreakBlock();
                BreakWall();
                CreateWater();
                CreateTorch();
            }
        }
    }

    private void IsMouseInsideArea()
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

    private void UpdateNeighboringBlocks(Vector2Int blockPosition)
    {
        GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(blockPosition.x, blockPosition.y));
        GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(blockPosition.x, blockPosition.y + 1));
        GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(blockPosition.x, blockPosition.y - 1));
        GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(blockPosition.x - 1, blockPosition.y));
        GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(blockPosition.x + 1, blockPosition.y));
    }

    private void InteractWithItemInHotbar()
    {
        switch (_inventoryData.GetSelectedItem().ItemData)
        {
            case BlockItemSO blockItem:
                {
                    PlaceBlock(blockItem);
                }
                break;
            default:
                break;
        }
    }

    //Change
    public void PlaceBlock(BlockItemSO blockItem)
    {
        if (!_isMouseInsideArea || !_isPlacingAllowed)
        {
            return;
        }

        Vector3 clickPosition = Input.mousePosition;
        Vector2Int intPos = Vector2Int.FloorToInt(Camera.main.ScreenToWorldPoint(clickPosition));
        _checkPlayerBoxCast.BoxCast(intPos);
        if (_checkPlayerBoxCast.Result)
        {
            return;
        }
        if (!WorldDataManager.Instance.IsFree(intPos.x, intPos.y))
        {
            return;
        }
        if (WorldDataManager.Instance.IsEmpty(intPos.x, intPos.y) && 
            (WorldDataManager.Instance.IsWall(intPos.x, intPos.y) || WorldDataManager.Instance.IsSolidAnyNeighbor(intPos.x, intPos.y)))
        {
            GameManager.Instance.Terrain.CreateBlock(intPos.x, intPos.y, blockItem.BlockToPlace);
            _inventoryData.DecreaseSelectedItemQuantity(1);
            UpdateNeighboringBlocks(intPos);
            StartCoroutine(WaitForPlacingCooldown());
        }
    }

    private IEnumerator WaitForPlacingCooldown()
    {
        _isPlacingAllowed = false;
        yield return new WaitForSeconds(_placingCooldown);
        _isPlacingAllowed = true;
    }

    //Change
    public void BreakBlock()
    {
        if (Input.GetMouseButton((int)MouseButton.Right))
        {
            Vector3 clickPosition = Input.mousePosition;
            Vector2Int blockPosition = Vector2Int.FloorToInt(Camera.main.ScreenToWorldPoint(clickPosition));
            if (!WorldDataManager.Instance.IsBreakable(blockPosition.x, blockPosition.y))
            {
                return;
            }
            if (!WorldDataManager.Instance.IsEmpty(blockPosition.x, blockPosition.y))
            {
                ref WorldCellData block = ref WorldDataManager.Instance.GetWorldCellData(blockPosition.x, blockPosition.y);
                _miningDamageController.AddDamageToBlock(blockPosition, _blockDamageMultiplier);
                if (_miningDamageController.IsBlockDamageReachedMaximum(blockPosition, block.BlockData.MaximumDamage))
                {
                    _miningDamageController.RemoveDamageFromBlocks(blockPosition);
                    CreateDrop(new Vector3(blockPosition.x + 0.5f, blockPosition.y + 0.5f), block.BlockData.Drop, 1);
                    GameManager.Instance.Terrain.CreateBlock(blockPosition.x, blockPosition.y, GameManager.Instance.BlocksAtlas.Air);
                    UpdateNeighboringBlocks(blockPosition);
                }
            }
        }
    }

    //Change
    public void BreakWall()
    {
        if (Input.GetMouseButton((int)MouseButton.Middle))
        {
            Vector3 clickPosition = Input.mousePosition;
            Vector2Int wallPosition = Vector2Int.FloorToInt(Camera.main.ScreenToWorldPoint(clickPosition));
            if (!WorldDataManager.Instance.IsBreakable(wallPosition.x, wallPosition.y))
            {
                return;
            }
            if (WorldDataManager.Instance.IsWall(wallPosition.x, wallPosition.y))
            {
                ref WorldCellData block = ref WorldDataManager.Instance.GetWorldCellData(wallPosition.x, wallPosition.y);
                _miningDamageController.AddDamageToWall(wallPosition, _wallDamageMultiplier);
                if (_miningDamageController.IsWallDamageReachedMaximum(wallPosition, block.WallData.MaximumDamage))
                {
                    _miningDamageController.RemoveDamageFromWalls(wallPosition);
                    CreateDrop(new Vector3(wallPosition.x + 0.5f, wallPosition.y + 0.5f), block.WallData.Drop, 1);
                    GameManager.Instance.Terrain.CreateWall(wallPosition.x, wallPosition.y, GameManager.Instance.BlocksAtlas.AirWall);
                }
            }
        }
    }

    //Delete
    public void CreateWater()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Vector3 clickPosition = Input.mousePosition;

            Vector3Int intPos = Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(clickPosition));

            GameManager.Instance.Terrain.CreateLiquidBlock((ushort)intPos.x, (ushort)intPos.y, (byte)GameManager.Instance.BlocksAtlas.Water.GetId());

            GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x, intPos.y));
        }
    }

    //Delete
    public void CreateTorch()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Vector3 clickPosition = Input.mousePosition;

            Vector2Int intPos = Vector2Int.FloorToInt(Camera.main.ScreenToWorldPoint(clickPosition));

            GameManager.Instance.Terrain.CreateBlock((ushort)intPos.x, (ushort)intPos.y, GameManager.Instance.BlocksAtlas.GetBlockById(FurnitureBlocksID.Torch));

            UpdateNeighboringBlocks(intPos);
        }
    }

    public void TakeDrop(Drop drop)
    {
        if (drop.Quantity == 0)
        {
            return;
        }
        int remainder = _inventoryData.AddItem(drop.Item, drop.Quantity, ItemLocations.Hotbar);
        drop.Quantity = remainder;

        if (remainder != 0)
        {
            remainder = _inventoryData.AddItem(drop.Item, drop.Quantity, ItemLocations.Storage);
            drop.Quantity = remainder;
        }
    }

    public bool IsEnoughSpaceToTakeDrop(Drop drop)
    {
        return !_inventoryData.IsEnoughSpaceForItem(drop.Item, ItemLocations.Hotbar) || !_inventoryData.IsEnoughSpaceForItem(drop.Item, ItemLocations.Storage);
    }

    public void ThrowSelectedItem()
    {
        Drop drop = CreateDrop(transform.position, _inventoryData.GetSelectedItem().ItemData, _inventoryData.GetSelectedItem().Quantity);
        if (drop != null)
        {
            DropPhysics dropPhysics = drop.GetComponent<DropPhysics>();
            dropPhysics.AddForce();
            _inventoryData.RemoveSelectedItem();
        }
    }

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

    public void HandleThrowItem(ItemSO itemData, int quantity)
    {
        CreateDrop(transform.position, itemData, quantity);
    }
    #endregion
}