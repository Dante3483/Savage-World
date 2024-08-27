using SavageWorld.Runtime.Managers;

namespace SavageWorld.Runtime.Menu.States
{
    public class WorldSelectionState : MainMenuStateBase
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
            ListOfExistingWorldsController.Instance.UpdateUI();
            ListOfExistingWorldsController.Instance.OnWorldCreated += _mainMenuManager.CreateWorld;
            ListOfExistingWorldsController.Instance.OnWorldSelected += _mainMenuManager.SelectWorld;
            ListOfExistingWorldsController.Instance.OnWorldDeleted += _mainMenuManager.DeleteWorld;
            UIManager.Instance.MainMenuWorldsUI.IsActive = true;
        }

        public override void Exit()
        {
            ListOfExistingWorldsController.Instance.OnWorldCreated -= _mainMenuManager.CreateWorld;
            ListOfExistingWorldsController.Instance.OnWorldSelected -= _mainMenuManager.SelectWorld;
            ListOfExistingWorldsController.Instance.OnWorldDeleted -= _mainMenuManager.DeleteWorld;
            UIManager.Instance.MainMenuWorldsUI.IsActive = false;
        }

        public override void Back()
        {
            _mainMenuManager.ChangeState(_mainMenuManager.PrevState);
        }
        #endregion
    }
}