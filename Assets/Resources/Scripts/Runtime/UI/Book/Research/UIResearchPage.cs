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
    public event Action<int> OnTryFinishResearch; 
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
            _listOfResearches.Add(uiItem);
        }
    }
    public void FinishResearch(int index)
    {
        _listOfResearches[index].Finish();
    }

    private void HandleFinishResearch(UIResearchNode research)
    {
        int index = _listOfResearches.IndexOf(research);
        OnTryFinishResearch?.Invoke(index);
    }
    
    #endregion
}
