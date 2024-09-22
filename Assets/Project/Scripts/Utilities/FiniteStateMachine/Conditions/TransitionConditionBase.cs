using SavageWorld.Runtime.Entities.NPC;

namespace SavageWorld.Runtime.Utilities.FiniteStateMachine.Conditions
{
    public abstract class TransitionConditionBase
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public abstract bool Check(NPCBase entityData);

        public virtual void Reset()
        {

        }
        #endregion

        #region Private Methods

        #endregion
    }
}
