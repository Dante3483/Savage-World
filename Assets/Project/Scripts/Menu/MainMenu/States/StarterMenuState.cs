using SavageWorld.Runtime.Managers;

namespace SavageWorld.Runtime.Menu.States
{
    public class StarterMenuState : MainMenuStateBase
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
            UIManager.Instance.MainMenuUI.IsActive = true;
        }

        public override void Exit()
        {
            UIManager.Instance.MainMenuUI.IsActive = false;
        }
        #endregion
    }
}