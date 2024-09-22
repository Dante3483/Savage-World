using SavageWorld.Runtime.Attributes;

namespace SavageWorld.Runtime.Utilities.FiniteStateMachine.Actions
{
    [FSMComponent("Move", "")]
    public class MoveAction : PhysicsActionBase
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
            _physic.Move(_stats.WalkingSpeed);
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
