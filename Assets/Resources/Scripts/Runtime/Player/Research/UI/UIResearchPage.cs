using System;
using System.Collections.Generic;
using UnityEngine;

public class UIResearchPage : MonoBehaviour
{
    #region Private fields
    [SerializeField]
    private UIResearchNode _researchPrefab;
    [SerializeField]
    private UIResearchDescription _researchDescription;
    [SerializeField]
    private UIItemDescription _itemDescription;
    [SerializeField]
    private RectTransform _researchesContent;
    private UIScaler _uiScaler;
    private List<UIResearchNode> _listOfResearches = new();
    private UIResearchNode _currentResearch;
    #endregion

    #region Public fields
    public event Action<int> OnTryFinishResearch, OnResearchDescriptionRequested, OnRewardDescriptionRequested, OnCostDescriptionRequested; 
    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        _uiScaler = GetComponent<UIScaler>();
    }

    public void InitializePage(int countOfResearches)
    {
        for (int i = 0; i < countOfResearches; i++)
        {
            UIResearchNode uiItem = Instantiate(_researchPrefab, new Vector3(0+300*i,0+300*i,0), Quaternion.identity);
            uiItem.transform.SetParent(_researchesContent, false);
            uiItem.name = "Research";
            uiItem.OnFinishResearch += HandleFinishResearch;
            uiItem.OnMouseEnter += HandleUpdateResearchDescription;
            uiItem.OnMouseLeave += HideResearchDescription;
            _listOfResearches.Add(uiItem);
        }
        _researchDescription.OnRewardDescriptionRequested += HandleRewardDescriptionRequested;
        _researchDescription.OnCostDescriptionRequested += HandleCostDescriptionRequested;
        _researchDescription.OnHideItemDescription += HideItemDescription;
    }

    public void UpdateResearch(int index, string name, Sprite image)
    {
        UIResearchNode research = _listOfResearches[index];
        research.SetData(name, image);
    }

    public void FinishResearch(int index)
    {
        _listOfResearches[index].Finish();
    }

    private void ShowResearchDescription()
    {
        _researchDescription.gameObject.SetActive(true);
    }

    public void HideResearchDescription()
    {
        _researchDescription.gameObject.SetActive(false);
    }

    public void UpdateResearchDescriptionPosition(float scale)
    {
        if (_currentResearch == null)
        {
            return;
        }
        Vector3 position = _currentResearch.transform.position;
        Vector2 size = (_currentResearch.transform as RectTransform).sizeDelta * scale;
        _researchDescription.transform.SetParent(_currentResearch.transform, false);
        _researchDescription.transform.position = new Vector3(position.x + size.x / 2f, position.y + size.y / 2f);
    }

    public void UpdateResearchDescription(string name, string description)
    {
        ShowResearchDescription();
        UpdateResearchDescriptionPosition(_uiScaler.Scale);
        _researchDescription.UpdateName(name);
        _researchDescription.UpdateDescriptionText(description);
    }

    public void AddRewardToResearchDescription(Sprite image)
    {
        _researchDescription.AddReward(image);
    }

    public void AddCostToResearchDescription(Sprite image, int quantity)
    {
        _researchDescription.AddCost(image, quantity);
    }

    public void UpdateItemDescription(Sprite image, string name, string description)
    {
        ShowItemDescription();
        _itemDescription.SetData(image, name, description);
    }

    private void ShowItemDescription()
    {
        _itemDescription.gameObject.SetActive(true);
        //_itemDescription.transform.SetParent(research.transform);
    }

    private void HideItemDescription()
    {
        _itemDescription.gameObject.SetActive(false);
    }

    public void ResetPage()
    {
        HideItemDescription();
        HideResearchDescription();
    }

    private void HandleFinishResearch(UIResearchNode research)
    {
        int index = _listOfResearches.IndexOf(research);
        OnTryFinishResearch?.Invoke(index);
    }

    private void HandleUpdateResearchDescription(UIResearchNode research)
    {
        int index = _listOfResearches.IndexOf(research);
        _currentResearch = research;
        OnResearchDescriptionRequested?.Invoke(index);
    }

    private void HandleRewardDescriptionRequested(int index)
    {
        OnRewardDescriptionRequested?.Invoke(index);
    }

    private void HandleCostDescriptionRequested(int index)
    {
        OnCostDescriptionRequested?.Invoke(index);
    } 
    #endregion
}
