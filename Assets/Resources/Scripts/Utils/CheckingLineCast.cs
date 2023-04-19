using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CheckingLineCast
{
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private Color _isTrueColor;
    [SerializeField] private Color _isFalseColor;

    public Tuple<bool, Collider2D> CheckLinecast(Vector3 start, Vector3 end, GameObject self = null)
    {
        RaycastHit2D raycastHit = Physics2D.Linecast(start, end, _targetLayer);

        bool boolResult = raycastHit.collider != null && raycastHit.collider.gameObject != self;
        Color color;
        if (boolResult)
        {
            color = _isTrueColor;
        }
        else
        {
            color = _isFalseColor;
        }

        Debug.DrawLine(start, end, color);

        return new Tuple<bool, Collider2D>(boolResult, raycastHit.collider);
    }
}
