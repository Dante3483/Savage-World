namespace SavageWorld.Runtime.Network.States
{
    public class HostStartedState : OnlineState
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
        public HostStartedState(ConnectionManager connectionManager) : base(connectionManager)
        {

        }

        public override void Enter()
        {

        }

        public override void Exit()
        {

        }

        public override void OnHostStopped()
        {
            _connectionManager.ChangeState(_connectionManager.OfflineState);
        }

        public override void OnClientConnected(int clientId)
        {
            EventManager.OnPlayerConnected(clientId);
        }

        public override void OnUserRequestedShutdown()
        {
            _connectionManager.NetworkManager.StopHost();
            _connectionManager.ChangeState(_connectionManager.OfflineState);
        }
        #endregion

        #region Private Methods

        #endregion
    }
}