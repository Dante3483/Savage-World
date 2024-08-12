namespace SavageWorld.Runtime.Network.States
{
    public class ClientConnectedState : OnlineState
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
        public ClientConnectedState(ConnectionManager connectionManager) : base(connectionManager)
        {

        }

        public override void Enter()
        {

        }

        public override void Exit()
        {

        }

        public override void OnDisconnected()
        {
            _connectionManager.ChangeState(_connectionManager.OfflineState);
        }

        public override void OnUserRequestedShutdown()
        {
            _connectionManager.NetworkManager.DisconnectFromServer();
            _connectionManager.ChangeState(_connectionManager.OfflineState);
        }
        #endregion

        #region Private Methods

        #endregion
    }
}