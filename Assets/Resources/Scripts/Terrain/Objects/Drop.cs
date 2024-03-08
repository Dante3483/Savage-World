using CustomTilemap;
using Items;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Drop : MonoBehaviour
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private BoxCollider2D _boxCollider;
    [SerializeField] private Vector3Int _intPosition;
    [SerializeField] private ItemSO _item;
    [SerializeField] private int _quantity;

    [Header("Moving")]
    [SerializeField] private Vector2 _colliderSize;
    [SerializeField] private float _speed;
    [SerializeField] private float _gravity = 30;
    [SerializeField] private float _xVelocity;
    [SerializeField] private float _yVelocity;

    [Header("Merging")]
    [SerializeField] private BoxCastUtil _mergeCheckBoxCast;

    [Header("Attraction")]
    [SerializeField] private bool _canBeAttracted;
    [SerializeField] private float _attractionCooldown;
    [SerializeField] private Transform _attractionTarget;
    [SerializeField] private float _attractionSpeed;
    #endregion

    #region Public fields
    public Action<Drop> OnEndOfAttraction;
    #endregion

    #region Properties
    public ItemSO Item
    {
        get
        {
            return _item;
        }

        set
        {
            _spriteRenderer.sprite = value.ItemImage;
            _item = value;
        }
    }

    public int Quantity
    {
        get
        {
            return _quantity;
        }

        set
        {
            if (value == 0)
            {
                Destroy(gameObject);
            }
            _quantity = value;
        }
    }

    public Transform AttractionTarget
    {
        get
        {
            return _attractionTarget;
        }

        set
        {
            _attractionTarget = value;
        }
    }
    #endregion

    #region Methods
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _colliderSize = GetComponent<BoxCollider2D>().size / 2;
        _mergeCheckBoxCast.Self = gameObject;
        _canBeAttracted = true;
    }

    private void FixedUpdate()
    {
        _intPosition = Vector3Int.FloorToInt(transform.position);
        if (_canBeAttracted && _attractionTarget != null)
        {
            Attract();
        }
        else
        {
            Fall();
        }
        Merge();
    }

    private void Attract()
    {
        float distance = Vector3.Distance(transform.position, _attractionTarget.position);
        if (distance < 0.5f)
        {
            OnEndOfAttraction?.Invoke(this);
        }
        Vector3 direction = (_attractionTarget.position - transform.position).normalized;
        _rigidbody.MovePosition(transform.position + direction * _attractionSpeed * Time.fixedDeltaTime);
        _attractionTarget = null;
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

    private void Merge()
    {
        RaycastHit2D dropHit = _mergeCheckBoxCast.BoxCast(transform.position);
        if (_mergeCheckBoxCast.Result)
        {
            Drop drop = dropHit.collider.GetComponent<Drop>();
            if (drop.Item == _item)
            {
                _quantity += drop.Quantity;
                Destroy(drop.gameObject);
            }
        }
    }

    private IEnumerator AttractionCooldownCoroutine()
    {
        _canBeAttracted = false;
        yield return new WaitForSeconds(_attractionCooldown);
        _canBeAttracted = true;
    }

    public void AddForce()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 direction = Input.mousePosition - screenPoint;
        direction.Normalize();

        _xVelocity = direction.x * _speed;
        _yVelocity = direction.y * _speed;

        StartCoroutine(AttractionCooldownCoroutine());
    }
    #endregion
}
