using SavageWorld.Runtime.Attributes;
using SavageWorld.Runtime.Entities.NPC;

namespace SavageWorld.Runtime.Utilities.FiniteStateMachine.Conditions
{
    [FSMComponent("No condition", "")]
    public class NoCondition : TransitionConditionBase
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override bool Check(NPCBase entityData)
        {
            return true;
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
