using Items;
using System;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerInteractions : MonoBehaviour
{
    #region Private fields
    [SerializeField] private Drop _dropPrefab;
    [SerializeField] private ItemSO _item;
    [SerializeField] private InventorySO _inventory;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
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
                ThrowItem();
            }
            BreakBlock();
            CreateWater();
            CreateTorch();
        }
    }

    private void InteractWithItemInHotbar()
    {
        switch (_inventory.GetSelectedItem().ItemData)
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

    public void PlaceBlock(BlockItemSO blockItem)
    {
        Vector3 clickPosition = Input.mousePosition;
        Vector3Int intPos = Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(clickPosition));

        if (WorldDataManager.Instance.WorldData[intPos.x, intPos.y].IsEmpty())
        {
            GameManager.Instance.Terrain.CreateBlock(intPos.x, intPos.y, blockItem.BlockToPlace);
            _inventory.DecreaseSelectedItemQuantity(1);

            GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x, intPos.y));
            GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x, intPos.y + 1));
            GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x, intPos.y - 1));
            GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x - 1, intPos.y));
            GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x + 1, intPos.y));
        }
    }

    public void BreakBlock()
    {
        if (Input.GetMouseButton((int)MouseButton.Right))
        {
            Vector3 clickPosition = Input.mousePosition;

            Vector3Int intPos = Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(clickPosition));
            if (!WorldDataManager.Instance.WorldData[intPos.x, intPos.y].IsEmpty())
            {
                ref WorldCellData block = ref WorldDataManager.Instance.GetWorldCellData(intPos.x, intPos.y);

                CreateDrop(new Vector3(intPos.x + 0.5f, intPos.y + 0.5f), block.BlockData.Drop);

                GameManager.Instance.Terrain.CreateBlock(intPos.x, intPos.y, GameManager.Instance.BlocksAtlas.Air);

                GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x, intPos.y));
                GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x, intPos.y + 1));
                GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x, intPos.y - 1));
                GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x - 1, intPos.y));
                GameManager.Instance.Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x + 1, intPos.y));
            }
        }
    }

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
        int remainder = _inventory.AddItem(drop.Item, drop.Quantity, ItemLocations.Storage);
        drop.Quantity = remainder;
    }

    public void ThrowItem()
    {
        Drop drop = CreateDrop(transform.position, _inventory.GetSelectedItem().ItemData);
        if (drop != null)
        {
            drop.AddForce();
            drop.Quantity = _inventory.GetSelectedItem().Quantity;
            _inventory.RemoveSelectedItem();
        }
    }

    private Drop CreateDrop(Vector3 position, ItemSO item)
    {
        if (item == null)
        {
            return null;
        }
        Drop drop = Instantiate(_dropPrefab, position, Quaternion.identity);
        drop.Quantity = 1;
        drop.Item = item;
        return drop;
    }
    #endregion
}
