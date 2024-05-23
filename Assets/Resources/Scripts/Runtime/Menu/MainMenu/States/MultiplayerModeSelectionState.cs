public class MultiplayerModeSelectionState : MainMenuStateBase
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
        UIManager.Instance.MainMenuMultiplayerUI.IsActive = true;
    }

    public override void Exit()
    {
        UIManager.Instance.MainMenuMultiplayerUI.IsActive = false;
    }

    public override void Back()
    {
        _mainMenuManager.ChangeState(_mainMenuManager.StarterMenuState);
    }
    #endregion
}
