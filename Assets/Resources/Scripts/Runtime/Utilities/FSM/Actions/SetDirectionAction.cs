using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SavageWorld.Runtime.Utilities.FSM.Actions
{
    [Serializable]
    public class SetDirectionAction : FSMPhysicsActionBase
    {
        #region Fields
        [SerializeField]
        private bool _isRandom;
        [SerializeField]
        private int _direction;
        #endregion

        #region Properties
        public override string Name => "Set direction";
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override void Invoke()
        {
            if (_isRandom)
            {
                _physics.SetMoveDirection(Random.Range(0, 101) < 50 ? -1 : 1);
            }
            else
            {
                _physics.SetMoveDirection(_direction);
            }
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
