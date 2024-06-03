using Items;
using UnityEngine;

public class ResearchController : Singleton<ResearchController>
{
    #region Private fields
    [Header("Main")]
    [SerializeField]
    private Player _player;
    [SerializeField]
    private UIResearchPage _researchPage;
    private ResearchesSO _researchesData;
    private InventoryModelOld _inventoryData;
    private int _indexOfActiveResearch;

    #endregion

    #region Public fields

    #endregion

    #region Properties
    public bool IsActive => UIManager.Instance.ResearchUI.IsActive;

    public BookControllerType Type => BookControllerType.Research;
    #endregion

    #region Methods
    private void Awake()
    {
        if (_player is null)
        {
            _player = GetComponentInParent<Player>();
        }
        InitializeData();
        InitializeUI();
    }

    public void InitializeData()
    {
        //_inventoryData = _player.Inventory;
        _researchesData = GameManager.Instance.Researches;
        _researchesData.Initialize();
        _researchesData.OnResearchChangedState += HandleUpdateUIResearchState;
    }

    public void InitializeUI()
    {
        int researchesCount = _researchesData.GetResearchesCount();
        _researchPage = UIManager.Instance.ResearchUI.Content.GetComponentInChildren<UIResearchPage>();
        _researchPage.InitializePage(researchesCount);
        for (int i = 0; i < _researchesData.GetResearchesCount(); i++)
        {
            _researchPage.UpdateResearch(
                i,
                _researchesData.GetName(i),
                _researchesData.GetIconImage(i),
                _researchesData.GetPosition(i),
                _researchesData.GetState(i));
            foreach (ResearchSO parent in _researchesData.GetListOfPerents(i))
            {
                _researchPage.CreateResearchLine(i, _researchesData.GetPosition(i), _researchesData.GetPosition(parent));
            }
        }
        _researchPage.OnTryFinishResearch += HandleTryFinishResearch;
        _researchPage.OnResearchDescriptionRequested += HandleResearchDescriptionRequested;
        _researchPage.OnRewardDescriptionRequested += HandleRewardDescriptionRequested;
        _researchPage.OnCostDescriptionRequested += HandleCostDescriptionRequested;
    }

    public void ResetPresenter()
    {
        UIManager.Instance.ResearchUI.ToggleActive();
        _researchPage.ResetPage();
    }

    private void HandleTryFinishResearch(int index)
    {
        if (_researchesData.ExamineResearch(_inventoryData, index))
        {
            _researchPage.FinishResearch(index);
        }
    }

    private void HandleResearchDescriptionRequested(int index)
    {
        _indexOfActiveResearch = index;
        _researchPage.UpdateResearchDescription(_researchesData.GetName(index), _researchesData.GetDescription(index));
        foreach (var reward in _researchesData.GetListOfRewards(index))
        {
            _researchPage.AddRewardToResearchDescription(reward.Result.Item.SmallItemImage);
        }
        foreach (var cost in _researchesData.GetListOfCosts(index))
        {
            _researchPage.AddCostToResearchDescription(cost.Item.SmallItemImage, cost.Quantity);
        }
    }

    private void HandleRewardDescriptionRequested(int index)
    {
        ItemSO reward = _researchesData.GetListOfRewards(_indexOfActiveResearch)[index].Result.Item;
        _researchPage.UpdateItemDescription(reward.SmallItemImage, reward.ColoredName, reward.Description);
    }

    private void HandleCostDescriptionRequested(int index)
    {
        ItemSO cost = _researchesData.GetListOfCosts(_indexOfActiveResearch)[index].Item;
        _researchPage.UpdateItemDescription(cost.SmallItemImage, cost.ColoredName, cost.Description);
    }

    private void HandleUpdateUIResearchState(int index, ResearchState state)
    {
        _researchPage.UpdateResearchState(index, state);
    }
    #endregion
}
