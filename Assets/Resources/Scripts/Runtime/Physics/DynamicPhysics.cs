using SavageWorld.Runtime.Attributes;
using UnityEngine;

namespace SavageWorld.Runtime.Physics
{
    public class DynamicPhysics : BasePhysics
    {
        #region Fields

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
        }
        #endregion

        #region Public Methods

        #endregion

        #region Private Methods
        private void FixVelocity()
        {
            float xVelocity = _rigidbody.velocity.x;
            float yVelocity = _rigidbody.velocity.y;
            if (yVelocity < 0)
            {
                _rigidbody.velocity = new(xVelocity, Mathf.Max(yVelocity, _maxFallingSpeed));
            }
        }

        [Button("Jump left")]
        private void JumpLeft()
        {
            Jump();
            MoveLeft();
        }

        [Button("Jump right")]
        private void JumpRight()
        {
            Jump();
            MoveRight();
        }

        [Button("Jump")]
        private void Jump()
        {
            if (_isGrounded)
            {
                _rigidbody.velocity = new(_rigidbody.velocity.x, _ySpeed * Time.fixedDeltaTime);
            }
        }

        [Button("Move left")]
        private void MoveLeft()
        {
            _rigidbody.velocity = new(-_xSpeed * Time.fixedDeltaTime, _rigidbody.velocity.y);
        }

        [Button("Move right")]
        private void MoveRight()
        {
            _rigidbody.velocity = new(_xSpeed * Time.fixedDeltaTime, _rigidbody.velocity.y);
        }
        #endregion
    }
}