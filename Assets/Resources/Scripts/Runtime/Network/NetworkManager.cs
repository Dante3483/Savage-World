using SavageWorld.Runtime.Console;
using SavageWorld.Runtime.Enums.Network;
using SavageWorld.Runtime.Network.Connection;
using SavageWorld.Runtime.Network.Messages;
using System;
using TMPro;
using UnityEngine;

namespace SavageWorld.Runtime.Network
{
    [RequireComponent(typeof(NetworkConfiguration))]
    public class NetworkManager : Singleton<NetworkManager>
    {
        #region Fields
        [SerializeField]
        private NetworkConfiguration _networkConfiguration;
        [SerializeField]
        private NetworkObjects _networkObjects;
        [SerializeField]
        private TMP_InputField _ipAddressInputField;
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

        public bool IsServer => _server.IsActive;

        public bool IsHost => _server.IsActive && _client.IsActive;

        public bool IsClient => _client.IsActive;
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
                DisconnectFromServer();
                _client = null;
            }
        }
        #endregion

        #region Public Methods
        public void ConfigureNetwork(string ipAddress, ushort port)
        {
            _networkConfiguration.Configure(ipAddress, port);
        }

        public void StartHost()
        {
            StartServer();
            GameConsole.LogText("Server started", Color.green);
        }

        public void StopHost()
        {
            StopServer();
            GameConsole.LogText("Server stopped", Color.green);
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
                _server.Start(_ipAddressInputField.text, _networkConfiguration.Port);
            }
        }

        public void StopServer()
        {
            if (_server.IsActive)
            {
                _server.Stop();
            }
        }

        public void ConnectToServer()
        {
            if (!_client.IsActive)
            {
                TCPNetworkConnection connection = new();
                connection.Connected += ConnectedEventHandler;
                connection.Disconnected += DisconnectedEventHandler;
                _client.SetConnection(connection);
                _client.Connect(_ipAddressInputField.text, _networkConfiguration.Port);
            }
        }

        public void DisconnectFromServer()
        {
            if (_client is not null)
            {
                _messanger.Write(NetworkMessageTypes.Disconnect, new MessageData { IntNumber1 = _client.Id });
                _client.WriteMessage(_messanger.WriteBuffer, _client.Disconnect);
            }
        }

        public void Broadcast(NetworkMessageTypes messageType, int ignoreClientId = -1)
        {
            if (IsHost)
            {
                _server.Broadcast(_messanger.WriteBuffer, ignoreClientId);
            }
        }

        public void SendMessageToClient(int clientId, NetworkMessageTypes messageType, MessageData messageData)
        {
            if (IsClient)
            {
                return;
            }
        }

        public void SendMessageToServer(NetworkMessageTypes messageType, MessageData messageData)
        {
            if (IsServer)
            {
                return;
            }
            _messanger.Write(messageType, messageData);
            _client.WriteMessage(_messanger.WriteBuffer);
        }

        public void CreatePlayer(long id, bool isOwner)
        {
            _networkObjects.CreatePlayerClient(id, isOwner);
        }

        public void UpdateObjectPosition(long id, float x, float y)
        {
            _networkObjects.UpdatePosition(id, x, y);
        }

        public string GetServerInfo()
        {
            if (_server.IsActive)
            {
                return _server.GetInfo();
            }
            return "Server is not started";
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
            int clientId = _server.AddClient(connection);
            _messanger.Write(NetworkMessageTypes.SendClientId, new MessageData { IntNumber1 = clientId });
            connection.Write(_messanger.WriteBuffer);
            if (clientId == -1)
            {
                connection.Disconnect();
            }
            else
            {
                long playerId = _networkObjects.CreatePlayerServer();
                _messanger.Write(NetworkMessageTypes.CreatePlayer, new MessageData { LongNumber1 = playerId, Bool1 = true });
                connection.Write(_messanger.WriteBuffer);
                GameConsole.LogText($"Client {clientId} connected");
                ClientConnected?.Invoke(clientId);
            }
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
            Disconnected?.Invoke(connection);
        }
        #endregion
    }
}
