using SavageWorld.Runtime.Attributes;
using SavageWorld.Runtime.Physics;
using UnityEngine;

namespace SavageWorld.Runtime.NPC
{
    [RequireComponent(typeof(DynamicPhysics))]
    public class TestNPCMovement : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private float _xSpeed = 5;
        [SerializeField]
        private float _ySpeed = 5;
        private DynamicPhysics _physics;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods
        private void Awake()
        {
            _physics = GetComponent<DynamicPhysics>();
        }

        private void FixedUpdate()
        {
            _physics.Move(_xSpeed);
        }
        #endregion

        #region Public Methods

        #endregion

        #region Private Methods
        [Button("Stop move")]
        private void StopMove()
        {
            _physics.SetMoveDirection(0);
        }

        [Button("Move left")]
        private void MoveLeft()
        {
            _physics.SetMoveDirection(-1);
        }

        [Button("Move right")]
        private void MoveRight()
        {
            _physics.SetMoveDirection(1);
        }

        [Button("Jump")]
        private void Jump()
        {
            _physics.Jump(_ySpeed, false);
        }

        [Button("Left jump")]
        private void LeftJump()
        {
            Jump();
            MoveLeft();
            StopMove();
        }

        [Button("Right jump")]
        private void RightJump()
        {
            Jump();
            MoveRight();
            StopMove();
        }
        #endregion
    }
}
