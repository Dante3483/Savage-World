using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SlimeNPC : NPC
{
    #region Private fields
    [Header("Slime")]
    [SerializeField] private float _jumpCooldown;
    [SerializeField] private float _attackCooldown;
    [SerializeField] private float _jumpXForce;
    [SerializeField] private float _attackXForce;

    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        base.Awake();
        Target = GameManager.Instance.GetPlayerTransform();
    }
    private void FixedUpdate()
    {
        GroundCheck();
        SlopeCheck();
        RiseCheck();
        FallCheck();

        _npcFlags.IsMovementBlocked = _npcFlags.IsGrounded && !_npcFlags.IsRise;

        Jump();

        SetFriction();
    }

    public override void Jump()
    {
        DirectionalJump(_jumpXForce);
        SetJumpCooldown();
    }

    public override void Attack()
    {
        DirectionalJump(_attackXForce);
        SetJumpCooldown();
    }

    private void DirectionalJump(float horizontalForce)
    {
        if (_npcFlags.IsJumpBlocked)
        {
            return;
        }
        if (transform.position.x - Target.position.x < 0)
        {
            _movementDirection = 1;
        }
        else
        {
            _movementDirection = -1;
        }
        _rigidbody.velocity = new Vector2(_movementDirection * horizontalForce, _npcStats.JumpForce);
        _npcFlags.IsRise = true;
    }

    private void SetJumpCooldown()
    {
        if (!_npcFlags.IsJumpBlocked)
        {
            _npcFlags.IsJumpBlocked = true;
            StartCoroutine(JumpWait());
        }

        IEnumerator JumpWait()
        {
            yield return new WaitForSeconds(_jumpCooldown);
            _npcFlags.IsJumpBlocked = false;
        }
    }

    #endregion
}
