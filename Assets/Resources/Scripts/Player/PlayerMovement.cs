using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private PlayerDebugger _playerDebugger;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private BoxCollider2D _boxCollider;

    [Header("Animation")]
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private string _currentAnimationState;
    private PlayerAnimations _playerAnimations;

    [Header("Movement")]
    [SerializeField] private float _gravityScale;
    [SerializeField] private int _movementDirection;

    [Header("Layers")]
    [SerializeField] private LayerMask _groundLayer;

    [Header("Ground check")]
    [SerializeField] private BoxCastUtil _groundCheckBoxCast;

    [Header("Wall check")]
    [SerializeField] private RaycastUtil _wallCheckRaycast;

    [Header("Slope check")]
    [SerializeField] private float _slopeCheckDistance;
    [SerializeField] private float _slopeAngle;
    [SerializeField] private Vector2 _slopeNormalPerpendicular;
    [SerializeField] private PhysicsMaterial2D _noFriction;
    [SerializeField] private PhysicsMaterial2D _fullFriction;
    private RaycastUtil _slopeCheckRaycast;

    [Header("Flags")]
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isJumpCooldownComplete;
    [SerializeField] private bool _isFacingRight;
    [SerializeField] private bool _isWallInFront;
    [SerializeField] private bool _isOnSlope;
    [SerializeField] private bool _isIdle;
    [SerializeField] private bool _isWalking;
    [SerializeField] private bool _isRunning;
    [SerializeField] private bool _isJumping;
    [SerializeField] private bool _isFalling;
    [SerializeField] private bool _isHurt;
    [SerializeField] private bool _isDeath;

    #endregion

    #region Public fields

    #endregion

    #region Properties
    public int MovementDirection
    {
        get
        {
            return _movementDirection;
        }

        set
        {
            _movementDirection = value;
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

    public bool IsIdle
    {
        get
        {
            return _isIdle;
        }

        set
        {
            _isIdle = value;
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

    public bool IsOnSlope
    {
        get
        {
            return _isOnSlope;
        }

        set
        {
            _isOnSlope = value;
        }
    }

    public bool IsWallInFront
    {
        get
        {
            return _isWallInFront;
        }

        set
        {
            _isWallInFront = value;
        }
    }

    public bool IsJumpCooldownComplete
    {
        get
        {
            return _isJumpCooldownComplete;
        }

        set
        {
            _isJumpCooldownComplete = value;
        }
    }

    public string CurrentAnimationState
    {
        get
        {
            return _currentAnimationState;
        }

        set
        {
            _currentAnimationState = value;
        }
    }

    public bool IsHurt
    {
        get
        {
            return _isHurt;
        }

        set
        {
            _isHurt = value;
        }
    }

    public bool IsDeath
    {
        get
        {
            return _isDeath;
        }

        set
        {
            _isDeath = value;
        }
    }
    #endregion

    #region Methods

    #region General
    private void Awake()
    {
        #region Initialization
        _playerStats = GetComponent<PlayerStats>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _playerDebugger = GetComponent<PlayerDebugger>();
        _playerAnimator = GetComponent<Animator>();
        _playerAnimations = new PlayerAnimations();

        _isFacingRight = true;
        _isJumpCooldownComplete = true;
        #endregion
    }

    private void FixedUpdate()
    {
        GroundCheck();
        WallCheck();
        SlopeCheck();
        JumpingCheck();
        FallingCheck();
        Move();
        FixVelocity();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V) && !IsHurt && !IsDeath)
        {
            IsHurt = true;
            IsDeath = _playerStats.DecreaseHealth(10);
        }
        HorizontalMovementCheck();
        Jump();
        SetFriction();
        SelectAnimation();
    }
    #endregion

    #region Movement
    private void Move()
    {
        if (IsDeath)
        {
            return;
        }
        float xSpeed = IsRunning ? _playerStats.RunningSpeed : _playerStats.WalkingSpeed;

        if (IsGrounded && !IsOnSlope && !IsJumping && _rigidbody.velocity.y >= -0.05f)
        {
            _rigidbody.velocity = new Vector2(xSpeed * MovementDirection, 0.0f);
        }
        else if (IsGrounded && IsOnSlope && !IsJumping)
        {
            _rigidbody.velocity = new Vector2(xSpeed * _slopeNormalPerpendicular.x * -MovementDirection, xSpeed * _slopeNormalPerpendicular.y * -MovementDirection);
        }
        else if (!IsGrounded)
        {
            _rigidbody.velocity = new Vector2(xSpeed * MovementDirection, _rigidbody.velocity.y);
        }
    }

    private void Jump()
    {
        if (IsDeath)
        {
            return;
        }

        if (IsGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            IsJumping = true;
            _rigidbody.velocity = Vector2.up * _playerStats.JumpForce;
            //StartCoroutine(JumpCooldown());
        }

        if (Input.GetKeyUp(KeyCode.Space) && _rigidbody.velocity.y > 0)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y * 0.5f);
        }
    }

    private void HurtComple()
    {
        IsHurt = false;
    }
    #endregion

    #region Checkers
    private void GroundCheck()
    {
        // Center bottom point
        Vector3 origin = _boxCollider.bounds.center;
        origin.y -= _boxCollider.bounds.extents.y;
        _groundCheckBoxCast.BoxCast(origin, out bool result, _playerDebugger.EnableGroundCheckVizualization);
        IsGrounded = result;
    }

    private void HorizontalMovementCheck()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        if (inputX > 0f)
        {
            IsIdle = false;
            IsWalking = true;
            MovementDirection = 1;
            if (!IsFacingRight)
            {
                Flip();
            }
        }
        else if (inputX < 0f)
        {
            IsIdle = false;
            IsWalking = true;
            MovementDirection = -1;
            if (IsFacingRight)
            {
                Flip();
            }
        }
        else
        {
            IsIdle = true;
            IsWalking = false;
            MovementDirection = 0;
        }

        if (IsWalking && Input.GetKey(KeyCode.LeftShift))
        {
            IsWalking = false;
            IsRunning = true;
        }
        else
        {
            IsRunning = false;
        }

        if (IsWallInFront && MovementDirection == transform.right.x)
        {
            IsWalking = false;
            IsRunning = false;
            IsIdle = true;
            MovementDirection = 0;
        }
    }

    private void SlopeCheck()
    {
        if (IsWallInFront || IsJumping)
        {
            IsOnSlope = false;
            return;
        }

        Vector2 checkPosistion = _boxCollider.bounds.center - new Vector3(0, _boxCollider.bounds.extents.y);

        RaycastHit2D slopeHitFront = _slopeCheckRaycast.Raycast(
            checkPosistion, 
            transform.right, 
            _slopeCheckDistance, 
            _groundLayer,
            Color.cyan,
            Color.red,
            _playerDebugger.EnableSlopeCheckVizualization);

        RaycastHit2D slopeHitBack = _slopeCheckRaycast.Raycast(
            checkPosistion, 
            -transform.right, 
            _slopeCheckDistance,
            _groundLayer,
            Color.cyan,
            Color.red,
            _playerDebugger.EnableSlopeCheckVizualization);

        if (slopeHitFront)
        {
            _slopeAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
            _slopeNormalPerpendicular = Vector2.Perpendicular(slopeHitFront.normal).normalized;

            if (_playerDebugger.EnableSlopeCheckVizualization)
            {
                Debug.DrawRay(slopeHitFront.point, _slopeNormalPerpendicular, Color.blue);
                Debug.DrawRay(slopeHitFront.point, slopeHitFront.normal, Color.red);
            }
        }
        else if (slopeHitBack)
        {
            _slopeAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
            _slopeNormalPerpendicular = Vector2.Perpendicular(slopeHitBack.normal).normalized;

            if (_playerDebugger.EnableSlopeCheckVizualization)
            {
                Debug.DrawRay(slopeHitBack.point, _slopeNormalPerpendicular, Color.blue);
                Debug.DrawRay(slopeHitBack.point, slopeHitBack.normal, Color.red);
            }
        }
        else
        {
            _slopeAngle = 0.0f;
        }

        IsOnSlope = _slopeAngle != 0f && _slopeAngle != 90;

        if (IsOnSlope)
        {
            IsGrounded = true;
        }
    }

    private void WallCheck()
    {
        _wallCheckRaycast.Direction = transform.right;
        _wallCheckRaycast.Raycast(_boxCollider.bounds.center, out bool result, _playerDebugger.EnableWallCheckVizualization);
        IsWallInFront = result;
    }

    private void JumpingCheck()
    {
        if (_rigidbody.velocity.y <= 0)
        {
            IsJumping = false;
        }
    }

    private void FallingCheck()
    {
        IsFalling = _rigidbody.velocity.y < 0 && !IsGrounded && !IsOnSlope;
        if (IsFalling)
        {
            IsJumping = false;
        }
    }
    #endregion

    #region Others
    private void Flip()
    {
        if (IsDeath)
        {
            return;
        }
        _isFacingRight = !_isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void FixVelocity()
    {
        if (_rigidbody.velocity.y > 0 && IsOnSlope && !IsJumping)
        {
            _rigidbody.velocity *= 1.5f;
        }
    }

    private void SetFriction()
    {
        if (MovementDirection == 0)
        {
            _rigidbody.sharedMaterial = _fullFriction;
        }
        else
        {
            _rigidbody.sharedMaterial = _noFriction;
        }
    }
    #endregion

    #region Animations
    private void SelectAnimation()
    {
        string newAnimationState = _playerAnimations.PlayerIdle;

        if (IsWalking)
        {
            newAnimationState = _playerAnimations.PlayerWalk;
        }

        if (IsRunning)
        {
            newAnimationState = _playerAnimations.PlayerRun;
        }

        if (IsJumping)
        {
            newAnimationState = _playerAnimations.PlayerJump;
        }

        if (IsFalling)
        {
            newAnimationState = _playerAnimations.PlayerFall;
        }

        if (IsHurt)
        {
            newAnimationState = _playerAnimations.PlayerHurt;
        }

        if (!IsHurt && IsDeath && IsGrounded)
        {
            newAnimationState = _playerAnimations.PlayerDeath;
        }

        ChangeAnimationState(newAnimationState);
    }

    private void ChangeAnimationState(string newAnimationState)
    {
        if (CurrentAnimationState == newAnimationState)
        {
            return;
        }

        _playerAnimator.Play(newAnimationState);

        CurrentAnimationState = newAnimationState;
    }
    #endregion

    #endregion
}
