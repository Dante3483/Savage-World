using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Animator))]
public class NPC : MonoBehaviour
{
    public enum MobAction
    {
        Nothing = 0,
        Idle = 1,
        Walk = 2,
        Run = 3,
        Jump = 4,
        Fall = 5,
        Attack = 6,
        Death = 7,
    }

    #region Private fields
    [Header("Main Properties")]
    [SerializeField] private bool _needUpdate;
    [SerializeField] private NPCID _id;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private CapsuleCollider2D _capsuleCollider;
    [SerializeField] private MobAction _currentAction;
    [SerializeField] private float _actionCooldown;
    [SerializeField] private World _world;

    [Header("Ground Checking")]
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private Vector2 _groundCheckSize;
    [SerializeField] private Vector2 _groundCheckCenterOffset;
    [SerializeField] private float _extraWidth;
    [SerializeField] private Color _isGroundedColor;
    [SerializeField] private Color _isNotGroundedColor;

    [Header("Walking")]
    [SerializeField] private int _moveDirection;
    [SerializeField] private float _minSpeed;
    [SerializeField] private float _maxSpeed;

    [Header("Jumping")]
    [SerializeField] private float _jumpForce;

    [Header("Animation Properties")]
    [SerializeField] private Animator _animator;

    [Header("NPC Flags")]
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isJumping;
    [SerializeField] private bool _isFalling;
    [SerializeField] private bool _isFacingRight;
    [SerializeField] private bool _isWalking;
    [SerializeField] private bool _isRunning;
    [SerializeField] private bool _isAttacking;
    #endregion

    #region Properties
    public bool NeedUpdate
    {
        get
        {
            return _needUpdate;
        }

        set
        {
            _needUpdate = value;
        }
    }

    public NPCID Id
    {
        get
        {
            return _id;
        }

        set
        {
            _id = value;
        }
    }

    public Rigidbody2D Rigidbody
    {
        get
        {
            return _rigidbody;
        }

        set
        {
            _rigidbody = value;
        }
    }

    public CapsuleCollider2D CapsuleCollider
    {
        get
        {
            return _capsuleCollider;
        }

        set
        {
            _capsuleCollider = value;
        }
    }

    public LayerMask GroundLayerMask
    {
        get
        {
            return _groundLayerMask;
        }

        set
        {
            _groundLayerMask = value;
        }
    }

    public Vector2 GroundCheckSize
    {
        get
        {
            return _groundCheckSize;
        }

        set
        {
            _groundCheckSize = value;
        }
    }

    public Vector2 GroundCheckCenterOffset
    {
        get
        {
            return _groundCheckCenterOffset;
        }

        set
        {
            _groundCheckCenterOffset = value;
        }
    }

    public float ExtraWidth
    {
        get
        {
            return _extraWidth;
        }

        set
        {
            _extraWidth = value;
        }
    }

    public Color IsGroundedColor
    {
        get
        {
            return _isGroundedColor;
        }

        set
        {
            _isGroundedColor = value;
        }
    }

    public Color IsNotGroundedColor
    {
        get
        {
            return _isNotGroundedColor;
        }

        set
        {
            _isNotGroundedColor = value;
        }
    }

    public int MoveDirection
    {
        get
        {
            return _moveDirection;
        }

        set
        {
            _moveDirection = value;
        }
    }

    public float MinSpeed
    {
        get
        {
            return _minSpeed;
        }

        set
        {
            _minSpeed = value;
        }
    }

    public float MaxSpeed
    {
        get
        {
            return _maxSpeed;
        }

        set
        {
            _maxSpeed = value;
        }
    }

    public MobAction CurrentAction
    {
        get
        {
            return _currentAction;
        }

        set
        {
            _currentAction = value;
        }
    }

    public float JumpForce
    {
        get
        {
            return _jumpForce;
        }

        set
        {
            _jumpForce = value;
        }
    }

    public Animator Animator
    {
        get
        {
            return _animator;
        }

        set
        {
            _animator = value;
        }
    }

    public bool IsGrounded
    {
        get
        {
            return _isGrounded;
        }

        set
        {
            _isGrounded = value;
        }
    }

    public bool IsJumping
    {
        get
        {
            return _isJumping;
        }

        set
        {
            _isJumping = value;
        }
    }

    public bool IsFalling
    {
        get
        {
            return _isFalling;
        }

        set
        {
            _isFalling = value;
        }
    }

    public bool IsFacingRight
    {
        get
        {
            return _isFacingRight;
        }

        set
        {
            _isFacingRight = value;
        }
    }

