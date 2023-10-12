using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using static UnityEngine.UI.Image;

[Serializable]
public struct LinecastUtil
{
    #region Private fields
    [SerializeField] private Color _hitColor;
    [SerializeField] private Color _notHitColor;
    [SerializeField] private Vector3 _vFrom;
    [SerializeField] private Vector3 _vTo;
    [SerializeField] private LayerMask _layerMask;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public Color HitColor
    {
        get
        {
            return _hitColor;
        }

        set
        {
            _hitColor = value;
        }
    }

    public Color NotHitColor
    {
        get
        {
            return _notHitColor;
        }

        set
        {
            _notHitColor = value;
        }
    }

    public Vector3 VFrom
    {
        get
        {
            return _vFrom;
        }

        set
        {
            _vFrom = value;
        }
    }

    public Vector3 VTo
    {
        get
        {
            return _vTo;
        }

        set
        {
            _vTo = value;
        }
    }

    public LayerMask LayerMask
    {
        get
        {
            return _layerMask;
        }

        set
        {
            _layerMask = value;
        }
    }
    #endregion

    #region Methods
    public RaycastHit2D CheckLinecast(Vector3 _vFrom, Vector3 _vTo,  out bool result, bool needVizualize = true)
    {
        Color rayColor;

        RaycastHit2D hit = Physics2D.Linecast(VFrom, VTo, LayerMask);
        if (hit)
        {
            result = true;
            rayColor = HitColor;
        }
        else
        {
            result = false;
            rayColor = NotHitColor;
        }

        if (needVizualize)
        {
            Debug.DrawLine(VFrom, VTo, rayColor);
        }

        return hit;
    }
    #endregion
}
