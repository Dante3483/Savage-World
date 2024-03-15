using Inventory;
using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class InventoryController : MonoBehaviour, IBookPageController
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private UIInventoryPage _inventoryUI;
    [SerializeField] private InventorySO _inventoryData;

    [Header("Test data")]
    [SerializeField] private bool _isTestDataEnabled;
    [SerializeField] private List<ItemSO> _initStorageItems;
    [SerializeField] private List<ItemSO> _initHotbarItems;
    [SerializeField] private List<ItemSO> _initAccessoriesItems;

    [SerializeField] private float _minTimeToTakeItem;
    [SerializeField] private float _maxTimeToTakeItem;
    [SerializeField] private float _stepToLerpTimeToTakeItem;
    [SerializeField] private float _currentTimeToTakeItem;

    private Coroutine _takeItemCoroutine;
    private bool _isTurboTakeItem;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        PrepareUI();
        PrepareData();
        _currentTimeToTakeItem = _maxTimeToTakeItem;
    }

    private void Start()
    {
        if (_isTestDataEnabled)
        {
            foreach (ItemSO item in _initStorageItems)
            {
                _inventoryData.AddItem(item, Random.Range(10, item.MaxStackSize), ItemLocations.Storage);
            }
            foreach (ItemSO item in _initHotbarItems)
            {
                _inventoryData.AddItem(item, Random.Range(10, item.MaxStackSize), ItemLocations.Hotbar);
            }
            foreach (ItemSO item in _initAccessoriesItems)
            {
                _inventoryData.AddItem(item, Random.Range(10, item.MaxStackSize), ItemLocations.Accessories);
            }
        }
    }

    public void PrepareUI()
    {
        _inventoryUI.InitializePage(_inventoryData.StorageSize, _inventoryData.HotbarSize, _inventoryData.AccessoriesSize);

        _inventoryUI.OnDraggingItem += HandleDragItem;
        _inventoryUI.OnStartTakeItem += HandleStartTakeItem;
        _inventoryUI.OnStopTakeItem += HandleStopTakeItem;
        _inventoryUI.OnDescriptionRequested += HandleDescriptionRequest;
    }

    public void PrepareData()
    {
        _inventoryData.Initialize();
        _inventoryData.OnStorageChanged += HandleUpdateStorageUI;
        _inventoryData.OnHotbarChanged += HandleUpdateHotbarUI;
        _inventoryData.OnAccessoriesChanged += HandleUpdateAccessoriesUI;
        _inventoryData.OnArmorChanged += HandleUpdateArmorUI;
        _inventoryData.OnBufferItemChanged += HandleUpdateItemInBufferUI;
    }

    public void ResetData()
    {
        UIManager.Instance.InventoryUI.ReverseActivity();
        if (!UIManager.Instance.InventoryUI.IsActive)
        {
            _inventoryUI.ResetPage();
            _inventoryData.ClearBuffer();
        }
    }

    private void HandleUpdateStorageUI(InventoryItem[] storageState)
    {
        for (int i = 0; i < storageState.Length; i++)
        {
            _inventoryUI.UpdateStorageItemData(i, storageState[i].ItemData?.ItemImage, storageState[i].Quantity);
        }
    }

    private void HandleUpdateHotbarUI(InventoryItem[] hotbarState, int startIndex, int count)
    {
        for (int i = startIndex; i < count; i++)
        {
            _inventoryUI.UpdateHotbarItemData(i, hotbarState[i].ItemData?.ItemImage, hotbarState[i].Quantity);
        }
    }
     
    private void HandleUpdateAccessoriesUI(InventoryItem[] accessoriesState)
    {
        for (int i = 0; i < accessoriesState.Length; i++)
        {
            _inventoryUI.UpdateAccessoriesItemData(i, accessoriesState[i].ItemData?.ItemImage);
        }
    }

    private void HandleUpdateArmorUI(InventoryItem[] armorState)
    {
        for (int i = 0; i < armorState.Length; i++)
        {
            _inventoryUI.UpdateArmorItemData(i, armorState[i].ItemData?.ItemImage);
        }
    }

    private void HandleUpdateItemInBufferUI(InventoryItem bufferState)
    {
        _inventoryUI.UpdateItemInBufferData(bufferState.ItemData?.ItemImage, bufferState.Quantity);
    }

    private void HandleDescriptionRequest(int itemIndex, ItemLocations itemLocation)
    {
        _inventoryUI.UpdateTooltip(_inventoryData.GetItemDescription(itemIndex, itemLocation));
    }

    private void HandleDragItem(int itemIndex, ItemLocations itemLocation)
    {
        _inventoryData.TakeItem(itemIndex, itemLocation);
    }

    private void HandleStartTakeItem(int itemIndex, ItemLocations itemLocation)
    {
        if (_takeItemCoroutine == null)
        {
            _currentTimeToTakeItem = _inventoryData.CompareItemWithBuffer(itemIndex, itemLocation) ? _currentTimeToTakeItem : _maxTimeToTakeItem;
            _takeItemCoroutine = StartCoroutine(TakeItemCoroutine(itemIndex, itemLocation));
        }
    }

    private void HandleStopTakeItem()
    {
        _currentTimeToTakeItem = _maxTimeToTakeItem;
        if (_takeItemCoroutine != null)
        {
            StopCoroutine(_takeItemCoroutine);
            _takeItemCoroutine = null;
        }
    }

    private IEnumerator TakeItemCoroutine(int index, ItemLocations location)
    {
        _isTurboTakeItem = _currentTimeToTakeItem <= (_minTimeToTakeItem + _minTimeToTakeItem / 10);
        _inventoryData.TakeItem(index, location, _isTurboTakeItem ? 50 : 1);
        yield return new WaitForSeconds(_currentTimeToTakeItem);
        _currentTimeToTakeItem = Mathf.Lerp(_currentTimeToTakeItem, _minTimeToTakeItem, _stepToLerpTimeToTakeItem);
        _takeItemCoroutine = null;
    }
    #endregion
}
