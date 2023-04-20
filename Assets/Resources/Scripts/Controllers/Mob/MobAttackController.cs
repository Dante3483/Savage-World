using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NPC;

public class MobAttackController : MonoBehaviour
{
    #region Private Fields
    [Header("Main Properties")]
    [SerializeField] private bool _updateData;
    [SerializeField] private NPC _npc;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private Vector2 _attackCheckSize;
    [SerializeField] private Vector2 _attackCheckCenterOffset;
    [SerializeField] private float _extraWidth;
    [SerializeField] private Color _isAttackingColor;
    [SerializeField] private Color _isNotAttackingColor;
    [SerializeField] private CheckingLineCast _groundCheck;

    #endregion

    #region Properties
    public NPC Npc
    {
        get
        {
            return _npc;
        }

        set
        {
            _npc = value;
        }
    }

    public LayerMask TargetLayer
    {
        get
        {
            return _targetLayer;
        }

        set
        {
            _targetLayer = value;
        }
    }

    public Vector2 AttackCheckSize
    {
        get
        {
            return _attackCheckSize;
        }

        set
        {
            _attackCheckSize = value;
        }
    }

    public Vector2 AttackCheckCenterOffset
    {
        get
        {
            return _attackCheckCenterOffset;
        }

        set
        {
            _attackCheckCenterOffset = value;
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

    public Color IsAttackingColor
    {
        get
        {
            return _isAttackingColor;
        }

        set
        {
            _isAttackingColor = value;
        }
    }

    public Color IsNotAttackingColor
    {
        get
        {
            return _isNotAttackingColor;
        }

        set
        {
            _isNotAttackingColor = value;
        }
    }
    #endregion

    #region Methods
    private void Start()
    {
        Npc = transform.parent.GetComponent<NPC>();
    }

    private void OnValidate()
    {
        if (_updateData)
        {
            CheckIfCanAttack();
            _updateData = !_updateData;
        }
    }


    private void FixedUpdate()
    {
        Vector3 startPosition = transform.parent.transform.position;
        Vector3 endPosition = (_npc as EnemiesMobs).Target.position;
        var result = _groundCheck.CheckLinecast(startPosition, endPosition);
        bool result2 = CheckIfCanAttack();
        if (!result.Item1 && result2)
        {
            (Npc as EnemiesMobs).IsTargetInAttackArea = true;
        }
        else
        {
            (Npc as EnemiesMobs).IsTargetInAttackArea = false;
        }
    }

    private bool CheckIfCanAttack()
    {
        Vector2 center = transform.parent.GetComponent<CapsuleCollider2D>().bounds.center;
        center += AttackCheckCenterOffset;
        RaycastHit2D raycastHit = Physics2D.BoxCast(center, AttackCheckSize, 0f, Vector2.down, ExtraWidth, TargetLayer);

        Color rayColor;
        if (raycastHit.collider != null)
        {
            rayColor = IsAttackingColor;
        }
        else
        {
            rayColor = IsNotAttackingColor;
        }

        Vector2 halfSize = AttackCheckSize / 2f;
        Vector2 centerForDebug = new Vector2(center.x, center.y + halfSize.y);

        Debug.DrawRay(centerForDebug + new Vector2(halfSize.x, 0), Vector2.down * (AttackCheckSize.y + ExtraWidth), rayColor);
        Debug.DrawRay(centerForDebug - new Vector2(halfSize.x, 0), Vector2.down * (AttackCheckSize.y + ExtraWidth), rayColor);
        Debug.DrawRay(centerForDebug - new Vector2(halfSize.x, AttackCheckSize.y + ExtraWidth), Vector2.right * AttackCheckSize.x, rayColor);
        Debug.DrawRay(centerForDebug - new Vector2(halfSize.x, 0), Vector2.right * AttackCheckSize.x, rayColor);

        return raycastHit.collider != null;
    }
    #endregion
}
