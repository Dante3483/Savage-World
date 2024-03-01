using CustomTilemap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour
{
    #region Private fields
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Vector3Int _intPosition;
    [SerializeField] private Vector2 _direction;
    [SerializeField] private Vector2 _colliderSize;
    [SerializeField] private float _speed;
    [SerializeField] private float _gravity = 30;
    [SerializeField] private float _xVelocity;
    [SerializeField] private float _yVelocity;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _colliderSize = GetComponent<BoxCollider2D>().size / 2;
    }

    private void FixedUpdate()
    {
        _intPosition = Vector3Int.FloorToInt(transform.position);
        Fall();
    }

    private void Fall()
    {
        if (transform.position.y == _intPosition.y + _colliderSize.y)
        {
            _yVelocity = 0;
            _xVelocity = 0;
        }
        _xVelocity = Mathf.Lerp(_xVelocity, 0, Time.fixedDeltaTime);
        _yVelocity = Mathf.Lerp(_yVelocity, -_gravity, Time.fixedDeltaTime);
        Vector3 newPosition = transform.position + new Vector3(_xVelocity, _yVelocity) * Time.fixedDeltaTime;
        if (_xVelocity > 0 && WorldDataManager.Instance.GetAdjacentWorldCellData(_intPosition.x, _intPosition.y, Vector2Int.right).IsSolid())
        {
            newPosition.x = Mathf.Min(newPosition.x, (_intPosition.x + 1) - _colliderSize.x);
        }
        if (_xVelocity < 0 && WorldDataManager.Instance.GetAdjacentWorldCellData(_intPosition.x, _intPosition.y, Vector2Int.left).IsSolid())
        {
            newPosition.x = Mathf.Max(newPosition.x, _intPosition.x + _colliderSize.x);
        }
        if (WorldDataManager.Instance.GetAdjacentWorldCellData(_intPosition.x, _intPosition.y, Vector2Int.down).IsSolid())
        {
            newPosition.y = Mathf.Max(newPosition.y, _intPosition.y + _colliderSize.y);
        }
        if (WorldDataManager.Instance.GetWorldCellData(_intPosition.x, _intPosition.y).IsSolid())
        {
            newPosition.y = _intPosition.y + 1;
        }
        _rigidbody.MovePosition(newPosition);
    }

    public void AddForce()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);

        _direction = Input.mousePosition - screenPoint;

        _direction.Normalize();

        _xVelocity = _direction.x * _speed;
        _yVelocity = _direction.y * _speed;
    }
    #endregion
}
