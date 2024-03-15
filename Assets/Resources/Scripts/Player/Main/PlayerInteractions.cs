using Inventory;
using Items;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    #region Private fields
    [SerializeField] private Drop _dropPrefab;
    [SerializeField] private ItemSO _item;
    [SerializeField] private InventorySO _inventoryData;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        _inventoryData.OnInventoryFull += HandleThrowItem;
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameSession)
        {
            if (Input.GetMouseButton((int)MouseButton.Left))
            {
                InteractWithItemInHotbar();
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                ThrowSelectedItem();
            }
            BreakBlock();
            CreateWater();
            CreateTorch();
        }
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
        Vector3 clickPosition = Input.mousePosition;
        Vector3Int intPos = Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(clickPosition));

        if (WorldDataManager.Instance.WorldData[intPos.x, intPos.y].IsEmpty())
        {
            GameManager.Instance.Terrain.CreateBlock(intPos.x, intPos.y, blockItem.BlockToPlace);
            _inventoryData.DecreaseSelectedItemQuantity(1);

            GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x, intPos.y));
            GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x, intPos.y + 1));
            GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x, intPos.y - 1));
            GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x - 1, intPos.y));
            GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x + 1, intPos.y));
        }
    }

    //Change
    public void BreakBlock()
    {
        if (Input.GetMouseButton((int)MouseButton.Right))
        {
            Vector3 clickPosition = Input.mousePosition;

            Vector3Int intPos = Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(clickPosition));
            if (!WorldDataManager.Instance.WorldData[intPos.x, intPos.y].IsEmpty())
            {
                ref WorldCellData block = ref WorldDataManager.Instance.GetWorldCellData(intPos.x, intPos.y);

                CreateDrop(new Vector3(intPos.x + 0.5f, intPos.y + 0.5f), block.BlockData.Drop, 1);

                GameManager.Instance.Terrain.CreateBlock(intPos.x, intPos.y, GameManager.Instance.BlocksAtlas.Air);

                GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x, intPos.y));
                GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x, intPos.y + 1));
                GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x, intPos.y - 1));
                GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x - 1, intPos.y));
                GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x + 1, intPos.y));
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

            Vector3Int intPos = Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(clickPosition));

            GameManager.Instance.Terrain.CreateBlock((ushort)intPos.x, (ushort)intPos.y, GameManager.Instance.BlocksAtlas.GetBlockById(FurnitureBlocksID.Torch));

            GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x, intPos.y));
            GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x, intPos.y + 1));
            GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x, intPos.y - 1));
            GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x - 1, intPos.y));
            GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x + 1, intPos.y));
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
