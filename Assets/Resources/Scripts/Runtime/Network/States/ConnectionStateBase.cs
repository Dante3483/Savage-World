namespace SavageWorld.Runtime.Network.States
{
    public abstract class ConnectionStateBase : StateBase
    {
        #region Private fields
        protected NetworkManager _connectionManager;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        public ConnectionStateBase(NetworkManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public virtual void OnClientConnected(ulong clientId) { }

        public virtual void OnClientDisconnect(ulong clientId) { }

        public virtual void OnServerStarted() { }

        public virtual void StartClientIP(string playerName, string address, ushort port) { }

        public virtual void StartHostIP(string playerName, string address, ushort port) { }

        public virtual void OnUserRequestedShutdown() { }

        public virtual void ApprovalCheck(Unity.Netcode.NetworkManager.ConnectionApprovalRequest request, Unity.Netcode.NetworkManager.ConnectionApprovalResponse response) { }

        public virtual void OnTransportFailure() { }

        public virtual void OnServerStopped() { }
        #endregion
    }
}