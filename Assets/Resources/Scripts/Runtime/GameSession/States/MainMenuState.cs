namespace SavageWorld.Runtime.GameSession.States
{
    public class MainMenuState : GameStateBase
    {
        #region Private fields
        private MainMenuManager _mainMenuManager;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        public MainMenuState() : base()
        {
            _mainMenuManager = MainMenuManager.Instance;
        }

        public override void Enter()
        {
            _mainMenuManager.ChangeState(_mainMenuManager.StarterMenuState);
        }

        public override void Exit()
        {

        }
        #endregion
    }
}