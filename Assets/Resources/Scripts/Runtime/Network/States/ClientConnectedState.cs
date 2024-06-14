namespace SavageWorld.Runtime.Network.States
{
    public class ClientConnectedState : OnlineState
    {
        #region Private fields

        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        public ClientConnectedState(NetworkManager connectionManager) : base(connectionManager)
        {

        }

        public override void Enter()
        {

        }

        public override void Exit()
        {

        }

        public override void OnClientDisconnect(ulong _)
        {
            var disconnectReason = _connectionManager.NetworkManagerOld.DisconnectReason;
            if (string.IsNullOrEmpty(disconnectReason) ||
                disconnectReason == "Disconnected due to host shutting down.")
            {
                _connectionManager.ChangeState(_connectionManager.ClientReconnectingState);
            }
            else
            {
                _connectionManager.ChangeState(_connectionManager.OfflineState);
            }
        }
        #endregion
    }
}