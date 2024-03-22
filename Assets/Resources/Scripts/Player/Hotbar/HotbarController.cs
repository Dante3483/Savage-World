using Inventory;
using UnityEngine;

public class HotbarController : MonoBehaviour
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private UIHotbarPage _hotbarUI;
    [SerializeField] private InventorySO _inventoryData;

    private KeyCode[] _hotbarKeys = new KeyCode[]
    {
        KeyCode.Alpha1,KeyCode.Alpha2, 
        KeyCode.Alpha3,KeyCode.Alpha4, 
        KeyCode.Alpha5,
    };
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        UIManager.Instance.HotbarUI.IsActive = true;
        PrepareUI();
        PrepareInventoryData();
    }

    private void Update()
    {
        CheckHotbarKeyClicked();
        CheckMouseScroll();
    }

    private void PrepareUI()
    {
        _hotbarUI.InitializePage(_inventoryData.HotbarSize);
    }

    private void PrepareInventoryData()
    {
        _inventoryData.OnHotbarChanged += HandleUpdateHotbarUI;
        SelectCell(0);
    }

    private void CheckHotbarKeyClicked()
    {
        for (int i = 0; i < _hotbarKeys.Length; i++)
        {
            if (!GameManager.Instance.IsInputTextInFocus && Input.GetKeyDown(_hotbarKeys[i]))
            {
                SelectCell(i);
                return;
            }
        }
    }

    private void CheckMouseScroll()
    {
        int scrollDeltaY = Mathf.Clamp((int)Input.mouseScrollDelta.y, -1, 1);
        int newIndex = _inventoryData.HotbarSelectedIndex + scrollDeltaY;
        if (newIndex >= _inventoryData.HotbarSize)
        {
            newIndex = 0;
        }
        if (newIndex < 0)
        {
            newIndex = _inventoryData.HotbarSize - 1;
        }
        SelectCell(newIndex);
    }

    private void SelectCell(int index)
    {
        _inventoryData.SelectItem(index);
        _hotbarUI.SelectCell(index);
    }

    private void HandleUpdateHotbarUI(InventoryItem[] hotbarState, int startIndex, int count)
    {
        for (int i = startIndex; i < count; i++)
        {
            _hotbarUI.UpdateHotbarItemData(i, hotbarState[i].ItemData?.SmallItemImage, hotbarState[i].Quantity);
        }
    }
    #endregion
}
