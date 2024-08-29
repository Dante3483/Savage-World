using SavageWorld.Runtime.NPC.Attack;
using SavageWorld.Runtime.Utilities.Raycasts;
using UnityEngine;

namespace SavageWorld.Runtime.NPC
{
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
        [SerializeField] protected float _movementDirection;
        [SerializeField] protected SpriteRenderer _spriteRenderer;

        [Header("Layers")]
        [SerializeField] private LayerMask _groundLayer;

        [Header("Ground check")]
        [SerializeField] private BoxCastUtil _groundCheckBoxCast;

        [Header("Slope check")]
        [SerializeField] private float _slopeCheckDistanceLeft;
        [SerializeField] private float _slopeCheckDistanceRight;
        [SerializeField] private float _slopeAngle;
        [SerializeField] private Vector2 _slopeNormalPerpendicular;
        [SerializeField] private PhysicsMaterial2D _noFriction;
        [SerializeField] private PhysicsMaterial2D _fullFriction;
        private RaycastUtil _slopeCheckRaycast;

        [Header("Attack properties")]
        [SerializeField] private GameObject _attackCollider;
        [SerializeField] private GameObject _hitCollider;
        [SerializeField] private Transform _target;


        #endregion

        #region Public fields

        #endregion

        #region Properties
        public Transform Target
        {
            get
            {
                return _target;
            }

            set
            {
                _target = value;
            }
        }

        public NPCFlags NpcFlags
        {
            get
            {
                return _npcFlags;
            }

            set
            {
                _npcFlags = value;
            }
        }
        #endregion

        #region Methods
        public void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _npcFlags = GetComponent<NPCFlags>();
            _npcStats = GetComponent<NPCStats>();
            _boxCollider = GetComponent<BoxCollider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _npcFlags.IsFaceToTheRight = true;
            _npcFlags.IsIdle = true;
        }
        public void OnValidate()
        {
            if (!transform.Find("AttackCollider"))
            {
                _attackCollider = new GameObject("AttackCollider");
                _attackCollider.AddComponent<MobAttackController>();
                _attackCollider.transform.parent = transform;
                _attackCollider.transform.position = transform.position;
            }

            if (!transform.Find("HitCollider"))
            {
                _hitCollider = new GameObject("HitCollider");
                _hitCollider.AddComponent<PolygonCollider2D>();
                _hitCollider.GetComponent<PolygonCollider2D>().isTrigger = true;
                _hitCollider.AddComponent<MobHitController>();
                _hitCollider.transform.parent = transform;
                _hitCollider.transform.position = transform.position;
            }
        }
        public virtual void Move()
        {
            if (_npcFlags.IsMovementBlocked)
            {
                _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
                return;
            }

            float xSpeed = _npcStats.WalkingSpeed;

            float currentMovementDirection = _movementDirection;

            if (_npcFlags.IsGrounded && !_npcFlags.IsOnSlope && !_npcFlags.IsRise && _rigidbody.velocity.y >= -0.05f)
            {
                _rigidbody.velocity = new Vector2(xSpeed * currentMovementDirection, 0.0f);
            }
            else if (_npcFlags.IsGrounded && _npcFlags.IsOnSlope && !_npcFlags.IsRise)
            {
                _rigidbody.velocity = new Vector2(xSpeed * _slopeNormalPerpendicular.x * -currentMovementDirection, xSpeed * _slopeNormalPerpendicular.y * -currentMovementDirection);
            }
            else if (!_npcFlags.IsGrounded)
            {
                _rigidbody.velocity = new Vector2(xSpeed * currentMovementDirection, _rigidbody.velocity.y);
            }
        }

        public virtual void Jump()
        {
            if (_npcFlags.IsJumpBlocked)
            {
                return;
            }

            _rigidbody.velocity = Vector2.up * _npcStats.JumpForce;
            _npcFlags.IsRise = true;
        }

        public virtual void Attack()
        {

        }

        public virtual void SetAnimation()
        {

        }

        protected void SetFriction()
        {
            if (_movementDirection == 0 || _npcFlags.IsMovementBlocked)
            {
                _rigidbody.sharedMaterial = _fullFriction;
            }
            else
            {
                _rigidbody.sharedMaterial = _noFriction;
            }
        }

        protected void GroundCheck()
        {
            Vector3 origin = _boxCollider.bounds.center;
            origin.y -= _boxCollider.bounds.extents.y;
            _groundCheckBoxCast.BoxCast(origin);
            _npcFlags.IsGrounded = _groundCheckBoxCast.Result;
        }

        protected void RiseCheck()
        {
            if (_rigidbody.velocity.y <= 0)
            {
                _npcFlags.IsRise = false;
            }
        }

        protected void FallCheck()
        {
            _npcFlags.IsFall = _rigidbody.velocity.y < 0.1f && !_npcFlags.IsGrounded; //&& !_npcFlags.IsOnSlope;
            if (_npcFlags.IsFall)
            {
                _npcFlags.IsRise = false;
            }
        }

        protected void SlopeCheck()
        {
            if (_npcFlags.IsSlopeCheckBlocked)
            {
                _npcFlags.IsOnSlope = false;
                return;
            }

            Vector2 checkPosistion = _boxCollider.bounds.center - new Vector3(0, _boxCollider.bounds.extents.y);

            RaycastHit2D slopeHitFront = _slopeCheckRaycast.Raycast(
                checkPosistion,
                transform.right,
                _slopeCheckDistanceRight,
                _groundLayer,
                Color.cyan,
                Color.red,
                true);

            RaycastHit2D slopeHitBack = _slopeCheckRaycast.Raycast(
                checkPosistion,
                -transform.right,
                _slopeCheckDistanceLeft,
                _groundLayer,
                Color.cyan,
                Color.red,
                true);

            if (slopeHitFront)
            {
                _slopeAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
                _slopeNormalPerpendicular = Vector2.Perpendicular(slopeHitFront.normal).normalized;

                if (true)
                {
                    Debug.DrawRay(slopeHitFront.point, _slopeNormalPerpendicular, Color.blue);
                    Debug.DrawRay(slopeHitFront.point, slopeHitFront.normal, Color.red);
                }
            }
            else if (slopeHitBack)
            {
                _slopeAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
                _slopeNormalPerpendicular = Vector2.Perpendicular(slopeHitBack.normal).normalized;

                if (true)
                {
                    Debug.DrawRay(slopeHitBack.point, _slopeNormalPerpendicular, Color.blue);
                    Debug.DrawRay(slopeHitBack.point, slopeHitBack.normal, Color.red);
                }
            }
            else
            {
                _slopeAngle = 0.0f;
            }

            _npcFlags.IsOnSlope = _slopeAngle != 0f && _slopeAngle != 90;

            if (_npcFlags.IsOnSlope)
            {
                _npcFlags.IsGrounded = true;
            }
        }
        #endregion
    }
}