using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private UIInventoryPage _inventoryUI;
    [SerializeField] private InventorySO _inventoryData;
    [SerializeField] private ItemsAtlas _itemsAtlas;
    [Header("RMB clamp")]
    [SerializeField] private float _maxTimeToTakeOne;
    [SerializeField] private float _minTimeToTakeOne;
    [SerializeField] private float _stepTimeToTakeOne;
    [SerializeField] private float _currentTimeToTakeOne;
    [SerializeField] private bool _readyToTakeOne;

    public List<InventoryItem> initialItems = new List<InventoryItem>();

    public ItemsAtlas ItemsAtlas
    {
        get
        {
            return _itemsAtlas;
        }

        set
        {
            _itemsAtlas = value;
        }
    }

    private void Awake()
    {
        _itemsAtlas.LoadData();
    }

    private void Start()
    {
        PrepareUI();
        PrepareInventoryData();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (_inventoryUI.isActiveAndEnabled == false)
            {
                _inventoryUI.Show();
                foreach (var item in _inventoryData.GetCurrentInventoryState())
                {
                    if (item.Value.IsEmpty)
                    {
                        continue;
                    }
                    _inventoryUI.UpdateItemData(item.Key, item.Value.Item.ItemImage, item.Value.Quantity, item.Value.Item.ItemType);
                }
            }
            else
            {
                if (!_inventoryData.ItemInChangeState.IsEmpty)
                {
                    InventoryItem remainderItem = _inventoryData.CloseInventory(_inventoryData.ItemInChangeState);
                    if (!remainderItem.IsEmpty)
                    {
                        Vector3 playerPosition = gameObject.transform.position;
                        GetComponent<Interactions>().CreateDrop(playerPosition, remainderItem.Item, remainderItem.Quantity, true);
                    }
                }
                _inventoryUI.Hide();
            }
        }
    }

    private void PrepareUI()
    {
        _inventoryUI.InitializeInventoryUI(_inventoryData.Size);
        this._inventoryUI.OnDescpriptionRequested += HandleDescriptionRequest;
        this._inventoryUI.OnItemStartChangingCell += HandleBeginDragging;
        this._inventoryUI.OnItemEndChangingCell += HandleEndDragging;
        this._inventoryUI.OnItemAction += HandleItemAction;
        this._inventoryUI.OnItemStopChangeOne += HandleResetTimer;
        this._inventoryUI.OnItemDrop += HandleDropItem;
        this._inventoryUI.OnNeedEquipArmor += HandleEquipArmor;
        this._inventoryUI.OnNeedRemoveArmor += HandleRemoveArmor;
    }

    private void PrepareInventoryData()
    {
        _inventoryData.Initialize();
        _inventoryData.OnInventoryChanged += HandleNeedUpdateUI;
        foreach (InventoryItem item in initialItems)
        {
            if (item.IsEmpty)
            {
                continue;
            }
            _inventoryData.AddItem(item);
        }
    }

    private void HandleNeedUpdateUI(Dictionary<int, InventoryItem> inventoryState, List<InventoryItem> armorState)
    {
        foreach (var item in inventoryState)
        {
            if (item.Value.IsEmpty)
            {
                _inventoryUI.UpdateItemData(item.Key, null, 0, 0);
            }
            else
            {
                _inventoryUI.UpdateItemData(item.Key, item.Value.Item.ItemImage, item.Value.Quantity, item.Value.Item.ItemType);
            }
        }

        List<Sprite> playerView = new List<Sprite>();

        for (int i = 0; i < armorState.Count; i++)
        {
            playerView.Add(armorState[i].IsEmpty ? null : (armorState[i].Item as ArmorItemSO).PlayerView);
            _inventoryUI.UpdateArmorData(i, armorState[i]);
        }

        for (int i = 0; i < playerView.Count / 2; i++)
        {
            _inventoryUI.UpdatePlayerView(playerView[i + armorState.Count / 2] != null ? playerView[i + armorState.Count / 2] : playerView[i], i);
        }

        if (_inventoryData.ItemInChangeState.IsEmpty)
        {
            _inventoryUI.ResetDraggedItem();
        }
        else
        {
            _inventoryUI.CreateDraggedItem(_inventoryData.ItemInChangeState.Item.ItemImage, _inventoryData.ItemInChangeState.Quantity);
        }
    }

    private void HandleDropItem()
    {
        if (!_inventoryData.ItemInChangeState.IsEmpty)
        {
            Vector3 playerPosition = gameObject.transform.position;
            GetComponent<Interactions>().CreateDrop(playerPosition, _inventoryData.ItemInChangeState.Item, _inventoryData.ItemInChangeState.Quantity, true);
            _inventoryData.ItemInChangeState = InventoryItem.GetEmptyItem();
            _inventoryUI.ResetDraggedItem();
        }
    }

    private void HandleItemAction(int itemIndex)
    {
        InventoryItem inventoryItem = _inventoryData.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
        {
            return;
        }
        switch (inventoryItem.Item.ItemType)
        {
            case ItemType.Block:
                {
                    TakeItem(itemIndex, inventoryItem);
                    _inventoryUI.ResetTooltipDescription();
                    _inventoryUI.IsItemChangeCell = true;
                }
                break;
            case ItemType.Tool:
                break;
            case ItemType.Weapon:
                break;
            case ItemType.Armor:
                {
                    if (_inventoryData.ItemInChangeState.IsEmpty)
                    {
                        EquipArmor(itemIndex);
                        _inventoryUI.ResetTooltipDescription();
                        if (!_inventoryData.GetItemAt(itemIndex).IsEmpty)
                        {
                            HandleDescriptionRequest(itemIndex);
                        }
                    }
                }
                break;
            case ItemType.Accessory:
                break;
            default:
                break;
        }
    }

    #region Take one item
    private void HandleResetTimer()
    {
        _readyToTakeOne = true;
        _currentTimeToTakeOne = _maxTimeToTakeOne;
    }

    private IEnumerator CooldownTimer()
    {
        yield return new WaitForSeconds(_currentTimeToTakeOne);
        _readyToTakeOne = true;
    }

    private void TakeItem(int itemIndex, InventoryItem inventoryItem)
    {
        if (_inventoryData.ItemInChangeState.IsEmpty)
        {
            _readyToTakeOne = false;
            _currentTimeToTakeOne = _maxTimeToTakeOne;
            StartCoroutine(CooldownTimer());

            int quantity = _inventoryData.RemoveItemAt(itemIndex, 1);
            _inventoryData.AddItemInBuffer(inventoryItem, quantity);
            _inventoryUI.CreateDraggedItem(_inventoryData.ItemInChangeState.Item.ItemImage, _inventoryData.ItemInChangeState.Quantity);
        }
        else
        {
            if (_readyToTakeOne)
            {
                if (_inventoryData.ItemInChangeState.Item == inventoryItem.Item)
                {
                    _readyToTakeOne = false;
                    _currentTimeToTakeOne = Mathf.Lerp(_currentTimeToTakeOne, _minTimeToTakeOne, _stepTimeToTakeOne);
                    StartCoroutine(CooldownTimer());

                    int turboQuantity = _currentTimeToTakeOne == _minTimeToTakeOne ? 5 : 0;

                    int quantity = _inventoryData.RemoveItemAt(itemIndex, 1 + turboQuantity);
                    _inventoryData.AddItemInBuffer(inventoryItem, quantity);
                    _inventoryUI.CreateDraggedItem(_inventoryData.ItemInChangeState.Item.ItemImage, _inventoryData.ItemInChangeState.Quantity);
                }
            }
        }
    }

    private void EquipArmor(int itemIndex)
    {
        _inventoryData.QuickSetArmor(itemIndex);
    }
    #endregion

    #region Take all items in cell
    private void HandleBeginDragging(int itemIndex)
    {
        InventoryItem inventoryItem = _inventoryData.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
        {
            return;
        }
        _inventoryData.AddItemInBuffer(inventoryItem);
        _inventoryData.RemoveItemAt(itemIndex);
        _inventoryUI.CreateDraggedItem(inventoryItem.Item.ItemImage, inventoryItem.Quantity);
    }

    private void HandleEndDragging(int itemIndex)
    {
        InventoryItem inventoryItem = _inventoryData.GetItemAt(itemIndex);
        _inventoryData.AddItemAt(_inventoryData.ItemInChangeState, itemIndex);

        if (_inventoryData.ItemInChangeState.IsEmpty)
        {
            _inventoryUI.ResetDraggedItem();
        }
        else
        {
            _inventoryUI.CreateDraggedItem(_inventoryData.ItemInChangeState.Item.ItemImage, _inventoryData.ItemInChangeState.Quantity);
        }
    }
    #endregion

    #region Create tooltip description
    private void HandleDescriptionRequest(int itemIndex)
    {
        InventoryItem inventoryItem = _inventoryData.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
        {
            return;
        }
        ItemSO item = inventoryItem.Item;
        _inventoryUI.CreateTooltipDescription(item.GetDescription());
    }
    #endregion

    #region Equip/Remove armor
    private void HandleEquipArmor(ArmorType type)
    {
        _inventoryData.SetArmor(type);
    }

    private void HandleRemoveArmor(ArmorType type)
    {
        _inventoryData.QuickRemoveArmor(type);
    }
    #endregion


    #region Save/Load
    public InventorySO GetInventory()
    {
        return _inventoryData;
    }
    #endregion
}
