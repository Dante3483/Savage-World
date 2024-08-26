using Items;
using SavageWorld.Runtime;
using SavageWorld.Runtime.Enums.Network;
using SavageWorld.Runtime.Network;
using SavageWorld.Runtime.Network.Messages;
using System;
using UnityEngine;

public class Drop : GameObjectBase
{
    #region Private fields
    [Header("Main")]
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    [SerializeField]
    private Rigidbody2D _rigidbody;
    [SerializeField]
    private BoxCollider2D _boxCollider;
    [SerializeField]
    private Vector2 _maxColliderSize;
    [SerializeField]
    private ItemSO _item;
    [SerializeField]
    private int _quantity;
    private DropAttraction _attraction;
    [SerializeField]
    private bool _hasTarget;
    [SerializeField]
    private bool _isAnotherObjectTarget;

    private bool _isPhysicsEnabled;
    private bool _isAttractionEnabled;
    private bool _isMergingEnabled;
    #endregion

    #region Public fields
    public Action OnColliderSizeChanged;
    public Action OnDropReset;
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
            SetSprite(value.SmallItemImage);
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
                DestroySelf();
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

    public bool HasTarget
    {
        get
        {
            return _hasTarget;
        }

        set
        {
            _hasTarget = value;
        }
    }

    public bool IsAnotherObjectTarget
    {
        get
        {
            return _isAnotherObjectTarget;
        }

        set
        {
            _isAnotherObjectTarget = value;
        }
    }
    #endregion

    #region Methods
    private void Awake()
    {
        NetworkObject.Type = NetworkObjectTypes.Drop;
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _attraction = GetComponent<DropAttraction>();

        _isPhysicsEnabled = true;
        _isAttractionEnabled = true;
        _isMergingEnabled = true;
    }

    public void SetTarget(Transform target, Action<Drop> endAttractionCallback)
    {
        if (_hasTarget)
        {
            return;
        }
        if (NetworkManager.Instance.IsClient)
        {
            if (_isAnotherObjectTarget)
            {
                return;
            }
            MessageData messageData = new()
            {
                Bool1 = false,
                LongNumber1 = GameManager.Instance.Player.NetworkObject.Id,
                LongNumber2 = NetworkObject.Id
            };
            NetworkManager.Instance.BroadcastMessage(NetworkMessageTypes.TakeDrop, messageData);
        }
        _attraction.Target = target;
        _hasTarget = true;
        _attraction.OnEndOfAttraction = endAttractionCallback;
    }

    public void RemoveTarget(Transform target)
    {
        if (!_hasTarget || _attraction.Target != target)
        {
            return;
        }
        if (NetworkManager.Instance.IsClient)
        {
            if (_isAnotherObjectTarget)
            {
                return;
            }
            MessageData messageData = new()
            {
                Bool1 = true,
                LongNumber1 = GameManager.Instance.Player.NetworkObject.Id,
                LongNumber2 = NetworkObject.Id
            };
            NetworkManager.Instance.BroadcastMessage(NetworkMessageTypes.TakeDrop, messageData);
        }
        _attraction.Target = null;
        _hasTarget = false;
        _attraction.OnEndOfAttraction = null;
    }

    public void EndAttraction()
    {
        _attraction.EndAttraction();
    }

    private void SetSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
        Vector2 size = sprite.bounds.size;
        float aspectRatio = size.x / size.y;
        float currentSizeX = Mathf.Min(sprite.bounds.size.x, _maxColliderSize.x);
        float currentSizeY = Mathf.Min(sprite.bounds.size.y, _maxColliderSize.y);
        float newScaleX;
        float newScaleY;
        if (aspectRatio > 1)
        {
            newScaleX = currentSizeX;
            newScaleY = currentSizeY / aspectRatio;
        }
        else
        {
            newScaleX = currentSizeX * aspectRatio;
            newScaleY = currentSizeY;
        }
        _boxCollider.size = new Vector2(newScaleX, newScaleY);
        newScaleX /= size.x;
        newScaleY /= size.y;
        _spriteRenderer.transform.localScale = new Vector3(newScaleX, newScaleY, 1);
        OnColliderSizeChanged?.Invoke();
    }

    public override GameObjectBase CreateInstance(Vector3 position, Transform parent = null, bool isOwner = true)
    {
        GameObjectBase instance = base.CreateInstance(position, parent, isOwner);
        if (!isOwner)
        {
            instance.GetComponent<Drop>().Rigidbody.bodyType = RigidbodyType2D.Static;
        }
        return instance;
    }
    #endregion
}
