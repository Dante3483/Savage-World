using System;
using UnityEngine;

[Serializable]
public struct LinecastUtil
{
    #region Private fields
    [SerializeField] private bool _needVisualize;
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

    public bool NeedVisualize
    {
        get
        {
            return _needVisualize;
        }

        set
        {
            _needVisualize = value;
        }
    }
    #endregion

    #region Methods
    public RaycastHit2D CheckLinecast(Vector3 vFrom, Vector3 vTo,  out bool result)
    {
        Color rayColor;

        RaycastHit2D hit = Physics2D.Linecast(vFrom, vTo, LayerMask);
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

        DrawLine(vFrom, vTo, rayColor);

        return hit;
    }

    private void DrawLine(Vector3 vFrom, Vector3 vTo, Color rayColor)
    {
        if (!_needVisualize)
        {
            return;
        }
        Debug.DrawLine(vFrom, vTo, rayColor);
    }
    #endregion
}
