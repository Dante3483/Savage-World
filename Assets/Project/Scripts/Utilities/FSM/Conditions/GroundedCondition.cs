using SavageWorld.Runtime.Entities.NPC;

namespace SavageWorld.Runtime.Utilities.FSM.Conditions
{
    public class GroundedCondition : FSMPhysicsConditionBase
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
            return !entity.Flags.PrevGrounded && entity.Flags.IsGrounded && !entity.Flags.IsRise;
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
