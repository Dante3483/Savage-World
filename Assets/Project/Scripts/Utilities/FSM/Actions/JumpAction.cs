namespace SavageWorld.Runtime.Utilities.FSM.Actions
{
    public class JumpAction : FSMPhysicsActionBase
    {
        #region Fields

        #endregion

        #region Properties
        public override string Name => "Jump";
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override void Invoke()
        {
            _physics.Jump(_stats.JumpForce, false);
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
