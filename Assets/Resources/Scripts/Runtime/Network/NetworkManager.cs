using SavageWorld.Runtime.Network.Connection;
using SavageWorld.Runtime.Network.Messages;
using System;
using UnityEngine;

namespace SavageWorld.Runtime.Network
{
    [RequireComponent(typeof(NetworkConfiguration))]
    public class NetworkManager : Singleton<NetworkManager>
    {
        #region Fields
        [SerializeField]
        private NetworkConfiguration _networkConfiguration;
        private NetworkServer _server;
        private NetworkClient _client;
        private NetworkMessanger _messanger;
        #endregion

        #region Properties
        public NetworkMessanger Messanger
        {
            get
            {
                return _messanger;
            }
        }

        public NetworkServer Server
        {
            get
            {
                return _server;
            }
        }

        public NetworkClient Client
        {
            get
            {
                return _client;
            }
        }
        #endregion

        #region Events / Delegates
        public event Action<INetworkConnection> ServerStarted;
        public event Action<INetworkConnection> ServerStopped;
        public event Action<int> ClientConnected;
        public event Action<int> ClientDisconnected;
        public event Action<INetworkConnection> Connected;
        public event Action<INetworkConnection> Disconnected;
        #endregion

        #region Monobehaviour Methods
        protected override void Awake()
        {
            base.Awake();
            _networkConfiguration = GetComponent<NetworkConfiguration>();
            _messanger = new();
            _server = new();
            _client = new();
        }

        private void OnApplicationQuit()
        {
            if (_server.IsActive)
            {
                _server.Stop();
                _server = null;
            }
            if (_client.IsActive)
            {
                _client.Disconnect();
                _client = null;
            }
        }
        #endregion

        #region Public Methods
        public void ConfigureNetwork(string ipAddress, ushort port)
        {
            _networkConfiguration.Configure(ipAddress, port);
        }

        public void StartServer()
        {
            if (!_server.IsActive)
            {
                TCPNetworkConnection connection = new();
                connection.ServerStarted += ServerStartedEventHandler;
                connection.ServerStopped += ServerStoppedEventHandler;
                connection.ClientConnected += ClientConnectedEventHandler;
                connection.ClientDisconnected += ClientDisconnectedEventHandler;
                _server.SetConnection(connection, _networkConfiguration.MaxClients);
                _server.Start(_networkConfiguration.IpAddress, _networkConfiguration.Port);
            }
        }

        public void StopServer()
        {
            if (_server.IsActive)
            {
                _server.Stop();
            }
        }

        public void Connect()
        {
            if (!_client.IsActive)
            {
                TCPNetworkConnection connection = new();
                connection.Connected += ConnectedEventHandler;
                connection.Disconnected += DisconnectedEventHandler;
                _client.SetConnection(connection);
                _client.Connect(_networkConfiguration.IpAddress, _networkConfiguration.Port);
            }
        }

        public void Disconnect()
        {
            if (_client is not null)
            {
                _client.Disconnect();
            }
        }
        #endregion

        #region Private Methods
        private void ServerStartedEventHandler(INetworkConnection connection)
        {
            ServerStarted?.Invoke(connection);
        }

        private void ServerStoppedEventHandler(INetworkConnection connection)
        {
            ServerStopped?.Invoke(connection);
        }

        private void ClientConnectedEventHandler(INetworkConnection connection)
        {
            int id = _server.AddClient(connection);
            Debug.Log($"Client {id} connected");
            _messanger.Write(Enums.Network.NetworkMessageTypes.SendId, id);
            _server.WriteMessageTo(id, _messanger.WriteBuffer);
            ClientConnected?.Invoke(id);
        }

        private void ClientDisconnectedEventHandler(INetworkConnection connection)
        {
            ClientDisconnected?.Invoke(0);
        }

        private void ConnectedEventHandler(INetworkConnection connection)
        {
            Connected?.Invoke(connection);
        }

        private void DisconnectedEventHandler(INetworkConnection connection)
        {
            int id = _client.Id;
            _messanger.Write(Enums.Network.NetworkMessageTypes.Disconnect, id);
            _client.WriteMessage(_messanger.WriteBuffer);
            Debug.Log("Disconnected");
            Disconnected?.Invoke(connection);
        }
        #endregion
    }
}