    public bool IsWalking
    {
        get
        {
            return _isWalking;
        }

        set
        {
            _isWalking = value;
        }
    }

    public bool IsRunning
    {
        get
        {
            return _isRunning;
        }

        set
        {
            _isRunning = value;
        }
    }

    public float ActionCooldown
    {
        get
        {
            return _actionCooldown;
        }

        set
        {
            _actionCooldown = value;
        }
    }

    public bool IsAttacking
    {
        get
        {
            return _isAttacking;
        }

        set
        {
            _isAttacking = value;
        }
    }

    public World World
    {
        get
        {
            return _world;
        }

        set
        {
            _world = value;
        }
    }
    #endregion

    #region Methods
    public void Start()
    {
        Physics2D.IgnoreLayerCollision(6, 7);
        Physics2D.IgnoreLayerCollision(7, 7);
        UpdateData();
        World = GameObject.FindGameObjectWithTag("World").GetComponent<World>();
    }

    public void OnValidate()
    {
        if (NeedUpdate)
        {
            UpdateData();
            NeedUpdate = !NeedUpdate;
        }
    }

    private void UpdateData()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        CapsuleCollider = GetComponent<CapsuleCollider2D>();
        Animator = GetComponent<Animator>();
        IsFacingRight = true;
    }

    public void Animate()
    {
        bool isIdle = false;
        bool isWalk = false;
        bool isRun = false;
        bool isJump = false;
        bool isFall = false;
        bool isAttack = false;
        bool isDeath = false;
        switch (CurrentAction)
        {
            case MobAction.Nothing:
                break;
            case MobAction.Idle:
                {
                    isIdle = true;
                }
                break;
            case MobAction.Walk:
                {
                    isWalk = true;
                }
                break;
            case MobAction.Run:
                {
                    isRun = true;
                }
                break;
            case MobAction.Jump:
                {
                    isJump = true;
                }
                break;
            case MobAction.Fall:
                {
                    isFall = true;
                }
                break;
            case MobAction.Attack:
                {
                    isAttack = true;
                }
                break;
            case MobAction.Death:
                {
                    isDeath = true;
                }
                break;
            default:
                break;
        }
        Animator.SetBool("IsIdle", isIdle);
        Animator.SetBool("IsWalk", isWalk);
        Animator.SetBool("IsRun", isRun);
        Animator.SetBool("IsJump", isJump);
        Animator.SetBool("IsFall", isFall);
        Animator.SetBool("IsAttack", isAttack);
        Animator.SetBool("IsDeath", isDeath);

        //if (IsGrounded)
        //{
        //    Animator.SetBool("Jump", false);
        //}
        //if (IsJumping)
        //{
        //    Animator.SetBool("Jump", true);
        //    Animator.SetFloat("yVelocity", Rigidbody.velocity.y);
        //}
    }

    public bool GroundCheck()
    {
        Vector2 center = CapsuleCollider.bounds.center;
        center += GroundCheckCenterOffset;

        RaycastHit2D raycastHit = Physics2D.BoxCast(center, GroundCheckSize, 0f, Vector2.down, ExtraWidth, GroundLayerMask);

        Color rayColor;
        if (raycastHit.collider != null)
        {
            rayColor = IsGroundedColor;
        }
        else
        {
            rayColor = IsNotGroundedColor;
        }

        Vector2 halfSize = GroundCheckSize / 2f;
        Vector2 centerForDebug = center + halfSize;

        Debug.DrawRay(centerForDebug + new Vector2(halfSize.x, 0), Vector2.down * (GroundCheckSize.y + ExtraWidth), rayColor);
        Debug.DrawRay(centerForDebug - new Vector2(halfSize.x, 0), Vector2.down * (GroundCheckSize.y + ExtraWidth), rayColor);
        Debug.DrawRay(centerForDebug - new Vector2(halfSize.x, GroundCheckSize.y + ExtraWidth), Vector2.right * GroundCheckSize.x, rayColor);
        Debug.DrawRay(centerForDebug - new Vector2(halfSize.x, 0), Vector2.right * GroundCheckSize.x, rayColor);

        Debug.Log(raycastHit.collider);
        return raycastHit.collider != null;
    }

    public bool JumpingCheck()
    {
        return Rigidbody.velocity.y > 0 && !IsGrounded;
    }

    public bool FallingCheck()
    {
        return Rigidbody.velocity.y < 0 && !IsGrounded;
    }

    public void Flip()
    {
        IsFacingRight = !IsFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    #endregion
}
