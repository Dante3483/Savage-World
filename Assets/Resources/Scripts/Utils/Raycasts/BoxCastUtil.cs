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
    [SerializeField] private Vector3 _originOffset;
    [Min(0.001f)]
    [SerializeField] private Vector2 _size;
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
    public bool Result
    {
        get
        {
            return _result;
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

    public GameObject Self
    {
        set
        {
            _self = value;
        }
    }
    #endregion

    #region Methods
    public RaycastHit2D BoxCast(Vector3 origin)
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

        DrawBox(origin, rayColor);
        return hit;
    }

    public RaycastHit2D[] BoxCastAll(Vector3 origin)
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

        DrawBox(origin, rayColor);
        return hits;
    }

    private void DrawBox(Vector3 origin, Color rayColor)
    {
        if (!_needVisualize)
        {
            return;
        }
        Vector2 halfSize = _size / 2f;
        Vector2 leftBottom = origin - new Vector3(halfSize.x, halfSize.y);
        Vector2 leftTop = origin - new Vector3(halfSize.x, -halfSize.y);
        Vector2 rightBottom = origin + new Vector3(halfSize.x, -halfSize.y);
        Vector2 rightTop = origin + new Vector3(halfSize.x, halfSize.y);

        if (_direction.x < 0)
        {
            leftBottom.x -= _distance;
            leftTop.x -= _distance;
        }
        if (_direction.x > 0)
        {
            rightBottom.x += _distance;
            rightTop.x += _distance;
        }
        if (_direction.y < 0)
        {
            leftBottom.y -= _distance;
            rightBottom.y -= _distance;
        }
        if (_direction.y > 0)
        {
            leftTop.y += _distance;
            rightTop.y += _distance;
        }

        Debug.DrawLine(leftBottom, leftTop, rayColor);
        Debug.DrawLine(leftTop, rightTop, rayColor);
        Debug.DrawLine(rightTop, rightBottom, rayColor);
        Debug.DrawLine(rightBottom, leftBottom, rayColor);
    }
    #endregion
}
