using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactions : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private World _world;
    [SerializeField] private InventorySO _inventory;
    [SerializeField] private InventorySO _hotbar;
    [SerializeField] private GameObject _dropSection;
    [SerializeField] private GameObject _dropPrefab;
    [SerializeField] private float _blockBreakingSpeed;
    [SerializeField] private bool _canPlaceBreakBlock;

    public bool CanPlaceBreakBlock
    {
        get
        {
            return _canPlaceBreakBlock;
        }

        set
        {
            _canPlaceBreakBlock = value;
        }
    }

    public Player Player { get => _player; set => _player = value; }

    private void Awake()
    {
        _dropSection = GameManager.Instance.DropSection;
        _world = GameManager.Instance.World;
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        ItemSO selectedItem = _inventory.GetSelectedItem();
        if (selectedItem != null && Input.GetMouseButton(0))
        {
            switch (selectedItem.ItemType)
            {
                case ItemType.Block:
                    {
                        if (CanPlaceBreakBlock)
                        {
                            PlaceBlock(selectedItem as BlockItemSO);
                        }
                    }
                    break;
                case ItemType.Tool:
                    {

                    }
                    break;
                case ItemType.Weapon:
                    {

                    }
                    break;
                case ItemType.Armor:
                    {

                    }
                    break;
                case ItemType.Accessory:
                    {

                    }
                    break;
                default:
                    break;
            }
        }
        if (Input.GetMouseButton(1))
        {
            if (CanPlaceBreakBlock)
            {
                BreakBlock();
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (GameManager.Instance.mouseFollowerOutline.CurrentOutlinedItem != null)
            {
                GameManager.Instance.mouseFollowerOutline.CurrentOutlinedItem.GetComponent<IInteractiveObject>().HandleInteraction();
            }
        }
    }

    public void PlaceBlock(BlockItemSO block)
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int intPosition = _world.BlockTilemap.WorldToCell(position);

        if (_world.IsValidPlaceToCreateBlock(intPosition))
        {
            _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.GetBlockByID(block.BlockToPlace.blockType, block.BlockToPlace.GetID()), intPosition.x, intPosition.y);
            _inventory.RemoveSelectedItem(1);
        }
    }

    public void BreakBlock()
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int intPosition = _world.BlockTilemap.WorldToCell(position);

        if (_world.IsValidPlaceToBreakBlock(intPosition))
        {
            if (_world.DecreaseDurability(new Vector2Int(intPosition.x, intPosition.y), _blockBreakingSpeed * Time.fixedDeltaTime))
            {
                //Create drop
                ObjectData dropData = _world.GetBlockByPosition(intPosition);
                BlockSO dropBlockSO = GameManager.Instance.WorldObjectAtlas.GetBlockByID(dropData.Type, dropData.Id);
                CreateDrop(intPosition, dropBlockSO.Drop, 1, false);

                //Create air block
                _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.Air, intPosition.x, intPosition.y);
            }
        }
    }

    public void PickUpDrop(Drop drop)
    {
        if (drop.IsDropCanBePickedUp)
        {
            int reminder = _inventory.AddItem(drop.DropItem, drop.Quantity);
            if (reminder == 0)
            {
                Destroy(drop.gameObject);
            }
            else
            {
                drop.Quantity = reminder;
            }
        }
    }

    public void CreateDrop(Vector3 position, ItemSO dropItem, int quantity, bool needCooldown)
    {
        if (dropItem != null)
        {
            GameObject dropGameObject = Instantiate(_dropPrefab);
            dropGameObject.name = dropItem.Name;
            dropGameObject.transform.parent = _dropSection.transform;
            dropGameObject.transform.position = position + new Vector3(Random.Range(0.1f, 0.9f), 0.5f);
            dropGameObject.GetComponent<SpriteRenderer>().sprite = dropItem.ItemImage;
            Drop drop = dropGameObject.GetComponent<Drop>();
            drop.DropItem = dropItem;
            drop.Quantity = quantity;
            if (needCooldown)
            {
                StartCoroutine(drop.WaitToMagnet());
            }
            else
            {
                drop.IsDropCanMagnet = true;
                drop.IsDropCanBePickedUp = true;
            }
        }
    }

    public bool IsDropCanBeAdded(ItemSO dropItem)
    {
        return _inventory.IsCanAddItemToInventory(dropItem);
    }
}
