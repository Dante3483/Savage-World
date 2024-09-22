using SavageWorld.Runtime.Entities.NPC;
using System;

namespace SavageWorld.Runtime.Utilities.FiniteStateMachine.Actions
{
    [Serializable]
    public abstract class StateActionBase
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public abstract void Invoke(NPCBase entityData);
        #endregion

        #region Private Methods

        #endregion
    }
}
