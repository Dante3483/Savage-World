using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

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

    public Tuple<bool, Collider2D> CheckWithMinDistance(Vector2 center, GameObject self, Vector3 startPosition, float distance)
    {
        center += CheckCenterOffset;
        RaycastHit2D[] raycastHit = Physics2D.BoxCastAll(center, CheckSize, 0f, Vector2.down, ExtraWidth, TargetLayer);

        Color rayColor;
        bool result = raycastHit.Length != 0;
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

        if (raycastHit.Length != 0)
        {
            List<RaycastHit2D> fixedRaycasts = new List<RaycastHit2D>();
            foreach (var hit in raycastHit)
            {
                if (hit.collider.gameObject == self)
                {
                    continue;
                }
                if (Math.Abs(startPosition.x - hit.transform.position.x) <= distance)
                {
                    fixedRaycasts.Add(hit);
                }
            }
            float minDistance = Math.Abs(center.x - raycastHit[0].transform.position.x);
            Collider2D resultCollider = raycastHit[0].collider;
            foreach (var hit in fixedRaycasts)
            {
                if (hit.collider.gameObject == self)
                {
                    continue;
                }
                if (Math.Abs(center.x - hit.transform.position.x) < minDistance)
                {
                    minDistance = Math.Abs(center.x - hit.transform.position.x);
                    resultCollider = hit.collider;
                }
            }
            return new Tuple<bool, Collider2D>(result, resultCollider);
        }
        else
        {
            return new Tuple<bool, Collider2D>(result, null);
        }
    }
}
