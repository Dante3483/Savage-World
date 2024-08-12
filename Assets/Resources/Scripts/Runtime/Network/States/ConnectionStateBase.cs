namespace SavageWorld.Runtime.Network.States
{
    public abstract class ConnectionStateBase : StateBase
    {
        #region Private fields
        protected ConnectionManager _connectionManager;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        public ConnectionStateBase(ConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public virtual void OnHostStarted() { }

        public virtual void OnHostStopped() { }

        public virtual void OnClientConnected(int clientId) { }

        public virtual void OnClientDisconnected(int clientId) { }

        public virtual void OnConnected() { }

        public virtual void OnDisconnected() { }

        public virtual void OnUserRequestedShutdown() { }

        public virtual void StartClientIP(string address, ushort port) { }

        public virtual void StartServerIp(string address, ushort port) { }
        #endregion
    }
}