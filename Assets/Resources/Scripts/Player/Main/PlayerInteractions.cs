using Items;
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
    [Min(0.001f)][SerializeField] private float _blockHealingMultiplier;    
    [Min(0.001f)][SerializeField] private float _backgroundDamageMultiplier;
    [Min(0.001f)][SerializeField] private float _backgroundHealingMultiplier;
    private Vector2Int _miningPosition;
    private Dictionary<Vector2Int, float> _damageByBlockPosition;
    private Dictionary<Vector2Int, float> _damageByBackgroundPosition;
    private SetDamageDelegate _setBlockDamage;
    private SetDamageDelegate _setBackgroundDamage;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        _damageByBlockPosition = new Dictionary<Vector2Int, float>();
        _damageByBackgroundPosition = new Dictionary<Vector2Int, float>();
        _setBlockDamage = (ref WorldCellData block, float damage) => block.SetBlockDamagePercent(damage);
        _setBackgroundDamage = (ref WorldCellData block, float damage) => block.SetBackgroundDamagePercent(damage);
        _inventoryData.OnInventoryFull += HandleThrowItem;
    }

    private void FixedUpdate()
    {
        IsMouseInsideArea();
        UpdateAllDamage();
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
            BreakBlock();
            BreakBackground();
            CreateWater();
            CreateTorch();
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

    private void UpdateAllDamage()
    {
        UpdateDamage(_damageByBlockPosition, _setBlockDamage, _blockHealingMultiplier);
        UpdateDamage(_damageByBackgroundPosition, _setBackgroundDamage, _backgroundHealingMultiplier);
        _miningPosition.x = -1;
        _miningPosition.y = -1;
    }

    private void UpdateDamage(Dictionary<Vector2Int, float> damageCollection, SetDamageDelegate setDamageAction, float healingMultiplier)
    {
        foreach (Vector2Int key in damageCollection.Keys.ToList())
        {
            setDamageAction?.Invoke(ref WorldDataManager.Instance.GetWorldCellData(key.x, key.y), damageCollection[key]);
            if (key == _miningPosition)
            {
                continue;
            }
            damageCollection[key] -= Time.fixedDeltaTime / healingMultiplier;
            if (damageCollection[key] <= 0)
            {
                damageCollection.Remove(key);
            }
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
        if (!_isMouseInsideArea)
        {
            return;
        }
        Vector3 clickPosition = Input.mousePosition;
        Vector2Int intPos = Vector2Int.FloorToInt(Camera.main.ScreenToWorldPoint(clickPosition));

        if (WorldDataManager.Instance.WorldData[intPos.x, intPos.y].IsEmpty())
        {
            GameManager.Instance.Terrain.CreateBlock(intPos.x, intPos.y, blockItem.BlockToPlace);
            _inventoryData.DecreaseSelectedItemQuantity(1);

            UpdateNeighboringBlocks(intPos);
        }
    }

    //Change
    public void BreakBlock()
    {
        if (!_isMouseInsideArea)
        {
            return;
        }
        if (!GameManager.Instance.IsInputTextInFocus && Input.GetMouseButton((int)MouseButton.Right))
        {
            Vector3 clickPosition = Input.mousePosition;

            Vector2Int blockPosition = Vector2Int.FloorToInt(Camera.main.ScreenToWorldPoint(clickPosition));
            if (!WorldDataManager.Instance.WorldData[blockPosition.x, blockPosition.y].IsEmpty())
            {
                ref WorldCellData block = ref WorldDataManager.Instance.GetWorldCellData(blockPosition.x, blockPosition.y);

                _miningPosition.x = blockPosition.x;
                _miningPosition.y = blockPosition.y;
                _damageByBlockPosition.TryAdd(blockPosition, 0f);
                _damageByBlockPosition[blockPosition] += Time.fixedDeltaTime * _blockDamageMultiplier;
                    
                if (_damageByBlockPosition[blockPosition] >= block.BlockData.BreakingTime)
                {
                    _damageByBlockPosition.Remove(blockPosition);
                    CreateDrop(new Vector3(blockPosition.x + 0.5f, blockPosition.y + 0.5f), block.BlockData.Drop, 1);
                    GameManager.Instance.Terrain.CreateBlock(blockPosition.x, blockPosition.y, GameManager.Instance.BlocksAtlas.Air);
                    UpdateNeighboringBlocks(blockPosition);
                }
            }
        }
    }

    //Change
    public void BreakBackground()
    {
        if (!_isMouseInsideArea)
        {
            return;
        }
        if (!GameManager.Instance.IsInputTextInFocus && Input.GetMouseButton((int)MouseButton.Middle))
        {
            Vector3 clickPosition = Input.mousePosition;

            Vector2Int backgroundPosition = Vector2Int.FloorToInt(Camera.main.ScreenToWorldPoint(clickPosition));
            if (WorldDataManager.Instance.WorldData[backgroundPosition.x, backgroundPosition.y].IsBackground())
            {
                ref WorldCellData block = ref WorldDataManager.Instance.GetWorldCellData(backgroundPosition.x, backgroundPosition.y);

                _miningPosition.x = backgroundPosition.x;
                _miningPosition.y = backgroundPosition.y;
                _damageByBackgroundPosition.TryAdd(backgroundPosition, 0f);
                _damageByBackgroundPosition[backgroundPosition] += Time.fixedDeltaTime * _backgroundDamageMultiplier;

                if (_damageByBackgroundPosition[backgroundPosition] >= block.BackgroundData.BreakingTime)
                {
                    _damageByBackgroundPosition.Remove(backgroundPosition);
                    CreateDrop(new Vector3(backgroundPosition.x + 0.5f, backgroundPosition.y + 0.5f), block.BackgroundData.Drop, 1);
                    GameManager.Instance.Terrain.CreateBackground(backgroundPosition.x, backgroundPosition.y, GameManager.Instance.BlocksAtlas.AirBG);
                }
            }
        }
    }

    //Delete
    public void CreateWater()
    {
        if (!_isMouseInsideArea)
        {
            return;
        }
        if (!GameManager.Instance.IsInputTextInFocus && Input.GetKeyDown(KeyCode.U))
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
        if (!_isMouseInsideArea)
        {
            return;
        }
        if (!GameManager.Instance.IsInputTextInFocus && Input.GetKeyDown(KeyCode.L))
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