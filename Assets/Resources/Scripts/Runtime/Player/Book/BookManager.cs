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

    [Header("Hotbar Configuration")]
    [SerializeField]
    private HotbarView _hotbarView;
    private HotbarPresenter _hotbarPresenter;

    [Header("Craft Station Configuration")]
    [SerializeField]
    private CraftStationView _craftStationView;
    private CraftStationPresenter _craftStationPresenter;

    [Header("Researches Configuration")]
    [SerializeField]
    private ResearchesView _researchesView;
    private ResearchesPresenter _researchesPresenter;

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

    public CraftStationPresenter CraftStationPresenter
    {
        get
        {
            return _craftStationPresenter;
        }
    }

    public ResearchesPresenter ResearchesPresenter
    {
        get
        {
            return _researchesPresenter;
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
        if (_inventoryPresenter is null)
        {
            _inventoryPresenter = new(_minTimeToTakeItem, _maxTimeToTakeItem, _stepToLerpTimeToTakeItem, model, _inventoryView);
            _listOfPresenters.Add(_inventoryPresenter);
        }
    }

    public void InitializeHotbar(InventoryModel model)
    {
        if (_hotbarPresenter is null)
        {
            _hotbarPresenter = new(model, _hotbarView);
            _hotbarPresenter.Enable();
        }
    }

    public void InitializeCraftStation(CraftStationModelSO model, InventoryModel inventory)
    {
        if (_craftStationPresenter is null)
        {
            _craftStationPresenter = new(model, _craftStationView, inventory);
            _listOfPresenters.Add(_craftStationPresenter);
        }
    }

    public void InitializeResearches(ResearchesModelSO model, InventoryModel inventory)
    {
        if (_researchesPresenter is null)
        {
            _researchesPresenter = new(model, _researchesView, inventory);
            _listOfPresenters.Add(_researchesPresenter);
        }
    }

    public bool TogglePresenter(PresenterBase presenter)
    {
        PresenterBase currentActivePresenter = _listOfPresenters.FirstOrDefault(p => p.IsAvtive);
        if (currentActivePresenter == presenter)
        {
            presenter.Disable();
            EventManager.OnBookClosed();
            return false;
        }
        else
        {
            currentActivePresenter?.Disable();
            presenter.Enable();
            EventManager.OnBookOpened();
            return true;
        }
    }
    #endregion

    #region Private Methods

    #endregion
}
