using SavageWorld.Runtime.Attributes;

namespace SavageWorld.Runtime.Utilities.FiniteStateMachine.Actions
{
    [FSMComponent("Jump", "")]
    public class JumpAction : PhysicsActionBase
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override void Invoke()
        {
            _physic.Jump(_stats.JumpForce, false);
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
