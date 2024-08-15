using System;

namespace SavageWorld.Runtime.Network.Connection
{
    public interface INetworkConnection
    {
        #region Properties
        public bool IsServerActive { get; }
        public bool IsClientActive { get; }
        public bool IsReading { get; }
        #endregion

        #region Events / Delegates
        public event Action<INetworkConnection> ServerStarted;
        public event Action<INetworkConnection> ServerStopped;
        public event Action<INetworkConnection> ClientConnected;
        public event Action<INetworkConnection> ClientDisconnected;
        public event Action<INetworkConnection> Connected;
        public event Action<INetworkConnection> Disconnected;
        #endregion

        #region Public Methods
        public void Start(string ipAddress, int port);

        public void Stop();

        public void Connect(string ipAddress, int port);

        public void Disconnect();

        public void Read(byte[] buffer, Action<object> callback = null);

        public void Write(byte[] buffer, long size);
        #endregion
    }
}