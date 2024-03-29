using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private BoxCollider2D _boxCollider;

    [Header("Animation")]
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private string _currentAnimationState;
    private PlayerAnimationsController _playerAnimations;

    [Header("Collider")]
    [SerializeField] private Vector2 _standingColliderSize;
    [SerializeField] private Vector2 _standingColliderOffset;
    [Space]
    [SerializeField] private Vector2 _crouchingColliderSize;
    [SerializeField] private Vector2 _crouchingColliderOffset;
    [Space]
    [SerializeField] private Vector2 _slidingColliderSize;
    [SerializeField] private Vector2 _slidingColliderOffset;

    [Header("Movement")]
    [SerializeField] private float _gravityScale;
    [SerializeField] private int _movementDirection;
    [SerializeField] private int _slidingDirection;
    [SerializeField] private float _slidingCooldown;

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
    [SerializeField] private bool _isFacingRight;
    [SerializeField] private bool _isWallInFront;
    [SerializeField] private bool _isOnSlope;
    [SerializeField] private bool _isJumpCooldownComplete;
    [SerializeField] private bool _isMovementBlocked;
    [SerializeField] private bool _isJumpBlocked;
    [SerializeField] private bool _isFlipBlocked;
    [SerializeField] private bool _isSlidingBlocked;
    [SerializeField] private bool _isCancelSlidingBlocked;
    [SerializeField] private bool _isIdle;
    [SerializeField] private bool _isWalking;
    [SerializeField] private bool _isRunning;
    [SerializeField] private bool _isJumping;
    [SerializeField] private bool _isFalling;
    [SerializeField] private bool _isHurt;
    [SerializeField] private bool _isDeath;
    [SerializeField] private bool _isPunch;
    [SerializeField] private bool _isCrouch;
    [SerializeField] private bool _isStartSliding;
    [SerializeField] private bool _isSliding;
    [SerializeField] private bool _isEndSliding;

    private Coroutine _waitForMaxSlidingTime;

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

    public bool IsPunch
    {
        get
        {
            return _isPunch;
        }

        set
        {
            _isPunch = value;
        }
    }

    public bool IsCrouch
    {
        get
        {
            return _isCrouch;
        }

        set
        {
            _isCrouch = value;
        }
    }

    public bool IsSliding
    {
        get
        {
            return _isSliding;
        }

        set
        {
            _isSliding = value;
        }
    }

    public bool IsMovementBlocked
    {
        get
        {
            return _isMovementBlocked;
        }

        set
        {
            _isMovementBlocked = value;
        }
    }

    public int SlidingDirection
    {
        get
        {
            return _slidingDirection;
        }

        set
        {
            _slidingDirection = value;
        }
    }

    public bool IsJumpBlocked
    {
        get
        {
            return _isJumpBlocked;
        }

        set
        {
            _isJumpBlocked = value;
        }
    }

    public bool IsFlipBlocked
    {
        get
        {
            return _isFlipBlocked;
        }

        set
        {
            _isFlipBlocked = value;
        }
    }

    public bool IsSlidingBlocked
    {
        get
        {
            return _isSlidingBlocked;
        }

        set
        {
            _isSlidingBlocked = value;
        }
    }

    public bool IsStartSliding
    {
        get
        {
            return _isStartSliding;
        }

        set
        {
            _isStartSliding = value;
        }
    }

    public bool IsEndSliding
    {
        get
        {
            return _isEndSliding;
        }

        set
        {
            _isEndSliding = value;
        }
    }

    public bool IsCancelSlidingBlocked
    {
        get
        {
            return _isCancelSlidingBlocked;
        }

        set
        {
            _isCancelSlidingBlocked = value;
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
        _playerAnimator = GetComponent<Animator>();
        _playerAnimations = GetComponent<PlayerAnimationsController>();

        _isFacingRight = true;
        _isJumpCooldownComplete = true;
        _isSlidingBlocked = false;
        #endregion
    }

    private void FixedUpdate()
    {
        #region Checkers
        GroundCheck();
        WallCheck();
        SlopeCheck();
        JumpingCheck();
        FallingCheck();
        MovementAbilityCheck();
        #endregion

        ChangeCollider();
        Move();
        FixVelocity();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsPunch)
        {
            IsPunch = true;
        }
        if (Input.GetKeyDown(KeyCode.V) && !IsHurt && !IsDeath)
        {
            IsHurt = true;
            IsDeath = _playerStats.DecreaseHealth(10);
        }

        SetMovement();
        SetJump();
        SetFriction();
        SelectAnimation();
    }
    #endregion

    #region Movement
    private void SetMovement()
    {
        float inputX = Input.GetAxisRaw("Horizontal");

        //If we move to the right
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
        //Else if we move to the left
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
        //Else if we stand
        else
        {
            IsIdle = true;
            IsWalking = false;
            MovementDirection = 0;
        }

        //If we crouch
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            IsCrouch = !IsCrouch;
        }

        //If we run
        if (IsWalking && !IsCrouch && !IsWallInFront && Input.GetKey(KeyCode.LeftShift))
        {
            IsWalking = false;
            IsRunning = true;
        }
        else
        {
            IsRunning = false;
        }

        //If we slide
        if (IsRunning && !IsSliding && Input.GetKeyDown(KeyCode.C))
        {
            IsStartSliding = true;
            IsCancelSlidingBlocked = true;
            StartCoroutine(WaitForMinSlidingTime());
            _waitForMaxSlidingTime = StartCoroutine(WaitForMaxSlidingTime());
            SlidingDirection = MovementDirection;
        }

        if (IsSliding && !Input.GetKey(KeyCode.C) && !IsCancelSlidingBlocked)
        {
            IsEndSliding = true;
            StopCoroutine(_waitForMaxSlidingTime);
        }

        //If the is a wall ahead
        if (IsWallInFront && MovementDirection == transform.right.x)
        {
            IsWalking = false;
            IsRunning = false;
            if (IsSliding)
            {
                IsEndSliding = true;
            }
            IsIdle = true;
            MovementDirection = 0;
        }
    }

    private void Move()
    {
        if (IsMovementBlocked)
        {
            _rigidbody.velocity = Vector2.zero;
            return;
        }

        float xSpeed = _playerStats.WalkingSpeed;
        if (IsRunning)
        {
            xSpeed = _playerStats.RunningSpeed;
        }
        if (IsCrouch)
        {
            xSpeed = _playerStats.CrouchWalkingSpeed;
        }
        if (IsSliding)
        {
            xSpeed = _playerStats.SlidingSpeed;
        }

        int movementDirection = IsSliding ? SlidingDirection : MovementDirection;

        if (IsGrounded && !IsOnSlope && !IsJumping && _rigidbody.velocity.y >= -0.05f)
        {
            _rigidbody.velocity = new Vector2(xSpeed * movementDirection, 0.0f);
        }
        else if (IsGrounded && IsOnSlope && !IsJumping)
        {
            _rigidbody.velocity = new Vector2(xSpeed * _slopeNormalPerpendicular.x * -movementDirection, xSpeed * _slopeNormalPerpendicular.y * -movementDirection);
        }
        else if (!IsGrounded)
        {
            _rigidbody.velocity = new Vector2(xSpeed * movementDirection, _rigidbody.velocity.y);
        }
    }

    private void SetJump()
    {
        if (IsJumpBlocked)
        {
            return;
        }

        if (IsGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            IsJumping = true;
            _rigidbody.velocity = Vector2.up * _playerStats.JumpForce;
        }

        if (Input.GetKeyUp(KeyCode.Space) && _rigidbody.velocity.y > 0)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y * 0.5f);
        }
    }

    private void HurtComplete()
    {
        IsHurt = false;
    }

    private void PunchComplete()
    {
        IsPunch = false;
    }

    #region Sliding
    private IEnumerator WaitForMinSlidingTime()
    {
        yield return new WaitForSeconds(_playerStats.SlidingMinTime);
        IsCancelSlidingBlocked = false;
    }

    private IEnumerator WaitForMaxSlidingTime()
    {
        yield return new WaitForSeconds(_playerStats.SlidingMaxTime);
        if (IsSliding)
        {
            IsEndSliding = true;
        }
    }

    private void StartSlidingComplete()
    {
        IsStartSliding = false;
        IsSliding = true;
    }

    private void EndSlidingComplete()
    {
        IsEndSliding = false;
        StopSliding();
    }

    private void StopSliding()
    {
        IsSliding = false;
        SlidingDirection = 0;
    }
    #endregion

    #endregion

    #region Checks
    private void GroundCheck()
    {
        // Center bottom point
        Vector3 origin = _boxCollider.bounds.center;
        origin.y -= _boxCollider.bounds.extents.y;
        _groundCheckBoxCast.BoxCast(origin);
        IsGrounded = _groundCheckBoxCast.Result;
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
            Color.red);

        RaycastHit2D slopeHitBack = _slopeCheckRaycast.Raycast(
            checkPosistion, 
            -transform.right, 
            _slopeCheckDistance,
            _groundLayer,
            Color.cyan,
            Color.red);

        if (slopeHitFront)
        {
            _slopeAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
            _slopeNormalPerpendicular = Vector2.Perpendicular(slopeHitFront.normal).normalized;

            //if (_playerDebugger.EnableSlopeCheckVizualization)
            //{
            //    Debug.DrawRay(slopeHitFront.point, _slopeNormalPerpendicular, Color.blue);
            //    Debug.DrawRay(slopeHitFront.point, slopeHitFront.normal, Color.red);
            //}
        }
        else if (slopeHitBack)
        {
            _slopeAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
            _slopeNormalPerpendicular = Vector2.Perpendicular(slopeHitBack.normal).normalized;

            //if (_playerDebugger.EnableSlopeCheckVizualization)
            //{
            //    Debug.DrawRay(slopeHitBack.point, _slopeNormalPerpendicular, Color.blue);
            //    Debug.DrawRay(slopeHitBack.point, slopeHitBack.normal, Color.red);
            //}
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
        _wallCheckRaycast.Raycast(_boxCollider.bounds.center, out bool result);
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
            StopSliding();
        }
    }

    private void MovementAbilityCheck()
    {
        if (IsDeath || IsPunch || IsHurt || IsEndSliding)
        {
            IsMovementBlocked = true;
        }
        else
        {
            IsMovementBlocked = false;
        }

        if (IsDeath || IsPunch || IsHurt || IsSliding)
        {
            IsJumpBlocked = true;
        }
        else
        {
            IsJumpBlocked = false;
        }

        if (IsDeath || IsPunch || IsHurt || IsSliding)
        {
            IsFlipBlocked = true;
        }
        else
        {
            IsFlipBlocked = false;
        }
    }
    #endregion

    #region Others
    private void Flip()
    {
        if (IsFlipBlocked)
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
        if ((MovementDirection == 0 && SlidingDirection == 0) || IsMovementBlocked)
        {
            _rigidbody.sharedMaterial = _fullFriction;
        }
        else
        {
            _rigidbody.sharedMaterial = _noFriction;
        }
    }

    private void ChangeCollider()
    {
        if (IsCrouch)
        {
            _boxCollider.size = _crouchingColliderSize;
            _boxCollider.offset = _crouchingColliderOffset;
        }
        else
        {
            _boxCollider.size = _standingColliderSize;
            _boxCollider.offset = _standingColliderOffset;
        }

        if (IsSliding)
        {
            _boxCollider.size = _slidingColliderSize;
            _boxCollider.offset = _slidingColliderOffset;
        }
    }
    #endregion

    #region Animations
    private void SelectAnimation()
    {
        string newAnimationState = "";

        if (IsIdle)
        {
            if (IsCrouch)
            {
                newAnimationState = _playerAnimations.PlayerCrouchIdle;
            }
            else
            {
                newAnimationState = _playerAnimations.PlayerIdle;
            }
        }

        if (IsWalking)
        {
            if (IsCrouch)
            {
                newAnimationState = _playerAnimations.PlayerCrouchWalk;
            }
            else
            {
                newAnimationState = _playerAnimations.PlayerWalk;
            }
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

        if (IsPunch)
        {
            newAnimationState = _playerAnimations.PlayerPunch;
        }

        if (IsDeath && IsGrounded)
        {
            newAnimationState = _playerAnimations.PlayerDeath;
        }

        if (IsHurt)
        {
            newAnimationState = _playerAnimations.PlayerHurt;
        }

        if (IsStartSliding)
        {
            newAnimationState = _playerAnimations.PlayerStartSliding;
        }

        if (IsSliding)
        {
            newAnimationState = _playerAnimations.PlayerSliding;
        }

        if (IsEndSliding)
        {
            newAnimationState = _playerAnimations.PlayerEndSliding;
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
