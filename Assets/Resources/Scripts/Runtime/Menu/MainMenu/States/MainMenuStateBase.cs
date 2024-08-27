using SavageWorld.Runtime.Utilities.StateMachine;

namespace SavageWorld.Runtime.Menu.States
{
    public abstract class MainMenuStateBase : StateBase
    {
        #region Private fields
        protected MainMenuManager _mainMenuManager;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        public MainMenuStateBase()
        {
            _mainMenuManager = MainMenuManager.Instance;
        }

        public virtual void Back()
        {

        }
        #endregion
    }
}