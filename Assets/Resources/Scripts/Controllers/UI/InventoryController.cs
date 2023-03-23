using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private UIInventoryPage _inventoryUI;
    [SerializeField] private InventorySO _inventoryData;
    [SerializeField] private float _maxTimeToTakeOne;
    [SerializeField] private float _minTimeToTakeOne;
    [SerializeField] private float _stepTimeToTakeOne;
    [SerializeField] private float _currentTimeToTakeOne;
    [SerializeField] private bool _readyToTakeOne;

    public List<InventoryItem> initialItems = new List<InventoryItem>();

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
                    _inventoryUI.UpdateData(item.Key, item.Value.Item.ItemImage, item.Value.Quantity);
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
        this._inventoryUI.OnItemChangeOne += HandleTakeOneItem;
        this._inventoryUI.OnItemStopChangeOne += HandleResetTimer;
        this._inventoryUI.OnItemDrop += HandleDropItem;
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

    private void HandleNeedUpdateUI(Dictionary<int, InventoryItem> inventoryState)
    {
        foreach (var item in inventoryState)
        {
            if (item.Value.IsEmpty)
            {
                _inventoryUI.UpdateData(item.Key, null, 0);
            }
            else
            {
                _inventoryUI.UpdateData(item.Key, item.Value.Item.ItemImage, item.Value.Quantity);
            }
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

    #region Take one item
    private void HandleTakeOneItem(int itemIndex)
    {
        InventoryItem inventoryItem = _inventoryData.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
        {
            return;
        }
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
}
