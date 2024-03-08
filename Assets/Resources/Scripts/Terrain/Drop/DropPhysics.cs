using System.Collections;
using UnityEngine;

public class DropPhysics : MonoBehaviour
{
    #region Private fields
    [SerializeField] private Drop _drop;
    [SerializeField] private Vector2 _colliderHalfSize;
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
        _drop = GetComponent<Drop>();
        _colliderHalfSize = _drop.BoxCollider.size / 2;
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
        Vector3Int intPosition = Vector3Int.FloorToInt(transform.position);
        bool isBottomBlockSolid = WorldDataManager.Instance.GetAdjacentWorldCellData(intPosition.x, intPosition.y, Vector2Int.down).IsSolid();
        if (transform.position.y == intPosition.y + _colliderHalfSize.y && isBottomBlockSolid)
        {
            return;
        }

        bool isRightBlockSolid = WorldDataManager.Instance.GetAdjacentWorldCellData(intPosition.x, intPosition.y, Vector2Int.right).IsSolid();
        bool isLeftBlockSolid = WorldDataManager.Instance.GetAdjacentWorldCellData(intPosition.x, intPosition.y, Vector2Int.left).IsSolid();
        bool isCurrentBlockSolid = WorldDataManager.Instance.GetWorldCellData(intPosition.x, intPosition.y).IsSolid();

        _xVelocity = Mathf.Lerp(_xVelocity, 0, Time.fixedDeltaTime);
        _yVelocity = Mathf.Lerp(_yVelocity, -_gravity, Time.fixedDeltaTime);
        Vector3 newPosition = transform.position + new Vector3(_xVelocity, _yVelocity) * Time.fixedDeltaTime;
        if (_xVelocity > 0 && isRightBlockSolid)
        {
            newPosition.x = Mathf.Min(newPosition.x, (intPosition.x + 1) - _colliderHalfSize.x);
        }
        if (_xVelocity < 0 && isLeftBlockSolid)
        {
            newPosition.x = Mathf.Max(newPosition.x, intPosition.x + _colliderHalfSize.x);
        }
        if (isBottomBlockSolid)
        {
            newPosition.y = Mathf.Max(newPosition.y, intPosition.y + _colliderHalfSize.y);
        }
        if (isCurrentBlockSolid)
        {
            newPosition.y = intPosition.y + 1;
        }
        _drop.Rigidbody.MovePosition(newPosition);
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
