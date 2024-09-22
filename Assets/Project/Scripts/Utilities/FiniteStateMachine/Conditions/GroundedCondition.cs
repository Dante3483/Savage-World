using SavageWorld.Runtime.Attributes;

namespace SavageWorld.Runtime.Utilities.FiniteStateMachine.Conditions
{
    [FSMComponent("Grounded", "")]
    public class GroundedCondition : PhysicsConditionBase
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override bool Check()
        {
            return !_flags.PrevGrounded && _flags.IsGrounded && !_flags.IsRise;
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
