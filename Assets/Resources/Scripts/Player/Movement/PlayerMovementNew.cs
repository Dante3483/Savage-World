using System;
using System.Collections;
using UnityEngine;

public class PlayerMovementNew : MonoBehaviour
{
    [Serializable]
    private struct StateProperties
    {
        [SerializeField] private Vector2 _colliderSize;
        [SerializeField] private Vector2 _colliderOffset;
        [SerializeField] private float _wallInFrontCheckDistance;

        public Vector2 ColliderSize
        {
            get
            {
                return _colliderSize;
            }

            set
            {
                _colliderSize = value;
            }
        }

        public Vector2 ColliderOffset
        {
            get
            {
                return _colliderOffset;
            }

            set
            {
                _colliderOffset = value;
            }
        }

        public float WallInFrontCheckDistance
        {
            get
            {
                return _wallInFrontCheckDistance;
            }

            set
            {
                _wallInFrontCheckDistance = value;
            }
        }
    }

    #region Private fields
    [Header("Main")]
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private PlayerFlags _playerFlags;
    [SerializeField] private PlayerAnimationsController _playerAnimationsController;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private BoxCollider2D _boxCollider;

    [Header("Movement")]
    [SerializeField] private float _movementDirection;
    [SerializeField] private float _slideDirection;
    [SerializeField] private float _slidingCooldown;

    [Header("State properties")]
    [SerializeField] private StateProperties _currentState;
    [SerializeField] private StateProperties _fullHeightState;
    [SerializeField] private StateProperties _crouchState;
    [SerializeField] private StateProperties _slideState;

    [Header("Layers")]
    [SerializeField] private LayerMask _groundLayer;

    [Header("Ground check")]
    [SerializeField] private BoxCastUtil _groundCheckBoxCast;

    [Header("Wall check")]
    [SerializeField] private BoxCastUtil _wallCheckBoxCast;

    [Header("Ceiling check")]
    [SerializeField] private BoxCastUtil _ceilingCheckBoxCast;

    [Header("Slope check")]
    [SerializeField] private float _slopeCheckDistanceLeft;
    [SerializeField] private float _slopeCheckDistanceRight;
    [SerializeField] private float _slopeAngle;
    [SerializeField] private Vector2 _slopeNormalPerpendicular;
    [SerializeField] private PhysicsMaterial2D _noFriction;
    [SerializeField] private PhysicsMaterial2D _fullFriction;
    private RaycastUtil _slopeCheckRaycast;

    private Coroutine _waitForMaxSlidingTime;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        #region Initialization
        _playerStats = GetComponent<PlayerStats>();
        _playerFlags = GetComponent<PlayerFlags>();
        _playerAnimationsController = GetComponent<PlayerAnimationsController>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();

