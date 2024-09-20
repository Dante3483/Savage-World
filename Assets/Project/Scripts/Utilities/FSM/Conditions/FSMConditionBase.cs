using SavageWorld.Runtime.Entities.NPC;
using System;

namespace SavageWorld.Runtime.Utilities.FSM.Conditions
{
    public abstract class FSMConditionBase : ICloneable
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public abstract bool Check(NPCBase entity);
        #endregion

        #region Private Methods
        public object Clone()
        {
            return MemberwiseClone();
        }
        #endregion
    }
}
