using SavageWorld.Runtime.Attributes;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SavageWorld.Runtime.Utilities.FiniteStateMachine.Actions
{
    [Serializable]
    [FSMComponent("Set direction", "")]
    public class SetDirectionAction : PhysicsActionBase
    {
        #region Fields
        [SerializeField]
        private bool _isRandom;
        [SerializeField]
        private int _direction;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override void Invoke()
        {
            if (_isRandom)
            {
                _physic.SetMoveDirection(Random.Range(0, 101) < 50 ? -1 : 1);
            }
            else
            {
                _physic.SetMoveDirection(_direction);
            }
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
