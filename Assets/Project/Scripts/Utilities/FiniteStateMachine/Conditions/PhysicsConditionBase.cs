using SavageWorld.Runtime.Entities.NPC;
using SavageWorld.Runtime.Physics;

namespace SavageWorld.Runtime.Utilities.FiniteStateMachine.Conditions
{
    public abstract class PhysicsConditionBase : TransitionConditionBase
    {
        #region Fields
        protected PhysicsFlags _flags;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override bool Check(NPCBase entityData)
        {
            _flags = entityData.Flags;
            return Check();
        }

        public abstract bool Check();
        #endregion

        #region Private Methods

        #endregion
    }
}
