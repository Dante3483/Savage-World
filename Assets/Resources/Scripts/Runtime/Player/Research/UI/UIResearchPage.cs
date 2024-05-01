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
    private List<UIResearchNode> _listOfResearches = new();
    
    #endregion

    #region Public fields
    public event Action<int> OnTryFinishResearch, OnResearchDescriptionRequested, OnRewardDescriptionRequested, OnCostDescriptionRequested; 
    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        
    }

    public void InitializePage(int countOfResearches)
    {
        for (int i = 0; i < countOfResearches; i++)
        {
            UIResearchNode uiItem = Instantiate(_researchPrefab, Vector3.zero, Quaternion.identity);
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

    public void FinishResearch(int index)
    {
        _listOfResearches[index].Finish();
    }

    private void HideResearchDescription()
    {
        _researchDescription.gameObject.SetActive(false);
    }

    public void UpdateResearchDescription(string name, string description)
    {
        _researchDescription.gameObject.SetActive(true);
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

    public void UpdateItemDescription(Sprite image, String name, string description)
    {
        _itemDescription.gameObject.SetActive(true);
        _itemDescription.SetData(image, name, description);
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
        OnResearchDescriptionRequested?.Invoke(index);
        _researchDescription.transform.SetParent(research.transform);
        _itemDescription.transform.SetParent(research.transform);
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
