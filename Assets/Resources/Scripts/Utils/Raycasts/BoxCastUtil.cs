using System;
using UnityEngine;

[Serializable]
public struct BoxCastUtil
{
    #region Private fields
    [SerializeField] private Vector3 _originOffset;
    [SerializeField] private Vector2 _size;
    [SerializeField] private float _angle;
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

    public Vector2 Size
    {
        get
        {
            return _size;
        }

        set
        {
            _size = value;
        }
    }

    public float Angle
    {
        get
        {
            return _angle;
        }

        set
        {
            _angle = value;
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
    public RaycastHit2D BoxCast(Vector3 origin, out bool result, bool needVizualize = true)
    {
        Color rayColor;
        origin += OriginOffset;

        RaycastHit2D hit = Physics2D.BoxCast(origin, Size, Angle, Direction, Distance, LayerMask);
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
            Vector2 halfSize = Size / 2f;
            Vector2 originForDebug = new Vector2(origin.x, origin.y + halfSize.y);

            Debug.DrawRay(originForDebug + new Vector2(halfSize.x, 0), Vector2.down * (Size.y + Distance), rayColor);
            Debug.DrawRay(originForDebug - new Vector2(halfSize.x, 0), Vector2.down * (Size.y + Distance), rayColor);
            Debug.DrawRay(originForDebug - new Vector2(halfSize.x, Size.y + Distance), Vector2.right * Size.x, rayColor);
            Debug.DrawRay(originForDebug - new Vector2(halfSize.x, 0), Vector2.right * Size.x, rayColor);
        }

        return hit;
    }
    #endregion
}
