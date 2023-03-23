using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    #region Private fields
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private int _horizontalValue;
    private int _verticalValue;
    private bool _facingRight;
    [SerializeField] private bool _nextInline;
    [SerializeField] private BoxCollider2D _groundCheckCollider;
    [SerializeField] private LayerMask _layers;

    //For test
    public Tilemap blocks;
    public float speedMult;
    #endregion

    #region Public fields
    public float Speed;
    public float SpeedMultiplier;
    public float JumpForce;
    public float XVelocity;
    public float YVelocity;
    public float Gravity;
    public float GravityScale;

    [Header("Hit Property")]
    public float UnHitTime;
    public float HitCooldownTime;
    public float HitStrength = 8f;

    [Header("Player Flags")]
    public bool IsGrounded;
    public bool IsJumping;
    public bool IsHit;
    public bool IsHitCooldown;

    #endregion

    #region Properties

    #endregion

    #region Methods
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _groundCheckCollider = GetComponent<BoxCollider2D>();
        _facingRight = true;
    }

    private void Update()
    {
        //Check if grounded
        if (_groundCheckCollider.IsTouchingLayers(_layers.value))
        {
            IsGrounded = true;
            IsJumping = false;
        }
        else
        {
            IsGrounded = false;
        }

        //Set gravity
        if (!IsGrounded)
        {
            if (!IsJumping)
            {
                _rigidbody.gravityScale = Gravity;
            }
            else
            {
                _rigidbody.gravityScale = Gravity * GravityScale;
            }
        }
        else
        {
            _rigidbody.gravityScale = 0f;
        }

        //Horizontal movement
        _horizontalValue = 0;
        if (Input.GetKey(KeyCode.A))
        {
            _horizontalValue = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            _horizontalValue = 1;
        }
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {
            _horizontalValue = 0;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            YVelocity = JumpForce;
            IsJumping = true;
            //_rigidbody.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    private void FixedUpdate()
    {
        if (!IsHit)
        {
            _nextInline = CheckInline();
            Move(_horizontalValue);
        }
    }

    private void Move(int direction)
    {
        //Move Left/Right
        float mult = !_nextInline ? 1f : speedMult;
        float xValue = Speed * direction * SpeedMultiplier * Time.fixedDeltaTime * mult;
        XVelocity = xValue;
        Vector2 targetPosition = transform.position;
        

        //Jumping
        float yValue = YVelocity * Time.fixedDeltaTime;
        if (YVelocity > 0f)
        {
            YVelocity += -Gravity * Time.fixedDeltaTime;
            xValue /= mult;
        }
        else
        {
            YVelocity = 0f;
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }

        targetPosition.x += xValue;
        targetPosition.y += yValue;

        _rigidbody.MovePosition(targetPosition);

        //Store the current scale value
        Vector3 currentScale = transform.localScale;

        //If looking right and clicked to the left
        if (_facingRight && direction < 0)
        {
            currentScale.x *= -1;
            _facingRight = false;
        }

        //If looking left and clicked to the right
        else if (!_facingRight && direction > 0)
        {
            currentScale.x *= -1;
            _facingRight = true;
        }
        transform.localScale = currentScale;
    }

    public bool CheckInline()
    {
        Vector3 newCenterPosition = transform.position;
        int direction = transform.localScale.x > 0f ? 1 : -1;
        newCenterPosition.x -= 0.5f * direction;

        newCenterPosition.y -= 1.75f;
        Vector3Int intPosition = blocks.WorldToCell(newCenterPosition);
        intPosition.x += direction;

        if (blocks.GetTile(intPosition) != null)
        {
            return true;
        }
        return false;
    }

    public void Hit(Transform enemy)
    {
        if (!IsHit)
        {
            var direction = transform.position - enemy.position;
            _rigidbody.velocity = direction.normalized * HitStrength;

            IsHit = true;
            IsHitCooldown = true;

            StartCoroutine(FadeToWhite());
            StartCoroutine(UnHit());
            StartCoroutine(HitCooldown());
        }
    }

    private IEnumerator UnHit()
    {
        yield return new WaitForSeconds(UnHitTime);
        IsHit = false;
    }

    private IEnumerator HitCooldown()
    {
        yield return new WaitForSeconds(HitCooldownTime);
        IsHitCooldown = false;
    }

    private IEnumerator FadeToWhite()
    {
        float alphaBound = 0f;
        while (IsHitCooldown)
        {
            yield return null;
            if (_spriteRenderer.color.a <= 0.1f)
            {
                alphaBound = 1f;
            }
            if (_spriteRenderer.color.a >= 0.9f)
            {
                alphaBound = 0f;
            }
            float newAlpha = Mathf.Lerp(_spriteRenderer.color.a, alphaBound, Time.fixedDeltaTime * 5f);
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, newAlpha);
        }
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 1f);
    }
    #endregion
}
