using Items;
using UnityEngine.InputSystem;

public class HotbarPresenter : PresenterBaseGeneric<InventoryModel, HotbarView>
{
    #region Fields

    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Public Methods
    public HotbarPresenter(InventoryModel model, HotbarView view) : base(model, view)
    {
        GameManager.Instance.PlayerInputActions.UI.SelectHotbarCellByKeyboard.performed += SelectCell;
        GameManager.Instance.PlayerInputActions.UI.SelectHotbarCellByScrolling.performed += ScrollCell;
    }

    public override void ResetPresenter()
    {

    }
    #endregion

    #region Private Methods
    protected override void InitializeModel()
    {
        _model.ItemDataChanged += ItemDataChangedEventHandler;
        _model.ItemQuantityChanged += ItemQuantityChangedEventHandler;
        _model.SelectedItemChanged += SelectedItemChangedEventHandler;
    }

    protected override void InitializeView()
    {
        _view.Configure(_model.HotbarSize);
        _view.Initialize();
    }

    private void SelectCell(InputAction.CallbackContext context)
    {
        _model.SelectHotbarItem((int)context.ReadValue<float>());
    }

    private void ScrollCell(InputAction.CallbackContext context)
    {
        _model.SelectAdjacentItem((int)context.ReadValue<float>());
    }

    private void ItemDataChangedEventHandler(ItemSO data, int index, ItemLocations location)
    {
        if (location != ItemLocations.Hotbar)
        {
            return;
        }
        _view.UpdateCellSprite(data?.SmallItemImage, index);
    }

    private void ItemQuantityChangedEventHandler(int quantity, int index, ItemLocations location)
    {
        if (location != ItemLocations.Hotbar)
        {
            return;
        }
        _view.UpdateCellQuantity(quantity, index);
    }

    private void SelectedItemChangedEventHandler(int index)
    {
        if (index == -1)
        {
            _view.DeselectAllCells();
        }
        else
        {
            _view.SelectCell(index);
        }
    }
    #endregion
}
