using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct RaycastUtil
{
    #region Private fields
    [SerializeField] private Vector3 _originOffset;
    [SerializeField] private Vector2 _direction;
    [SerializeField] private float _distance;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Color _hitColor;
    [SerializeField] private Color _notHitColor;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public Vector3 OriginOffset
    {
        get
        {
            return _originOffset;
        }

        set
        {
            _originOffset = value;
        }
    }

    public Vector2 Direction
    {
        get
        {
            return _direction;
        }

        set
        {
            _direction = value;
        }
    }

    public float Distance
    {
        get
        {
            return _distance;
        }

        set
        {
            _distance = value;
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
    #endregion

    #region Methods
    public RaycastHit2D Raycast(Vector3 origin, out bool result, bool needVizualize = true)
    {
        Color rayColor;
        origin += OriginOffset;

        RaycastHit2D hit = Physics2D.Raycast(origin, Direction, Distance, LayerMask);
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
            Debug.DrawRay(origin, Direction * Distance, rayColor);
        }

        return hit;
    }

    public RaycastHit2D Raycast(Vector3 origin, bool needVizualize = true)
    {
        Color rayColor;
        origin += OriginOffset;

        RaycastHit2D hit = Physics2D.Raycast(origin, Direction, Distance, LayerMask);
        if (hit)
        {
            rayColor = HitColor;
        }
        else
        {
            rayColor = NotHitColor;
        }
        if (needVizualize)
        {
            Debug.DrawRay(origin, Direction * Distance, rayColor);
        }

        return hit;
    }

    public RaycastHit2D Raycast(Vector3 origin, Vector2 direction, float distance, LayerMask layerMask, Color hitColor, Color notHitColor, out bool result, bool needVizualize = true)
    {
        Color rayColor;
        origin += OriginOffset;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, layerMask);
        if (hit)
        {
            result = true;
            rayColor = hitColor;
        }
        else
        {
            result = false;
            rayColor = notHitColor;
        }

        if (needVizualize)
        {
            Debug.DrawRay(origin, direction * distance, rayColor);
        }

        return hit;
    }

    public RaycastHit2D Raycast(Vector3 origin, Vector2 direction, float distance, LayerMask layerMask, Color hitColor, Color notHitColor, bool needVizualize = true)
    {
        Color rayColor;
        origin += OriginOffset;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, layerMask);
        if (hit)
        {
            rayColor = hitColor;
        }
        else
        {
            rayColor = notHitColor;
        }

        if (needVizualize)
        {
            Debug.DrawRay(origin, direction * distance, rayColor);
        }

        return hit;
    }
    #endregion
}
