using UnityEngine;

namespace SavageWorld.Runtime.Entities.NPC
{
    public class Slime : NPCBase
    {
        #region Fields
        [SerializeField]
        private float _jumpCooldown = 2f;
        [SerializeField]
        private float _currentJumpCooldown;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods
        private void OnEnable()
        {
            _physic.Grounded += GroundedEventHandler;
        }

        private void OnDisable()
        {
            _physic.Grounded -= GroundedEventHandler;
        }

        private void FixedUpdate()
        {
            _currentJumpCooldown += Time.fixedDeltaTime;
            if (_flags.IsIdle && _currentJumpCooldown >= _jumpCooldown)
            {
                if (_target != null)
                {
                    _moveDirection = _target.position.x - transform.position.x < 0 ? -1 : 1;
                }
                else
                {
                    _moveDirection = Random.Range(-1, 2);
                }
                Move();
            }
            _physic.Move(_stats.WalkingSpeed);
        }
        #endregion

        #region Public Methods
        public override void Move()
        {
            _physic.Jump(_stats.JumpForce, false);
            _physic.SetMoveDirection(_moveDirection);
        }
        #endregion

        #region Private Methods
        private void GroundedEventHandler()
        {
            _physic.SetMoveDirection(0);
            _currentJumpCooldown = 0f;
        }
        #endregion
    }
}
