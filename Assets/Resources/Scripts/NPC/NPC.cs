using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(NPCFlags))]
[RequireComponent(typeof(NPCStats))]
public class NPC : MonoBehaviour
{
    #region Private fields
    [Header("Main")]
    [SerializeField] protected Rigidbody2D _rigidbody;
    [SerializeField] protected BoxCollider2D _boxCollider;
    [SerializeField] protected NPCFlags _npcFlags;
    [SerializeField] protected NPCStats _npcStats;
    ///
    [Header("Ground check")]
    [SerializeField] private BoxCastUtil _groundCheckBoxCast;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _npcFlags = GetComponent<NPCFlags>();
        _npcStats = GetComponent<NPCStats>();
        _boxCollider= GetComponent<BoxCollider2D>();
        _npcFlags.IsFaceToTheRight =true;
        _npcFlags.IsIdle = true;
    }
        
    public virtual void Move()
    {

    }

    public virtual void Jump(Vector2 jumpDir)
    {

        _rigidbody.velocity = jumpDir * _npcStats.JumpForce;
    }

    public virtual void Attack()
    {

    }

    public virtual void SetAnimation()
    {

    }
    protected void GroundCheck()
    {
        Vector3 origin = _boxCollider.bounds.center;
        origin.y -= _boxCollider.bounds.extents.y;
        _groundCheckBoxCast.BoxCast(origin, out bool result, true);
        _npcFlags.IsGrounded = result;
    }
    protected void RisingCheck()
    {
        if (_rigidbody.velocity.y <= 0)
        {
            _npcFlags.IsRise = false;
        }
    }

    protected void FallingCheck()
    {
        _npcFlags.IsFall = _rigidbody.velocity.y < 0.1f && !_npcFlags.IsGrounded; //&& !_npcFlags.IsOnSlope;
        if (_npcFlags.IsFall)
        {
            _npcFlags.IsRise = false;
        }
    }
    #endregion
}
