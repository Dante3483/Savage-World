using UnityEngine;

public class HotbarController : MonoBehaviour
{
    #region Private fields
    [Header("Main")]
    [SerializeField]
    private Player _player;
    [SerializeField]
    private UIHotbarPage _hotbarUI;
    private Inventory _inventory;

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
        if (_player is null)
        {
            _player = GetComponentInParent<Player>();
        }
        PrepareData();
        PrepareUI();
    }

    private void Update()
    {
        CheckHotbarKeyClicked();
        CheckMouseScroll();
    }

    private void PrepareData()
    {
        _inventory = _player.Inventory;
        _inventory.OnHotbarChanged += HandleUpdateHotbarUI;
    }

    private void PrepareUI()
    {
        _hotbarUI = UIManager.Instance.HotbarUI.Content.GetComponentInChildren<UIHotbarPage>();
        _hotbarUI.InitializePage(_inventory.HotbarSize);
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
        int newIndex = _inventory.HotbarSelectedIndex + scrollDeltaY;
        if (newIndex >= _inventory.HotbarSize)
        {
            newIndex = 0;
        }
        if (newIndex < 0)
        {
            newIndex = _inventory.HotbarSize - 1;
        }
        SelectCell(newIndex);
    }

    private void SelectCell(int index)
    {
        _inventory.SelectItem(index);
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
