using SavageWorld.Runtime.Attributes;
using SavageWorld.Runtime.Entities.NPC;

namespace SavageWorld.Runtime.Utilities.FSM.Conditions
{
    [FSMComponent("No condition", "")]
    public class NoCondition : FSMConditionBase
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override bool Check(NPCBase entity)
        {
            return true;
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
