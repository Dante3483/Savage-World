namespace SavageWorld.Runtime.GameSession.States
{
    public class PlayingState : GameStateBase
    {
        #region Private fields

        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        public override void Enter()
        {
            GameTime.Instance.enabled = true;
        }

        public override void Exit()
        {
            GameTime.Instance.enabled = false;
        }
        #endregion
    }
}