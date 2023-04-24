using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CheckingLineCast
{
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private Color _isTrueColor;
    [SerializeField] private Color _isFalseColor;

    public LayerMask TargetLayer
    {
        get
        {
            return _targetLayer;
        }

        set
        {
            _targetLayer = value;
        }
    }

    public Color IsTrueColor
    {
        get
        {
            return _isTrueColor;
        }

        set
        {
            _isTrueColor = value;
        }
    }

    public Color IsFalseColor
    {
        get
        {
            return _isFalseColor;
        }

        set
        {
            _isFalseColor = value;
        }
    }

    public Tuple<bool, Collider2D> CheckLinecast(Vector3 start, Vector3 end, GameObject self = null)
    {
        RaycastHit2D raycastHit = Physics2D.Linecast(start, end, TargetLayer);

        bool boolResult = raycastHit.collider != null && raycastHit.collider.gameObject != self;
        Color color;
        if (boolResult)
        {
            color = IsTrueColor;
        }
        else
        {
            color = IsFalseColor;
        }

        Debug.DrawLine(start, end, color);

        return new Tuple<bool, Collider2D>(boolResult, raycastHit.collider);
    }
}
