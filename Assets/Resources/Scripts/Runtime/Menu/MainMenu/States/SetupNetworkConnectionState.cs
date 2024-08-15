public class SetupNetworkConnectionState : MainMenuStateBase
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
        GameManager.Instance.IsClient = true;
        UIManager.Instance.MainMenuConnectIPUI.IsActive = true;
    }

    public override void Exit()
    {
        UIManager.Instance.MainMenuConnectIPUI.IsActive = false;
    }

    public override void Back()
    {
        GameManager.Instance.IsClient = false;
        _mainMenuManager.ChangeState(_mainMenuManager.MultiplayerModeSelectionState);
    }
    #endregion
}
