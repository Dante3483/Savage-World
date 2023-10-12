using System;
using UnityEngine;

[Serializable]
public struct BoxCastUtil
{
    #region Private fields
    [SerializeField] private Vector3 _originOffset;
    [Min(0.001f)]
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
            Vector2 leftBottom = origin - new Vector3(halfSize.x, halfSize.y);
            Vector2 leftTop = origin - new Vector3(halfSize.x, -halfSize.y);
            Vector2 rightBottom = origin + new Vector3(halfSize.x, -halfSize.y);
            Vector2 rightTop = origin + new Vector3(halfSize.x, halfSize.y);

            if (Direction.x < 0)
            {
                leftBottom.x -= Distance;
                leftTop.x -= Distance;
            }
            if (Direction.x > 0)
            {
                rightBottom.x += Distance;
                rightTop.x += Distance;
            }
            if (Direction.y < 0)
            {
                leftBottom.y -= Distance;
                rightBottom.y -= Distance;
            }
            if (Direction.y > 0)
            {
                leftTop.y += Distance;
                rightTop.y += Distance;
            }

            Debug.DrawLine(leftBottom, leftTop, rayColor);
            Debug.DrawLine(leftTop, rightTop, rayColor);
            Debug.DrawLine(rightTop, rightBottom, rayColor);
            Debug.DrawLine(rightBottom, leftBottom, rayColor);
        }

        return hit;
    }

    public RaycastHit2D BoxCast(Vector3 origin, bool needVizualize = true)
    {
        Color rayColor;
        origin += OriginOffset;

        RaycastHit2D hit = Physics2D.BoxCast(origin, Size, Angle, Direction, Distance, LayerMask);
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
            Vector2 halfSize = Size / 2f;
            Vector2 leftBottom = origin - new Vector3(halfSize.x, halfSize.y);
            Vector2 leftTop = origin - new Vector3(halfSize.x, -halfSize.y);
            Vector2 rightBottom = origin + new Vector3(halfSize.x, -halfSize.y);
            Vector2 rightTop = origin + new Vector3(halfSize.x, halfSize.y);

            if (Direction.x < 0)
            {
                leftBottom.x -= Distance;
                leftTop.x -= Distance;
            }
            if (Direction.x > 0)
            {
                rightBottom.x += Distance;
                rightTop.x += Distance;
            }
            if (Direction.y < 0)
            {
                leftBottom.y -= Distance;
                rightBottom.y -= Distance;
            }
            if (Direction.y > 0)
            {
                leftTop.y += Distance;
                rightTop.y += Distance;
            }

            Debug.DrawLine(leftBottom, leftTop, rayColor);
            Debug.DrawLine(leftTop, rightTop, rayColor);
            Debug.DrawLine(rightTop, rightBottom, rayColor);
            Debug.DrawLine(rightBottom, leftBottom, rayColor);
        }

        return hit;
    }

    public RaycastHit2D BoxCast(Vector3 origin, Vector2 size, float angle, Vector2 direction, float distance, LayerMask layerMask, Color hitColor, Color notHitColor, out bool result, bool needVizualize = true)
    {
        Color rayColor;
        origin += OriginOffset;

        RaycastHit2D hit = Physics2D.BoxCast(origin, size, angle, direction, distance, layerMask);
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
            Vector2 halfSize = size / 2f;
            Vector2 leftBottom = origin - new Vector3(halfSize.x, halfSize.y);
            Vector2 leftTop = origin - new Vector3(halfSize.x, -halfSize.y);
            Vector2 rightBottom = origin + new Vector3(halfSize.x, -halfSize.y);
            Vector2 rightTop = origin + new Vector3(halfSize.x, halfSize.y);

            if (direction.x < 0)
            {
                leftBottom.x -= distance;
                leftTop.x -= distance;
            }
            if (direction.x > 0)
            {
                rightBottom.x += distance;
                rightTop.x += distance;
            }
            if (direction.y < 0)
            {
                leftBottom.y -= distance;
                rightBottom.y -= distance;
            }
            if (direction.y > 0)
            {
                leftTop.y += distance;
                rightTop.y += distance;
            }

            Debug.DrawLine(leftBottom, leftTop, rayColor);
            Debug.DrawLine(leftTop, rightTop, rayColor);
            Debug.DrawLine(rightTop, rightBottom, rayColor);
            Debug.DrawLine(rightBottom, leftBottom, rayColor);
        }

        return hit;
    }

    public RaycastHit2D BoxCast(Vector3 origin, Vector2 size, float angle, Vector2 direction, float distance, LayerMask layerMask, Color hitColor, Color notHitColor, bool needVizualize = true)
    {
        Color rayColor;
        origin += OriginOffset;

        RaycastHit2D hit = Physics2D.BoxCast(origin, size, angle, direction, distance, layerMask);
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
            Vector2 halfSize = size / 2f;
            Vector2 leftBottom = origin - new Vector3(halfSize.x, halfSize.y);
            Vector2 leftTop = origin - new Vector3(halfSize.x, -halfSize.y);
            Vector2 rightBottom = origin + new Vector3(halfSize.x, -halfSize.y);
            Vector2 rightTop = origin + new Vector3(halfSize.x, halfSize.y);

            if (direction.x < 0)
            {
                leftBottom.x -= distance;
                leftTop.x -= distance;
            }
            if (direction.x > 0)
            {
                rightBottom.x += distance;
                rightTop.x += distance;
            }
            if (direction.y < 0)
            {
                leftBottom.y -= distance;
                rightBottom.y -= distance;
            }
            if (direction.y > 0)
            {
                leftTop.y += distance;
                rightTop.y += distance;
            }

            Debug.DrawLine(leftBottom, leftTop, rayColor);
            Debug.DrawLine(leftTop, rightTop, rayColor);
            Debug.DrawLine(rightTop, rightBottom, rayColor);
            Debug.DrawLine(rightBottom, leftBottom, rayColor);
        }

        return hit;
    }
    public RaycastHit2D BoxCast(Vector3 origin, GameObject self, Color hitColor, Color notHitColor, bool needVizualize = true)
    {
        Color rayColor;
        origin += OriginOffset;

        RaycastHit2D hit = Physics2D.BoxCast(origin, Size, Angle, Direction, Distance, LayerMask);

        bool result = hit.collider != null && hit.collider.gameObject != self;
        if (result)
        {
            rayColor = hitColor;
        }
        else
        {
            rayColor = notHitColor;
        }
        if (needVizualize)
        {
            Vector2 halfSize = Size / 2f;
            Vector2 leftBottom = origin - new Vector3(halfSize.x, halfSize.y);
            Vector2 leftTop = origin - new Vector3(halfSize.x, -halfSize.y);
            Vector2 rightBottom = origin + new Vector3(halfSize.x, -halfSize.y);
            Vector2 rightTop = origin + new Vector3(halfSize.x, halfSize.y);

            if (Direction.x < 0)
            {
                leftBottom.x -= Distance;
                leftTop.x -= Distance;
            }
            if (Direction.x > 0)
            {
                rightBottom.x += Distance;
                rightTop.x += Distance;
            }
            if (Direction.y < 0)
            {
                leftBottom.y -= Distance;
                rightBottom.y -= Distance;
            }
            if (Direction.y > 0)
            {
                leftTop.y += Distance;
                rightTop.y += Distance;
            }

            Debug.DrawLine(leftBottom, leftTop, rayColor);
            Debug.DrawLine(leftTop, rightTop, rayColor);
            Debug.DrawLine(rightTop, rightBottom, rayColor);
            Debug.DrawLine(rightBottom, leftBottom, rayColor);
        }

        return hit;
    }
    #endregion
}
