using System;
using System.Linq;
using UnityEngine;

[Serializable]
public struct BoxCastUtil
{
    #region Private fields
    [SerializeField] private bool _needVisualize;
    [SerializeField] private bool _checkYourself;
    [SerializeField] private bool _result;
    [SerializeField] private Vector2 _originOffset;
    [Min(0.001f)][SerializeField] private Vector2 _size;
    [SerializeField] private float _angle;
    [SerializeField] private Vector2 _direction;
    [SerializeField] private float _distance;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Color _hitColor;
    [SerializeField] private Color _notHitColor;
    private GameObject _self;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public bool NeedVisualize
    {
        set
        {
            _needVisualize = value;
        }
    }

    public bool CheckYourself
    {
        set
        {
            _checkYourself = value;
        }
    }

    public bool Result
    {
        get
        {
            return _result;
        }
    }

    public Vector2 OriginOffset
    {
        set
        {
            _originOffset = value;
        }
    }

    public Vector2 Size
    {
        set
        {
            _size = value;
        }
    }

    public float Angle
    {
        set
        {
            _angle = value;
        }
    }

    public Vector2 Direction
    {
        set
        {
            _direction = value;
        }
    }

    public float Distance
    {
        set
        {
            _distance = value;
        }
    }

    public LayerMask LayerMask
    {
        set
        {
            _layerMask = value;
        }
    }

    public Color HitColor
    {
        set
        {
            _hitColor = value;
        }
    }

    public Color NotHitColor
    {
        set
        {
            _notHitColor = value;
        }
    }

    public GameObject Self
    {
        set
        {
            _self = value;
        }
    }
    #endregion

    #region Methods
    public RaycastHit2D BoxCast(Vector2 origin)
    {
        Color rayColor;
        origin += _originOffset;
        RaycastHit2D hit = Physics2D.BoxCast(origin, _size, _angle, _direction, _distance, _layerMask);

        _result = hit;
        if (_checkYourself && _self != null)
        {
            _result = _result && hit.collider.gameObject != _self;
        }
        rayColor = _result ? _hitColor : _notHitColor;

        if (_needVisualize)
        {
            DebugUtil.DrawBox(origin, _size, _direction, _distance, rayColor);
        }
        return hit;
    }

    public RaycastHit2D[] BoxCastAll(Vector2 origin)
    {
        Color rayColor;
        origin += _originOffset;
        RaycastHit2D[] hits = Physics2D.BoxCastAll(origin, _size, _angle, _direction, _distance, _layerMask);

        if (_checkYourself && _self != null)
        {
            _result = hits.Length > 1;
        }
        else
        {
            _result = hits.Length != 0;
        }
        rayColor = _result ? _hitColor : _notHitColor;

        if (_needVisualize)
        {
            DebugUtil.DrawBox(origin, _size, _direction, _distance, rayColor);
        }
        return hits;
    }
    #endregion
}
