using System.Collections.Generic;
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
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private MobAction _currentAction;
    [SerializeField] private float _actionCooldown;

    [Header("Ground Checking")]
    [SerializeField] private CheckingAreaUtil _groundCheck;

    [Header("Walking")]
    [SerializeField] private int _moveDirection;
    [SerializeField] private float _minSpeed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _minRandomDirectionChange;
    [SerializeField] private float _maxRandomDirectionChange;
    [SerializeField] private float _currentRandomDirectionChange;

    [Header("Jumping")]
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpXForce;


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

    [Header("Atack Properties")]
    [SerializeField] private Transform _target;
    [SerializeField] private float _attackSpeedMultiplier;
    [SerializeField] private bool _isTargetInAttackArea;
    [SerializeField] private float _attackCooldown;
    [SerializeField] private float _selfShootingCooldown;
    [SerializeField] private float _extraDistance;
    [SerializeField] private GameObject _attackCollider;
    [SerializeField] private GameObject _hitCollider;
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

    public float JumpXForce
    { 
        get => _jumpXForce;
        set => _jumpXForce = value; 
    }

    public SpriteRenderer SpriteRenderer { get => _spriteRenderer; set => _spriteRenderer = value; }

    public Transform Target { get => _target; set => _target = value; }

    public float AttackSpeedMultiplier { get => _attackSpeedMultiplier; set => _attackSpeedMultiplier = value; }

    public bool IsTargetInAttackArea { get => _isTargetInAttackArea; set => _isTargetInAttackArea = value; }

    public float AttackCooldown { get => _attackCooldown; set => _attackCooldown = value; }

    public float SelfShootingCooldown { get => _selfShootingCooldown; set => _selfShootingCooldown = value; }

    public float ExtraDistance { get => _extraDistance; set => _extraDistance = value; }

    public GameObject AttackCollider { get => _attackCollider; set => _attackCollider = value; }

    public GameObject HitCollider { get => _hitCollider; set => _hitCollider = value; }
    public float MinRandomDirectionChange { get => _minRandomDirectionChange; set => _minRandomDirectionChange = value; }
    public float MaxRandomDirectionChange { get => _maxRandomDirectionChange; set => _maxRandomDirectionChange = value; }
    public float CurrentRandomDirectionChange { get => _currentRandomDirectionChange; set => _currentRandomDirectionChange = value; }
    #endregion

    #region Methods
    public void Start()
    {

        Physics2D.IgnoreLayerCollision(6, 7);
        Physics2D.IgnoreLayerCollision(7, 7);
        UpdateData();
        
    }

    public void Update()
    {
        Animate();
    }

    public void FixedUpdate()
    {
        ActionCooldown += Time.fixedDeltaTime;
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
        SpriteRenderer = GetComponent<SpriteRenderer>();
        IsFacingRight = true;

        if (!transform.Find("AttackCollider"))
        {
            AttackCollider = new GameObject("AttackCollider");
            AttackCollider.AddComponent<MobAttackController>();
            AttackCollider.transform.parent = transform;
            AttackCollider.transform.position = transform.position;
        }

        if (!transform.Find("HitCollider"))
        {
            HitCollider = new GameObject("HitCollider");
            HitCollider.AddComponent<PolygonCollider2D>();
            HitCollider.GetComponent<PolygonCollider2D>().isTrigger = true;
            HitCollider.AddComponent<MobHitController>();
            HitCollider.transform.parent = transform;
            HitCollider.transform.position = transform.position;
        }

        _groundCheck.IsTrueColor = Color.green;
        _groundCheck.IsFalseColor = Color.red;
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
    }

    public bool GroundCheck()
    {
        var result = _groundCheck.CheckArea(transform.position, gameObject);
        return result.Item1;
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

    public void EndAttack()
    {
        IsAttacking = false;
    }

    public void ChooseDirectionByTarget()
    {
        if (IsJumping)
        {
            return;
        }
        if (transform.position.x - Target.position.x < 0)
        {
            MoveDirection = 1;
        }
        else
        {
            MoveDirection = -1;
        }
    }

    public void ChooseRandomDirection()
    {
        ActionCooldown = 0f;
        MoveDirection = Random.Range(-1, 2);
        CurrentRandomDirectionChange = Random.Range(MinRandomDirectionChange, MaxRandomDirectionChange);
    }

    public void ChangeScaleByMoveDirection()
    {
        if (IsFacingRight && MoveDirection < 0)
        {
            Flip();
        }
        if (!IsFacingRight && MoveDirection > 0)
        {
            Flip();
        }
    }

    public void ChangeScaleToNormal()
    {
        if (!IsFacingRight)
        {
            Flip();
        }
    }

    public void UpdatePhysicsShape()
    {
        List<Vector2> physicsShape = new List<Vector2>();
        SpriteRenderer.sprite.GetPhysicsShape(0, physicsShape);
        HitCollider.GetComponent<PolygonCollider2D>().SetPath(0, physicsShape);
    }
    #endregion
}
