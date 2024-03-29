using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPhasesInfo : UIDebug
{
    #region Private fields

    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public void Update()
    {
        _debugText.text = GameManager.Instance.PhasesInfo;
    }
    #endregion
}
