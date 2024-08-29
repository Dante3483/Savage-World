using SavageWorld.Runtime.Player.Movement;
using SavageWorld.Runtime.Utilities.Extensions;
using SavageWorld.Runtime.Utilities.Raycasts;
using System;
using UnityEngine;

namespace SavageWorld.Runtime.Physics
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class BasePhysics : MonoBehaviour
    {
        #region Fields
        [Header("Main")]
        [SerializeField]
        protected BoxCollider2D _boxCollider;
        [SerializeField]
        protected Rigidbody2D _rigidbody;
        [SerializeField]
        protected PhysicsMaterial2D _noFriction;
        [SerializeField]
        protected PhysicsMaterial2D _fullFriction;
        [SerializeField]
        protected PhysicsFlags _flags;
        [SerializeField]
        protected PlatformCreator _platformsCreator;
        private Vector2Int _currentTransformPosition;
        private Vector2Int _prevTransformPosition;

        [Space]
        [Header("Movement params")]
        [SerializeField]
        protected float _maxFallingSpeed = -30;
        [SerializeField]
        protected PhysicStateProperties _state;

        [Space]
        [Header("Checks")]
        [SerializeField]
        private LayerMask _groundLayer = 1 << 6;
        [SerializeField]
        private BoxCastUtil _groundCheckBoxCast = new();
        [SerializeField]
        private BoxCastUtil _wallCheckBoxCast = new();
        [SerializeField]
        private BoxCastUtil _ceilingCheckBoxCast = new();
        private RaycastUtil _slopeCheckRaycast;

        [Space]
        [Header("Slope params")]
        [SerializeField]
        protected float _slopeCheckDistanceLeft;
        [SerializeField]
        protected float _slopeCheckDistanceRight;
        [SerializeField]
        protected float _slopeAngle;
        [SerializeField]
        protected Vector2 _slopeNormalPerpendicular;
        #endregion

        #region Properties
        public PhysicsFlags Flags
        {
            get
            {
                return _flags;
            }
        }
        #endregion

        #region Events / Delegates
        public event Action EntityFall;
        #endregion

        #region Monobehaviour Methods
        public virtual void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _platformsCreator = new(_boxCollider.size);
            _flags.IsIdle = true;
            _flags.IsFaceToTheRight = true;
        }

        public virtual void FixedUpdate()
        {
            SetFlags();
            if (transform.hasChanged)
            {
                _currentTransformPosition = Vector2Int.FloorToInt(_boxCollider.bounds.center);
                _platformsCreator.Process(_boxCollider.bounds.center, _currentTransformPosition != _prevTransformPosition);
                _prevTransformPosition = _currentTransformPosition;
                transform.hasChanged = false;
            }
        }
        #endregion

        #region Public Methods
        public BasePhysics()
        {
            _groundCheckBoxCast.LayerMask = _groundLayer;

            _wallCheckBoxCast.LayerMask = _groundLayer;
            _wallCheckBoxCast.HitColor = ColorExtension.GetColor("FFDA00");
            _wallCheckBoxCast.NotHitColor = ColorExtension.GetColor("FF0088");
            _wallCheckBoxCast.Size = new(0.4f, 0.001f);
            _wallCheckBoxCast.Direction = Vector2.down;

            _ceilingCheckBoxCast.LayerMask = _groundLayer;
            _ceilingCheckBoxCast.HitColor = ColorExtension.GetColor("00FFE1");
            _ceilingCheckBoxCast.NotHitColor = ColorExtension.GetColor("9900FF");
            _ceilingCheckBoxCast.Size = new(1f, 0.001f);
            _ceilingCheckBoxCast.Direction = Vector2.up;
            _ceilingCheckBoxCast.Distance = 0.5f;

            _slopeCheckDistanceLeft = 1f;
            _slopeCheckDistanceRight = 1f;
        }

        public void SetState(PhysicStateProperties state)
        {
            _state = state;
            _boxCollider.size = _state.ColliderSize;
            _boxCollider.offset = _state.ColliderOffset;
        }
        #endregion

        #region Private Methods
        private void SetFlags()
        {
            CheckGrounding();
            CheckSlope();
            CheckRising();
            CheckFalling();
            CheckWallInFront();
            CheckCeil();
        }

        protected void Flip()
        {
            if (_flags.IsFlipBlocked)
            {
                return;
            }

            _flags.IsFaceToTheRight = !_flags.IsFaceToTheRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }

        protected void SetMaterial(PhysicsMaterial2D material)
        {
            _rigidbody.sharedMaterial = material;
        }

        private void CheckGrounding()
        {
            Vector3 origin = _boxCollider.bounds.center;
            origin.y -= _boxCollider.bounds.extents.y;
            _groundCheckBoxCast.SetSizeX(_boxCollider.size.x);
            _groundCheckBoxCast.BoxCast(origin);
            _flags.IsGrounded = _groundCheckBoxCast.Result;
        }

        private void CheckSlope()
        {
            if (_flags.IsSlopeCheckBlocked)
            {
                _flags.IsOnSlope = false;
                return;
            }

            Vector2 origin = _boxCollider.bounds.center - new Vector3(0, _boxCollider.bounds.extents.y);
            _slopeCheckDistanceRight = _boxCollider.size.x - 0.45f;
            _slopeCheckDistanceLeft = _boxCollider.size.x + 0.05f;

            RaycastHit2D slopeHitFront = _slopeCheckRaycast.Raycast(
                origin,
                transform.right,
                _slopeCheckDistanceRight,
                _groundLayer,
                Color.cyan,
                Color.red);

            RaycastHit2D slopeHitBack = _slopeCheckRaycast.Raycast(
                origin,
                -transform.right,
                _slopeCheckDistanceLeft,
                _groundLayer,
                Color.cyan,
                Color.red);

            if (slopeHitFront)
            {
                _slopeAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
                _slopeNormalPerpendicular = Vector2.Perpendicular(slopeHitFront.normal).normalized;
            }
            else if (slopeHitBack)
            {
                _slopeAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
                _slopeNormalPerpendicular = Vector2.Perpendicular(slopeHitBack.normal).normalized;
            }
            else
            {
                _slopeAngle = 0.0f;
            }

            _flags.IsOnSlope = _slopeAngle != 0f && _slopeAngle != 90;

            if (_flags.IsOnSlope)
            {
                _flags.IsGrounded = true;
            }
        }

        private void CheckRising()
        {
            if (_rigidbody.velocity.y <= 0)
            {
                _flags.IsRise = false;
            }
        }

        private void CheckFalling()
        {
            _flags.IsFall = _rigidbody.velocity.y < 0.1f && !_flags.IsGrounded && !_flags.IsOnSlope;
            if (_flags.IsFall)
            {
                _flags.IsRise = false;
                EntityFall?.Invoke();
            }
        }

        private void CheckWallInFront()
        {
            Vector3 origin = _boxCollider.bounds.center;
            origin.x += _boxCollider.bounds.extents.x * transform.right.x;
            _wallCheckBoxCast.Distance = _state.WallInFrontCheckDistance;
            _wallCheckBoxCast.SetOffsetY(_state.WallInFrontCheckDistance / 2f);
            _wallCheckBoxCast.BoxCast(origin);
            _flags.IsWallInFront = _wallCheckBoxCast.Result;
        }

        private void CheckCeil()
        {
            Vector3 origin = _boxCollider.bounds.center;
            origin.y += _boxCollider.bounds.extents.y;
            _ceilingCheckBoxCast.BoxCast(origin);
            _flags.IsTouchCeiling = _ceilingCheckBoxCast.Result;
        }
        #endregion
    }
}