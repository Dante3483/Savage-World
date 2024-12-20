using SavageWorld.Runtime.Attributes;
using UnityEngine;

namespace SavageWorld.Runtime.Physics
{
    public class DynamicPhysics : BasePhysics
    {
        #region Fields
        private int _moveDirection;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods
        public override void Awake()
        {
            base.Awake();
            _rigidbody.freezeRotation = true;
            _rigidbody.gravityScale = -Physics2D.gravity.y;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            FixVelocity();
            SetFriction();
        }
        #endregion

        #region Public Methods
        public void SetMoveDirection(int value)
        {
            _moveDirection = value;
            if (_flags.IsFaceToTheRight && _moveDirection == -1)
            {
                Flip();
            }
            else if (!_flags.IsFaceToTheRight && _moveDirection == 1)
            {
                Flip();
            }
            _flags.IsIdle = _moveDirection == 0;
            _flags.IsWalk = _moveDirection != 0;
        }

        public void Move(float speed)
        {
            Vector2 newVelocity = _rigidbody.linearVelocity;
            if (_flags.IsGrounded && !_flags.IsOnSlope && !_flags.IsRise && newVelocity.y >= -0.05f)
            {
                newVelocity.x = speed * _moveDirection;
                newVelocity.y = 0.0f;
            }
            else if (_flags.IsGrounded && _flags.IsOnSlope && !_flags.IsRise)
            {
                newVelocity.x = speed * _slopeNormalPerpendicular.x * -_moveDirection;
                newVelocity.y = speed * _slopeNormalPerpendicular.y * -_moveDirection;
            }
            else if (!_flags.IsGrounded)
            {
                newVelocity.x = speed * _moveDirection;
            }
            _rigidbody.linearVelocity = newVelocity;
        }

        public void Jump(float speed, bool stop)
        {
            if (!stop && _flags.IsGrounded)
            {
                _flags.IsRise = true;
                _rigidbody.linearVelocity = new(_rigidbody.linearVelocity.x, speed);
            }
            else if (stop && _rigidbody.linearVelocity.y > 0)
            {
                _rigidbody.linearVelocity = new(_rigidbody.linearVelocity.x, _rigidbody.linearVelocity.y * 0.5f);
            }
        }

        public void Run(bool stop)
        {
            if (stop)
            {
                _flags.IsRun = false;
            }
            else
            {
                _flags.IsRun = _flags.IsWalk && !_flags.IsWallInFront && !_flags.IsCrouch && !_flags.IsRise && !_flags.IsFall;
            }
        }

        public Vector2 GetVelocity()
        {
            return _rigidbody.linearVelocity;
        }
        #endregion

        #region Private Methods
        private void FixVelocity()
        {
            float xVelocity = _rigidbody.linearVelocity.x;
            float yVelocity = _rigidbody.linearVelocity.y;
            if (yVelocity < 0)
            {
                _rigidbody.linearVelocity = new(xVelocity, Mathf.Max(yVelocity, _maxFallingSpeed));
            }
            else if (yVelocity > 0 && _flags.IsOnSlope && !_flags.IsRise)
            {
                _rigidbody.linearVelocity *= 1.5f;
            }
        }

        private void SetFriction()
        {
            if (_moveDirection == 0 || _flags.IsMovementBlocked)
            {
                SetMaterial(_fullFriction);
            }
            else
            {
                SetMaterial(_noFriction);
            }
        }

        [Button("Debug")]
        public void DebugBtn()
        {
            Debug.Log("Button clicked");
        }
        #endregion
    }
}