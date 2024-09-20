using SavageWorld.Runtime.Managers;

namespace SavageWorld.Runtime.Network.States
{
    public class ServerStartedState : OnlineState
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
        public ServerStartedState(ConnectionManager connectionManager) : base(connectionManager)
        {

        }

        public override void Enter()
        {

        }

        public override void Exit()
        {

        }

        public override void OnServerStopped()
        {
            _connectionManager.ChangeState(_connectionManager.OfflineState);
        }

        public override void OnClientConnected(int clientId)
        {
            EventManager.OnPlayerConnected(clientId);
        }

        public override void OnUserRequestedShutdown()
        {
            _connectionManager.NetworkManager.StopServer();
            _connectionManager.ChangeState(_connectionManager.OfflineState);
        }
        #endregion

        #region Private Methods

        #endregion
    }
}