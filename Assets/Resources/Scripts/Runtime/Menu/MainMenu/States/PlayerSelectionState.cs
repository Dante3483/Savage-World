using SavageWorld.Runtime.Network;

public class PlayerSelectionState : MainMenuStateBase
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
        ListOfExistingPlayersController.Instance.UpdateUI();
        ListOfExistingPlayersController.Instance.OnPlayerCreated += _mainMenuManager.CreatePlayer;
        ListOfExistingPlayersController.Instance.OnPlayerSelected += _mainMenuManager.SelectPlayer;
        ListOfExistingPlayersController.Instance.OnPlayerDeleted += _mainMenuManager.DeletePlayer;
        UIManager.Instance.MainMenuPlayersUI.IsActive = true;
    }

    public override void Exit()
    {
        ListOfExistingPlayersController.Instance.OnPlayerCreated -= _mainMenuManager.CreatePlayer;
        ListOfExistingPlayersController.Instance.OnPlayerSelected -= _mainMenuManager.SelectPlayer;
        ListOfExistingPlayersController.Instance.OnPlayerDeleted -= _mainMenuManager.DeletePlayer;
        UIManager.Instance.MainMenuPlayersUI.IsActive = false;
    }

    public override void Back()
    {
        if (NetworkManager.Instance.IsMultiplayer)
        {
            if (GameManager.Instance.IsClient)
            {
                _mainMenuManager.ChangeState(_mainMenuManager.SetupNetworkConnectionState);
            }
            else
            {
                _mainMenuManager.ChangeState(_mainMenuManager.MultiplayerModeSelectionState);
            }
        }
        else
        {
            _mainMenuManager.ChangeState(_mainMenuManager.StarterMenuState);
        }
    }
    #endregion
}
