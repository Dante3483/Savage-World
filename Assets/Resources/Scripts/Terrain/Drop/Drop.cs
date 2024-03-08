using Items;
using System;
using System.Collections;
using UnityEngine;

public class Drop : MonoBehaviour
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private BoxCollider2D _boxCollider;
    [SerializeField] private ItemSO _item;
    [SerializeField] private int _quantity;

    private bool _isPhysicsEnabled;
    private bool _isAttractionEnabled;
    private bool _isMergingEnabled;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public Rigidbody2D Rigidbody
    {
        get
        {
            return _rigidbody;
        }
    }

    public BoxCollider2D BoxCollider
    {
        get
        {
            return _boxCollider;
        }
    }

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

    public bool IsPhysicsEnabled
    {
        get
        {
            return _isPhysicsEnabled;
        }

        set
        {
            _isPhysicsEnabled = value;
        }
    }

    public bool IsAttractionEnabled
    {
        get
        {
            return _isAttractionEnabled;
        }

        set
        {
            _isAttractionEnabled = value;
        }
    }

    public bool IsMergingEnabled
    {
        get
        {
            return _isMergingEnabled;
        }

        set
        {
            _isMergingEnabled = value;
        }
    }
    #endregion

    #region Methods
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _boxCollider= GetComponent<BoxCollider2D>();

        _isPhysicsEnabled = true;
        _isAttractionEnabled = true;
        _isMergingEnabled = true;
    }
    #endregion
}
