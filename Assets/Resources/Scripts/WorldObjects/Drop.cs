using System.Collections;
using UnityEngine;

public class Drop : MonoBehaviour
{
    [SerializeField] private ItemSO _dropItem;
    [SerializeField] private int _quantity = 1;
    [SerializeField] private Vector3 _groundCheckOffset;
    [SerializeField] private float _fallingVelocity;
    [SerializeField] private CheckingAreaUtil _isPlayerInMagnetArea;
    [SerializeField] private float _magnetVelocity;
    [SerializeField] private float _magnetCooldown;
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isMerge;
    [SerializeField] private bool _isDropAttracted;
    [SerializeField] private bool _isDropCanMagnet;
    [SerializeField] private bool _isDropCanBePickedUp;

    private World _world;
    private Rigidbody2D _rigidbody;

    public ItemSO DropItem
    {
        get
        {
            return _dropItem;
        }

        set
        {
            _dropItem = value;
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
            _quantity = value;
        }
    }

    public bool IsMerge
    {
        get
        {
            return _isMerge;
        }

        set
        {
            _isMerge = value;
        }
    }

    public bool IsDropCanMagnet
    {
        get
        {
            return _isDropCanMagnet;
        }

        set
        {
            _isDropCanMagnet = value;
        }
    }

    public bool IsDropCanBePickedUp
    {
        get
        {
            return _isDropCanBePickedUp;
        }

        set
        {
            _isDropCanBePickedUp = value;
        }
    }

    private void Start()
    {
        Debug.Log("Drop created");
        Physics2D.IgnoreLayerCollision(6, 11);
        Physics2D.IgnoreLayerCollision(7, 11);
        Physics2D.IgnoreLayerCollision(11, 11);
        _world = transform.parent.parent.GetComponent<World>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Vector2 center = transform.GetComponent<BoxCollider2D>().bounds.center;
        var checkingResult = _isPlayerInMagnetArea.CheckArea(center, transform.parent.gameObject);
        if (checkingResult.Item1 && IsDropCanMagnet)
        {
            if (checkingResult.Item2.GetComponent<Interactions>().IsDropCanBeAdded(_dropItem))
            {
                _isDropAttracted = true;
                MoveToTheTarget(checkingResult.Item2.transform);
            }
            else
            {
                _isDropAttracted = false;
            }
        }
        else
        {
            _isDropAttracted = false;
        }

        if (!_world.IsAdjacentBlockSolid(transform.position + _groundCheckOffset, Vector2Int.down) && !_isDropAttracted)
        {
            _rigidbody.MovePosition(transform.position + new Vector3(0f, -_fallingVelocity) * Time.fixedDeltaTime);
        }
    }

    private void MoveToTheTarget(Transform target)
    {
        Vector3 distance = (target.position - transform.position).normalized;
        _rigidbody.MovePosition(transform.position + distance * _magnetVelocity * Time.fixedDeltaTime);
    }

    public IEnumerator WaitToMagnet()
    {
        yield return new WaitForSeconds(_magnetCooldown);
        IsDropCanMagnet = true;
        _isDropCanBePickedUp = true;
    }
}
