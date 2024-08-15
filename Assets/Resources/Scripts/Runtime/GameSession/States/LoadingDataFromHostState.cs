namespace SavageWorld.Runtime.GameSession.States
{
    public class LoadingDataFromHostState : GameStateBase
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
            UIManager.Instance.MainMenuProgressBarUI.IsActive = true;
        }

        public override void Exit()
        {
            UIManager.Instance.MainMenuProgressBarUI.IsActive = false;
        }
        #endregion
    }
}