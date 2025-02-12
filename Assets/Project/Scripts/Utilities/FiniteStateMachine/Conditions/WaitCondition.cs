using SavageWorld.Runtime.Attributes;
using SavageWorld.Runtime.Entities.NPC;
using System;
using UnityEngine;

namespace SavageWorld.Runtime.Utilities.FiniteStateMachine.Conditions
{
    [Serializable]
    [FSMComponent("Wait", "")]
    public class WaitCondition : TransitionConditionBase
    {
        #region Fields
        [SerializeField]
        private float _timeToWait;
        [SerializeField]
        private float _time;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override bool Check(NPCBase entityData)
        {
            _time += Time.fixedDeltaTime;
            if (_time >= _timeToWait)
            {
                return true;
            }
            return false;
        }

        public override void Reset()
        {
            _time = 0f;
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
