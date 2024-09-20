using SavageWorld.Runtime.Attributes;

namespace SavageWorld.Runtime.Utilities.FSM.Actions
{
    [FSMComponent("Move", "")]
    public class MoveAction : FSMPhysicsActionBase
    {
        #region Fields

        #endregion

        #region Properties
        public override string Name => "Move";
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override void Invoke()
        {
            _physics.Move(_stats.WalkingSpeed);
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
