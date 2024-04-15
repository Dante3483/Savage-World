using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchController : MonoBehaviour, IBookPageController
{
    #region Private fields

    #endregion

    #region Public fields
    public bool IsActive => throw new System.NotImplementedException();
    #endregion

    #region Properties

    #endregion

    #region Methods
    public void PrepareData()
    {
        throw new System.NotImplementedException();
    }

    public void PrepareUI()
    {
        throw new System.NotImplementedException();
    }

    public void ResetData()
    {
        UIManager.Instance.ResearchUI.ReverseActivity();
    }
    #endregion
}
