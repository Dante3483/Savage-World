namespace SavageWorld.Runtime.Network.States
{
    public class OfflineState : ConnectionStateBase
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
        public OfflineState(ConnectionManager connectionManager) : base(connectionManager)
        {

        }

        public override void Enter()
        {

        }

        public override void Exit()
        {

        }

        public override void StartClientIP(string address, ushort port)
        {
            _connectionManager.NetworkManager.ConfigureNetwork(address, port);
            _connectionManager.ChangeState(_connectionManager.ClientConnectingState);

        }

        public override void StartServerIp(string address, ushort port)
        {
            _connectionManager.NetworkManager.ConfigureNetwork(address, port);
            _connectionManager.ChangeState(_connectionManager.StartingServerState);
        }
        #endregion

        #region Private Methods

        #endregion
    }
}