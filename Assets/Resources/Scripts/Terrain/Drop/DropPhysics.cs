using System.Collections;
using System.Linq;
using UnityEngine;

public class DropPhysics : MonoBehaviour
{
    #region Private fields
    [SerializeField] private Drop _drop;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Vector2 _colliderHalfSize;
    [SerializeField] private float _speed;
    [SerializeField] private float _gravity = 30;
    [SerializeField] private float _xVelocity;
    [SerializeField] private float _yVelocity;
    private Vector3Int _intPosition;
    private Vector3 _newPosition;
    bool _isBottomBlockSolid;
    bool _isTopBlockSolid;
    bool _isRightBlockSolid;
    bool _isLeftBlockSolid;
    bool _isCurrentBlockSolid;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        _drop = GetComponent<Drop>();
        _drop.OnColliderSizeChanged += () =>
        {
            _colliderHalfSize = _drop.BoxCollider.size / 2;
        };
    }

    private void FixedUpdate()
    {
        if (_drop.IsPhysicsEnabled)
        {
            Move();
        }
    }

    private void Move()
    {
        _intPosition = Vector3Int.FloorToInt(transform.position);
        _isBottomBlockSolid = WorldDataManager.Instance.GetAdjacentWorldCellData(_intPosition.x, _intPosition.y, Vector2Int.down).IsSolid();
        if (transform.position.y == _intPosition.y + _colliderHalfSize.y && _isBottomBlockSolid)
        {
            _xVelocity = 0;
            _yVelocity = 0;
            return;
        }

        _isTopBlockSolid = WorldDataManager.Instance.GetAdjacentWorldCellData(_intPosition.x, _intPosition.y, Vector2Int.up).IsSolid();
        _isRightBlockSolid = WorldDataManager.Instance.GetAdjacentWorldCellData(_intPosition.x, _intPosition.y, Vector2Int.right).IsSolid();
        _isLeftBlockSolid = WorldDataManager.Instance.GetAdjacentWorldCellData(_intPosition.x, _intPosition.y, Vector2Int.left).IsSolid();
        _isCurrentBlockSolid = WorldDataManager.Instance.GetWorldCellData(_intPosition.x, _intPosition.y).IsSolid();

        _xVelocity = Mathf.Lerp(_xVelocity, 0, Time.fixedDeltaTime);
        _yVelocity = Mathf.Lerp(_yVelocity, -_gravity, Time.fixedDeltaTime);
        _newPosition = transform.position + new Vector3(_xVelocity, _yVelocity) * Time.fixedDeltaTime;

        MoveRight();
        MoveLeft();
        MoveTop();
        MoveBottom();
        PushOut();

        _drop.Rigidbody.MovePosition(_newPosition);
    }

    private void PushOut()
    {
        if (_isCurrentBlockSolid)
        {
            if (!_isTopBlockSolid)
            {
                _newPosition.y = _intPosition.y + 1 + _colliderHalfSize.y;
            }
            else
            {
                _newPosition.y = _intPosition.y - _colliderHalfSize.y;
            }
            if (!_isRightBlockSolid)
            {
                _newPosition.x = _intPosition.x + 1 + _colliderHalfSize.x;
            }
            else
            {
                _newPosition.x = _intPosition.x - _colliderHalfSize.x;
            }
            _xVelocity = 0;
        }
    }

    private void MoveBottom()
    {
        if (_isBottomBlockSolid)
        {
            if (_newPosition.y < _intPosition.y + _colliderHalfSize.y)
            {
                _newPosition.y = _intPosition.y + _colliderHalfSize.y;
                _yVelocity = 0;
            }
        }
    }

    private void MoveTop()
    {
        if (_isTopBlockSolid)
        {
            if (_newPosition.y > (_intPosition.y + 1) - _colliderHalfSize.y)
            {
                _newPosition.y = (_intPosition.y + 1) - _colliderHalfSize.y;
                _yVelocity = 0;
            }
        }
    }

    private void MoveLeft()
    {
        if (_xVelocity < 0 && _isLeftBlockSolid)
        {
            if (_newPosition.x < _intPosition.x + _colliderHalfSize.x)
            {
                _newPosition.x = _intPosition.x + _colliderHalfSize.x;
                _xVelocity = 0;
            }
        }
    }

    private void MoveRight()
    {
        if (_xVelocity > 0 && _isRightBlockSolid)
        {
            if (_newPosition.x > (_intPosition.x + 1) - _colliderHalfSize.x)
            {
                _newPosition.x = (_intPosition.x + 1) - _colliderHalfSize.x;
                _xVelocity = 0;
            }
        }
    }

    public void AddForce()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 direction = Input.mousePosition - screenPoint;
        direction.Normalize();

        _xVelocity = direction.x * _speed;
        _yVelocity = direction.y * _speed;

        _drop.IsAttractionEnabled = false;
    }
    #endregion
}
