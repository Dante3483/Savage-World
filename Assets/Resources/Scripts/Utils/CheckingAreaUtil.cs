using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CheckingAreaUtil
{
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private Vector2 _CheckSize;
    [SerializeField] private Vector2 _CheckCenterOffset;
    [SerializeField] private float _extraWidth;
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

    public Vector2 CheckSize
    {
        get
        {
            return _CheckSize;
        }

        set
        {
            _CheckSize = value;
        }
    }

    public Vector2 CheckCenterOffset
    {
        get
        {
            return _CheckCenterOffset;
        }

        set
        {
            _CheckCenterOffset = value;
        }
    }

    public float ExtraWidth
    {
        get
        {
            return _extraWidth;
        }

        set
        {
            _extraWidth = value;
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

    public Tuple<bool, Collider2D> CheckArea(Vector2 center, GameObject self)
    {
        center += CheckCenterOffset;
        RaycastHit2D raycastHit = Physics2D.BoxCast(center, CheckSize, 0f, Vector2.down, ExtraWidth, TargetLayer);

        Color rayColor;
        bool result = raycastHit.collider != null && raycastHit.collider.gameObject != self;
        if (result)
        {
            rayColor = IsTrueColor;
        }
        else
        {
            rayColor = IsFalseColor;
        }

        Vector2 halfSize = CheckSize / 2f;
        Vector2 centerForDebug = new Vector2(center.x, center.y + halfSize.y);

        Debug.DrawRay(centerForDebug + new Vector2(halfSize.x, 0), Vector2.down * (CheckSize.y + ExtraWidth), rayColor);
        Debug.DrawRay(centerForDebug - new Vector2(halfSize.x, 0), Vector2.down * (CheckSize.y + ExtraWidth), rayColor);
        Debug.DrawRay(centerForDebug - new Vector2(halfSize.x, CheckSize.y + ExtraWidth), Vector2.right * CheckSize.x, rayColor);
        Debug.DrawRay(centerForDebug - new Vector2(halfSize.x, 0), Vector2.right * CheckSize.x, rayColor);

        return new Tuple<bool, Collider2D>(result, raycastHit.collider);
    }
}
