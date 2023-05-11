using System;
using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{
    #region Private Fields
    [Header("Main Properties")]
    [SerializeField] private bool _needUpdate;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private CapsuleCollider2D _capsuleCollider;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Player _player;

    [Header("Ground Checking")]
    [SerializeField] private CheckingAreaUtil _checkingGround;

    [Header("Moving")]
    [SerializeField] private int _moveDirection;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _staminaDecreaseRun;
    [SerializeField] private float _staminaIncrease;

    [Header("Slope")]
    [SerializeField] private Vector2 _capsuleColliderSize;
    [SerializeField] private Vector2 _slopeNormalPerpendicular;
    [SerializeField] private float _slopeCheckDistance;
    [SerializeField] private float _slopeDownAngle;
    [SerializeField] private float _slopeDownAnglePrev;
    [SerializeField] private float _slopeSideAngle;
    [SerializeField] private PhysicsMaterial2D _noFriction;
    [SerializeField] private PhysicsMaterial2D _fullFriction;

    [Header("Jumping")]
    [SerializeField] private float _jumpForce;

    [Header("Hit")]
    [SerializeField] private float _unHitTime;
    [SerializeField] private float _hitCooldownTime;
    [SerializeField] private float _hitStrength = 8f;
    [SerializeField] private float _blinkingSpeed;

    [Header("Flags")]
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isIdle;
    [SerializeField] private bool _isJumping;
    [SerializeField] private bool _isFalling;
    [SerializeField] private bool _isWalking;
    [SerializeField] private bool _isRunning;
    [SerializeField] private bool _isFacingRight;
    [SerializeField] private bool _isHit;
    [SerializeField] private bool _isHitCooldown;
    [SerializeField] private bool _isBlinking;
    [SerializeField] private bool _isOnSlope;
    [SerializeField] private bool _isLShiftDown;
    #endregion

    #region Public Fields

    #endregion

    #region Properties
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
    #endregion

    #region Methods
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _player = GetComponent<Player>();
    }

    void Start()
    {
        UpdateData();
    }

    private void OnValidate()
    {
        if (_needUpdate)
        {
            UpdateData();
            _needUpdate = !_needUpdate;
        }
    }

    private void Update()
    {
        SetMoving();
        GroundCheck();
        JumpingCheck();
        FallingCheck();

        if (_isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            _isJumping = true;
            Rigidbody.velocity = Vector2.up * _jumpForce;
        }

        if (Input.GetKeyUp(KeyCode.Space) && Rigidbody.velocity.y > 0)
        {
            Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, Rigidbody.velocity.y * 0.5f);
        }
    }

    private void FixedUpdate()
    {
        SlopeCheck();
        Move();
        AnimateMovement();
    }

    private void SlopeCheck()
    {
        Vector2 checkPosistion = transform.position - (Vector3)(new Vector2(_capsuleColliderSize.x / 2f * -_moveDirection, _capsuleColliderSize.y / 2));

        SlopeCheckHorizontal(checkPosistion);
        SlopeCheckVertical(checkPosistion);
    }

    private void SlopeCheckVertical(Vector2 checkPosistion)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPosistion, Vector2.down, _slopeCheckDistance, _checkingGround.TargetLayer);
        if (hit)
        {
            _slopeNormalPerpendicular = Vector2.Perpendicular(hit.normal).normalized;

            _slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (_slopeDownAngle > 0)
            {
                _isOnSlope = true;
            }

            _slopeDownAnglePrev = _slopeDownAngle;

            Debug.DrawRay(hit.point, _slopeNormalPerpendicular, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }
        else
        {
            _slopeDownAngle = 0;
        }

        if (_slopeSideAngle == 90)
        {
            _isOnSlope = false;
        }

        if (_isOnSlope)
        {
            Rigidbody.gravityScale = 0f;
        }
        else
        {
            Rigidbody.gravityScale = 9f;
        }
    }

    private void SlopeCheckHorizontal(Vector2 checkPosistion)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPosistion, transform.right, _slopeCheckDistance, _checkingGround.TargetLayer);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPosistion, -transform.right, _slopeCheckDistance, _checkingGround.TargetLayer);
        if (slopeHitFront)
        {
            _isOnSlope = true;
            _slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if (slopeHitBack)
        {
            _isOnSlope = true;
            _slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            _isOnSlope = false;
            _slopeSideAngle = 0.0f;
        }
    }

    private void SetMoving()
    {
        _isIdle = false;
        _isWalking = false;

        if (_isHit)
        {
            _moveDirection = 0;
            return;
        }

        if (Input.GetKey(KeyCode.A))
        {
            Rigidbody.sharedMaterial = _noFriction;
            _isWalking = true;
            _moveDirection = -1;
            if (_isFacingRight)
            {
                Flip();
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Rigidbody.sharedMaterial = _noFriction;
            _isWalking = true;
            _moveDirection = 1;
            if (!_isFacingRight)
            {
                Flip();
            }
        }
        else
        {
            Rigidbody.sharedMaterial = _fullFriction;
            _isIdle = true;
            _moveDirection = 0;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _isLShiftDown = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _isLShiftDown = false;
        }

        if (!_isIdle && _isLShiftDown)
        {
            _isWalking = false;
            _isRunning = true;
        }
        else if (!_isLShiftDown)
        {
            _isRunning = false;
        }

        if (!_player.CanUseStamina())
        {
            _isLShiftDown = false;
            _isRunning = false;
        }
    }

    private void Move()
    {
        float speed = _isRunning ? _runSpeed : _walkSpeed;

        if (_isRunning)
        {
            _player.RemoveStamina(_staminaDecreaseRun);
        }
        else
        {
            _player.AddStamina(_staminaIncrease);
        }

        if (_isGrounded && !_isOnSlope && !_isJumping && _rigidbody.velocity.y >= -0.05f)
        {
            _rigidbody.velocity = new Vector2(speed * _moveDirection, 0.0f);
        }
        else if (_isGrounded && _isOnSlope && !_isJumping)
        {
            _rigidbody.velocity = new Vector2(speed * _slopeNormalPerpendicular.x * -_moveDirection * 1.5f, speed * _slopeNormalPerpendicular.y * -_moveDirection);
        }
        else if (!_isGrounded)
        {
            _rigidbody.velocity = new Vector2(speed * _moveDirection, _rigidbody.velocity.y);
        }
    }

    private void UpdateData()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _capsuleColliderSize = _capsuleCollider.size;
        _isFacingRight = true;
    }

    #region Hit
    public void Hit(Transform enemy)
    {
        if (!_isHit)
        {
            _isHit = true;
            _isHitCooldown = true;

            var direction = transform.position - enemy.position;
            Rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            Rigidbody.velocity = direction.normalized * _hitStrength;

            if (!_isBlinking)
            {
                StartCoroutine(FadeToWhite());
            }
            StartCoroutine(UnHit());
            StartCoroutine(HitCooldown());
        }
    }

    private IEnumerator UnHit()
    {
        yield return new WaitForSeconds(_unHitTime);
        _isHit = false;
    }

    private IEnumerator HitCooldown()
    {
        yield return new WaitForSeconds(_hitCooldownTime);
        _isHitCooldown = false;
    }

    private IEnumerator FadeToWhite()
    {
        float alphaBound = 0f;
        _isBlinking = true;
        while (_isHitCooldown)
        {
            yield return new WaitForFixedUpdate();
            if (_spriteRenderer.color.a <= 0.1f)
            {
                alphaBound = 1f;
            }
            if (_spriteRenderer.color.a >= 0.9f)
            {
                alphaBound = 0f;
            }
            float newAlpha = Mathf.Lerp(_spriteRenderer.color.a, alphaBound, Time.fixedDeltaTime * _blinkingSpeed);
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, newAlpha);
        }
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 1f);
        _isBlinking = false;
    }
    #endregion

    #region Checks
    private void GroundCheck()
    {
        _isGrounded = _checkingGround.CheckArea(transform.position, gameObject).Item1;
    }

    private void JumpingCheck()
    {
        if (Rigidbody.velocity.y <= 0)
        {
            _isJumping = false;
        }
    }

    private void FallingCheck()
    {
        _isFalling = Rigidbody.velocity.y < 0 && !_isGrounded && !_isOnSlope;
        if (_isFalling)
        {
            _isJumping = false;
        }
    }

    public bool CanNewHitCheck()
    {
        return _isHitCooldown;
    }
    #endregion

    #region Other
    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void AnimateMovement()
    {
        //Reset flags
        _player.Animator.SetBool("IsIdle", false);
        _player.Animator.SetBool("IsWalking", false);
        _player.Animator.SetBool("IsRunning", false);
        _player.Animator.SetBool("IsJumping", false);
        _player.Animator.SetBool("IsFalling", false);

        //Set flag
        if (_isIdle)
        {
            _player.Animator.SetBool("IsIdle", true);
        }
        if (_isWalking)
        {
            _player.Animator.SetBool("IsWalking", true);
        }
        if (_isRunning)
        {
            _player.Animator.SetBool("IsRunning", true);
        }
        if (_isJumping)
        {
            _player.Animator.SetBool("IsJumping", true);
        }
        if (_isFalling)
        {
            _player.Animator.SetBool("IsFalling", true);
        }
    }
    #endregion

    #endregion
}
