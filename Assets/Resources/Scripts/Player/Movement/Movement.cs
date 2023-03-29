using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

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
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private Vector2 _groundCheckSize;
    [SerializeField] private Vector2 _groundCheckCenterOffset;
    [SerializeField] private float _extraWidth;
    [SerializeField] private Color _isGroundedColor;
    [SerializeField] private Color _isNotGroundedColor;

    [Header("Moving")]
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _staminaDecreaseRun;
    [SerializeField] private float _staminaIncrease;

    [Header("Jumping")]
    [SerializeField] private float _jumpForce;

    [Header("Hit")]
    [SerializeField] private float _unHitTime;
    [SerializeField] private float _hitCooldownTime;
    [SerializeField] private float _hitStrength = 8f;
    [SerializeField] private float _blinkingSpeed;

    [Header("Flags")]
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isJumping;
    [SerializeField] private bool _isFalling;
    [SerializeField] private bool _isWalking;
    [SerializeField] private bool _isRunning;
    [SerializeField] private bool _isFacingRight;
    [SerializeField] private bool _isHit;
    [SerializeField] private bool _isHitCooldown;
    [SerializeField] private bool _isBlinking;
    #endregion

    #region Public Fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _player = GetComponent<Player>();
    }

    // Start is called before the first frame update
    void Start()
    {
    	if (World.IsGameLoaded)
        {
            Rigidbody.bodyType = RigidbodyType2D.Static;
        }
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

    // Update is called once per frame
    private void Update()
    {
        _isGrounded = GroundCheck();
        _isJumping = JumpingCheck();
        _isFalling = FallingCheck();

        if (_isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            _rigidbody.velocity = Vector2.up * _jumpForce;
        }

        if (Input.GetKeyUp(KeyCode.Space) && _rigidbody.velocity.y > 0)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y * 0.5f);
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (WalkingCheck())
            {
                _isWalking = false;
                _isRunning = true;
            }
            else
            {
                _isWalking = true;
                _isRunning = false;
            }
        }
        else
        {
            _isWalking = true;
            _isRunning = false;
        }
        float speed = 0f;

        if (_isWalking)
        {
            _player.AddStamina(_staminaIncrease);
            speed = _walkSpeed;
        }
        if (_isRunning)
        {
            _player.RemoveStamina(_staminaDecreaseRun);
            if (_player.CanUseStamina())
            {
                speed = _runSpeed;
            }
            else
            {
                speed = _walkSpeed;
            }
        }

        if (!_isHit)
        {
            if (Input.GetKey(KeyCode.A))
            {
                if (_isFacingRight)
                {
                    Flip();
                }
                _rigidbody.velocity = new Vector2(-speed, _rigidbody.velocity.y);
            }
            else
            {
                if (Input.GetKey(KeyCode.D))
                {
                    if (!_isFacingRight)
                    {
                        Flip();
                    }
                    _rigidbody.velocity = new Vector2(speed, _rigidbody.velocity.y);
                }
                else
                {
                    // No keys pressed
                    // If is not jumping or is not falling freeze X posiotion and set X velocity to 0
                    if (!_isJumping || !_isFalling)
                    {
                        _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
                        if (_isGrounded)
                        {
                            _rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                        }
                    }
                }
            }
        }
    }

    private void UpdateData()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
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
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            _rigidbody.velocity = direction.normalized * _hitStrength;

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
    private bool GroundCheck()
    {
        Vector2 center = _capsuleCollider.bounds.center;
        center += _groundCheckCenterOffset;

        RaycastHit2D raycastHit = Physics2D.BoxCast(center, _groundCheckSize, 0f, Vector2.down, _extraWidth, _groundLayerMask);

        Color rayColor;
        if (raycastHit.collider != null)
        {
            rayColor = _isGroundedColor;
        }
        else
        {
            rayColor = _isNotGroundedColor;
        }

        Vector2 halfSize = _groundCheckSize / 2f;
        Vector2 centerForDebug = new Vector2(center.x, center.y + halfSize.y);

        Debug.DrawRay(centerForDebug + new Vector2(halfSize.x, 0), Vector2.down * (_groundCheckSize.y + _extraWidth), rayColor);
        Debug.DrawRay(centerForDebug - new Vector2(halfSize.x, 0), Vector2.down * (_groundCheckSize.y + _extraWidth), rayColor);
        Debug.DrawRay(centerForDebug - new Vector2(halfSize.x, _groundCheckSize.y + _extraWidth), Vector2.right * _groundCheckSize.x, rayColor);
        Debug.DrawRay(centerForDebug - new Vector2(halfSize.x, 0), Vector2.right * _groundCheckSize.x, rayColor);

        return raycastHit.collider != null;
    }

    private bool JumpingCheck()
    {
        return _rigidbody.velocity.y > 0 && !_isGrounded;
    }

    private bool FallingCheck()
    {
        return _rigidbody.velocity.y < 0 && !_isGrounded;
    }

    private bool WalkingCheck()
    {
        return Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
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
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }
    #endregion

    #endregion
}
