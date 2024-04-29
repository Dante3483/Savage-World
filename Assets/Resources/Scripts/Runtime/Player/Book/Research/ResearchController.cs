using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchController : MonoBehaviour, IBookPageController
{
    #region Private fields
    [SerializeField]
    private UIResearchPage _researchPage;
    [SerializeField]
    private ResearchesSO _researchData;
    private int _indexOfActiveResearch;
    
    #endregion

    #region Public fields
    public bool IsActive => UIManager.Instance.ResearchUI.IsActive;
    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        PrepareUI();
        PrepareData();
    }

    public void PrepareData()
    {
        
    }

    public void PrepareUI()
    {
        _researchPage.InitializePage(1);
        _researchPage.OnTryFinishResearch += HandleTryFinishResearch;
    }

    public void ResetData()
    {
        UIManager.Instance.ResearchUI.ReverseActivity();
    }

    private void HandleTryFinishResearch(int index)
    {
        if (_researchData.ExamineResearch(index))
        {
            _researchPage.FinishResearch(index);
        }
    }
    #endregion
}
