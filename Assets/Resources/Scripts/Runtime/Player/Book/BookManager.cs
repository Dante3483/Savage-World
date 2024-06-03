using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BookManager : Singleton<BookManager>
{
    #region Fields
    [Header("Inventory Configuration")]
    [SerializeField]
    private InventoryView _inventoryView;
    [SerializeField]
    private float _minTimeToTakeItem;
    [SerializeField]
    private float _maxTimeToTakeItem;
    [SerializeField]
    private float _stepToLerpTimeToTakeItem;
    private InventoryPresenter _inventoryPresenter;

    [SerializeField]
    private HotbarView _hotbarView;
    private HotbarPresenter _hotbarPresenter;

    private List<PresenterBase> _listOfPresenters;
    #endregion

    #region Properties
    public InventoryPresenter InventoryPresenter
    {
        get
        {
            return _inventoryPresenter;
        }
    }
    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods
    private void Start()
    {
        _listOfPresenters = new();
    }
    #endregion

    #region Public Methods
    public void InitializeInventory(InventoryModel model)
    {
        _inventoryPresenter = new(_minTimeToTakeItem, _maxTimeToTakeItem, _stepToLerpTimeToTakeItem, model, _inventoryView);
        _listOfPresenters.Add(_inventoryPresenter);
    }

    public void InitializeHotbar(InventoryModel model)
    {
        _hotbarPresenter = new(model, _hotbarView);
        _hotbarPresenter.Enable();
    }

    public void TogglePresenter(PresenterBase presenter)
    {
        PresenterBase currentActivePresenter = _listOfPresenters.FirstOrDefault(p => p.IsAvtive);
        if (currentActivePresenter == presenter)
        {
            presenter.Disable();
        }
        else
        {
            currentActivePresenter?.Disable();
            presenter.Enable();
        }
    }
    #endregion

    #region Private Methods

    #endregion
}
