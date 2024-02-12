using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebugger : MonoBehaviour
{
    #region Private fields
    [SerializeField] private bool _enableGroundCheckVizualization;
    [SerializeField] private bool _enableWallCheckVizualization;
    [SerializeField] private bool _enableSlopeCheckVizualization;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public bool EnableGroundCheckVizualization
    {
        get
        {
            return _enableGroundCheckVizualization;
        }

        set
        {
            _enableGroundCheckVizualization = value;
        }
    }

    public bool EnableSlopeCheckVizualization
    {
        get
        {
            return _enableSlopeCheckVizualization;
        }

        set
        {
            _enableSlopeCheckVizualization = value;
        }
    }

    public bool EnableWallCheckVizualization
    {
        get
        {
            return _enableWallCheckVizualization;
        }

        set
        {
            _enableWallCheckVizualization = value;
        }
    }
    #endregion

    #region Methods

    #endregion
}