        _playerFlags.IsFaceToTheRight = true;
        //_isJumpCooldownComplete = true;
        //_isSlidingBlocked = false;
        #endregion
    }

    private void FixedUpdate()
    {
        GroundCheck();
        SlopeCheck();
        RiseCheck();
        FallCheck();
        WallInFrontCheck();
        CeilingCheck();
        SlideCheck();

        _playerFlags.ChangeBlockFlagsState();

        Move();

        SetFriction();
        SetVelocity();
        SetColliderSize();
    }

    private void Update()
    {
        ReadMove();
        ReadJump();
        ReadRun();
        ReadCrouch();
        ReadSlide();

        StopMovement();

        _playerAnimationsController.SelectAnimation();
    }

    #region Read movement
    private void ReadMove()
    {
        bool keyA = Input.GetKey(KeyCode.A);
        bool keyD = Input.GetKey(KeyCode.D);

        if (keyA)
        {
            _movementDirection = -1;
            if (_playerFlags.IsFaceToTheRight)
            {
                Flip();
            }
        }
        else if (keyD)
        {
            _movementDirection = 1;
            if (!_playerFlags.IsFaceToTheRight)
            {
                Flip();
            }
        }
        else
        {
            _movementDirection = 0;
        }

        _playerFlags.IsIdle = !keyA && !keyD;
        _playerFlags.IsWalk = keyA || keyD;
    }
    
    private void ReadJump()
    {
        if (_playerFlags.IsJumpBlocked)
        {
            return;
        }

        if (_playerFlags.IsGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            _playerFlags.IsRise = true;
            _rigidbody.velocity = Vector2.up * _playerStats.JumpForce;
        }

        if (Input.GetKeyUp(KeyCode.Space) && _rigidbody.velocity.y > 0)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y * 0.5f);
        }
    }

    private void ReadRun()
    {
        if (_playerFlags.IsWalk && !_playerFlags.IsWallInFront && !_playerFlags.IsCrouch && Input.GetKey(KeyCode.LeftShift))
        {
            _playerFlags.IsWalk = false;
            _playerFlags.IsRun = true;
        }
        else
        {
            _playerFlags.IsRun = false;
        }
    }

    private void ReadCrouch()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            _playerFlags.IsCrouch = true;
        }
        else
        {
            if (_playerFlags.IsTouchCeiling)
            {
                return;
            }
            else
            {
                _playerFlags.IsCrouch = false;
            }
        }
    }

    private void ReadSlide()
    {
        if (_playerFlags.IsRun && !_playerFlags.IsRise && !_playerFlags.IsFall && !_playerFlags.IsSlide && Input.GetKeyDown(KeyCode.C))
        {
            _playerFlags.IsStartSlide = true;
            _playerFlags.IsCancelSlideBlocked = true;
            StartCoroutine(WaitForMinSlideTime());
            _waitForMaxSlidingTime = StartCoroutine(WaitForMaxSlideTime());
            _slideDirection = _movementDirection;
        }

        if (_playerFlags.IsSlide && !Input.GetKey(KeyCode.C) && !_playerFlags.IsCancelSlideBlocked)
        {
            _playerFlags.IsEndSlide = true;
            StopCoroutine(_waitForMaxSlidingTime);
        }
    }
    #endregion

    #region Apply movement
    private void Move()
    {
        if (_playerFlags.IsMovementBlocked)
        {
            _rigidbody.velocity = Vector2.zero;
            return;
        }

        float xSpeed = _playerStats.WalkingSpeed;
        if (_playerFlags.IsRun)
        {
            xSpeed = _playerStats.RunningSpeed;
        }
        if (_playerFlags.IsCrouch)
        {
            xSpeed = _playerStats.CrouchWalkingSpeed;
        }
        if (_playerFlags.IsSlide)
        {
            xSpeed = _playerStats.SlidingSpeed;
        }

        float currentMovementDirection = _playerFlags.IsSlide ? _slideDirection : _movementDirection;

        if (_playerFlags.IsGrounded && !_playerFlags.IsOnSlope && !_playerFlags.IsRise && _rigidbody.velocity.y >= -0.05f)
        {
            _rigidbody.velocity = new Vector2(xSpeed * currentMovementDirection, 0.0f);
        }
        else if (_playerFlags.IsGrounded && _playerFlags.IsOnSlope && !_playerFlags.IsRise)
        {
            _rigidbody.velocity = new Vector2(xSpeed * _slopeNormalPerpendicular.x * -currentMovementDirection, xSpeed * _slopeNormalPerpendicular.y * -currentMovementDirection);
        }
        else if (!_playerFlags.IsGrounded)
        {
            _rigidbody.velocity = new Vector2(xSpeed * currentMovementDirection, _rigidbody.velocity.y);
        }
    }
    #endregion

    #region Slide
    private IEnumerator WaitForMinSlideTime()
    {
        yield return new WaitForSeconds(_playerStats.SlidingMinTime);
        _playerFlags.IsCancelSlideBlocked = false;
    }

    private IEnumerator WaitForMaxSlideTime()
    {
        yield return new WaitForSeconds(_playerStats.SlidingMaxTime);
        if (_playerFlags.IsSlide)
        {
            _playerFlags.IsEndSlide = true;
        }
    }

    private void StartSlideComplete()
    {
        _playerFlags.IsStartSlide = false;
        _playerFlags.IsSlide = true;
    }

    private void TryStandUp()
    {
        if (_playerFlags.IsTouchCeiling)
        {
            EndSlideComplete();
            _playerFlags.IsCrouch = true;
            _playerAnimationsController.SelectAnimation();
        }
    }

    private void EndSlideComplete()
    {
        _playerFlags.IsEndSlide = false;
        StopSlide();
    }

    private void StopSlide()
    {
        _playerFlags.IsSlide = false;
        _slideDirection = 0;
    }
    #endregion

    #region Checks
    private void GroundCheck()
    {
        Vector3 origin = _boxCollider.bounds.center;
        origin.y -= _boxCollider.bounds.extents.y;

        _groundCheckBoxCast.BoxCast(origin);
        _playerFlags.IsGrounded = _groundCheckBoxCast.Result;
    }

    private void RiseCheck()
    {
        if (_rigidbody.velocity.y <= 0)
        {
            _playerFlags.IsRise = false;
        }
    }

    private void FallCheck()
    {
        _playerFlags.IsFall = _rigidbody.velocity.y < 0.1f && !_playerFlags.IsGrounded && !_playerFlags.IsOnSlope;
        if (_playerFlags.IsFall)
        {
            _playerFlags.IsRise = false;
            StopSlide();
        }
    }

    private void SlopeCheck()
    {
        if (_playerFlags.IsSlopeCheckBlocked)
        {
            _playerFlags.IsOnSlope = false;
            return;
        }

        Vector2 checkPosistion = _boxCollider.bounds.center - new Vector3(0, _boxCollider.bounds.extents.y);

        RaycastHit2D slopeHitFront = _slopeCheckRaycast.Raycast(
            checkPosistion,
            transform.right,
            _slopeCheckDistanceRight,
            _groundLayer,
            Color.cyan,
            Color.red);

        RaycastHit2D slopeHitBack = _slopeCheckRaycast.Raycast(
            checkPosistion,
            -transform.right,
            _slopeCheckDistanceLeft,
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

        _playerFlags.IsOnSlope = _slopeAngle != 0f && _slopeAngle != 90;

        if (_playerFlags.IsOnSlope)
        {
            _playerFlags.IsGrounded = true;
        }
    }

    private void WallInFrontCheck()
    {
        Vector3 origin = _boxCollider.bounds.center;
        origin.y += _boxCollider.bounds.extents.y;
        origin.x += _boxCollider.bounds.extents.x * transform.right.x;
        _wallCheckBoxCast.Distance = _currentState.WallInFrontCheckDistance;

        _wallCheckBoxCast.BoxCast(origin);
        _playerFlags.IsWallInFront = _wallCheckBoxCast.Result;
    }

    private void CeilingCheck()
    {
        Vector3 origin = _boxCollider.bounds.center;
        origin.y += _boxCollider.bounds.extents.y;

        _ceilingCheckBoxCast.BoxCast(origin);
        _playerFlags.IsTouchCeiling = _ceilingCheckBoxCast.Result;
    }

    private void SlideCheck()
    {
        if (_playerFlags.IsSlide && _rigidbody.velocity.y > 0f)
        {
            _playerFlags.IsEndSlide = true;
        }
    }
    #endregion

    #region Others
    private void Flip()
    {
        if (_playerFlags.IsFlipBlocked)
        {
            return;
        }
        
        _playerFlags.IsFaceToTheRight = !_playerFlags.IsFaceToTheRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void SetFriction()
    {
        if ((_movementDirection == 0 && _slideDirection == 0) || _playerFlags.IsMovementBlocked)
        {
            _rigidbody.sharedMaterial = _fullFriction;
        }
        else
        {
            _rigidbody.sharedMaterial = _noFriction;
        }
    }

    private void SetVelocity()
    {
        if (_rigidbody.velocity.y > 0 && _playerFlags.IsOnSlope && !_playerFlags.IsRise)
        {
            _rigidbody.velocity *= 1.5f;
        }
    }

    private void SetColliderSize()
    {
        _currentState = _fullHeightState;
        if (_playerFlags.IsCrouch)
        {
            _currentState = _crouchState;
        }

        if (_playerFlags.IsSlide)
        {
            _currentState = _slideState;
        }
        _boxCollider.size = _currentState.ColliderSize;
        _boxCollider.offset = _currentState.ColliderOffset;
    }

    private void StopMovement()
    {
        if (_playerFlags.IsWallInFront)
        {
            _playerFlags.IsWalk = false;
            _playerFlags.IsRun = false;
            //if (_playerFlags.IsSlide)
            //{
            //    _playerFlags.IsEndSlide = true;
            //}
            _playerFlags.IsIdle = true;
            _movementDirection = 0;
        }
    }
    #endregion

    #endregion
}
