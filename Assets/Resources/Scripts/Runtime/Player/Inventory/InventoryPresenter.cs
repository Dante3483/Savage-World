using Items;
using System.Collections;
using UnityEngine;

public class InventoryPresenter : PresenterBaseGeneric<InventoryModel, InventoryView>
{
    #region Fields
    private float _minTimeToTakeItem;
    private float _maxTimeToTakeItem;
    private float _stepToLerpTimeToTakeItem;
    private float _currentTimeToTakeItem;
    private Coroutine _takeItemCoroutine;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Public Methods
    public InventoryPresenter(float minTimeToTakeItem, float maxTimeToTakeItem, float stepToLerpTimeToTakeItem, InventoryModel model, InventoryView view) : base(model, view)
    {
        _minTimeToTakeItem = minTimeToTakeItem;
        _maxTimeToTakeItem = maxTimeToTakeItem;
        _stepToLerpTimeToTakeItem = stepToLerpTimeToTakeItem;
    }

    public override void ResetPresenter()
    {

    }
    #endregion

    #region Private Methods
    protected override void InitializeModel()
    {
        base.InitializeModel();
        _model.ItemDataChanged += ItemDataChangedEventHandler;
        _model.ItemQuantityChanged += ItemQuantityChangedEventHandler;
    }

    protected override void InitializeView()
    {
        _view.Configure(_model.StorageSize, _model.HotbarSize, _model.AccessoriesSize);
        _view.Initialize();
        _view.DragItemRequested += DragItemRequestedEventHandler;
        _view.TakeItemRequested += TakeItemRequestedEventHandler;
        _view.StopTakeItemRequested += StopTakeItemRequestedEventHandler;
        _view.DescriptionRequested += DescriptionRequestedEventHandler;
    }

    private IEnumerator TakeItemCoroutine(int index, ItemLocations location)
    {
        bool isTurboMode = _currentTimeToTakeItem <= (_minTimeToTakeItem + _minTimeToTakeItem / 10);
        _model.DragItem(index, location, isTurboMode ? 50 : 1);
        yield return new WaitForSeconds(_currentTimeToTakeItem);
        _currentTimeToTakeItem = Mathf.Lerp(_currentTimeToTakeItem, _minTimeToTakeItem, _stepToLerpTimeToTakeItem);
        _takeItemCoroutine = null;
    }

    private void ItemDataChangedEventHandler(ItemSO data, int index, ItemLocations location)
    {
        _view.UpdateCellSprite(data?.SmallItemImage, index, location);
    }

    private void ItemQuantityChangedEventHandler(int quantity, int index, ItemLocations location)
    {
        _view.UpdateCellQuantity(quantity, index, location);
    }

    private void DragItemRequestedEventHandler(int index, ItemLocations location)
    {
        _model.DragItem(index, location);
    }

    private void TakeItemRequestedEventHandler(int index, ItemLocations location)
    {
        if (_takeItemCoroutine == null)
        {
            _currentTimeToTakeItem = _model.CompareItemWithBuffer(index, location) ? _currentTimeToTakeItem : _maxTimeToTakeItem;
            _takeItemCoroutine = _view.StartCoroutine(TakeItemCoroutine(index, location));
        }
    }

    private void StopTakeItemRequestedEventHandler()
    {
        _currentTimeToTakeItem = _maxTimeToTakeItem;
        if (_takeItemCoroutine != null)
        {
            _view.StopCoroutine(_takeItemCoroutine);
            _takeItemCoroutine = null;
        }
    }

    private void DescriptionRequestedEventHandler(int index, ItemLocations location)
    {
        _view.UpdateTooltip(_model.GetItemDescription(index, location));
    }
    #endregion
}
