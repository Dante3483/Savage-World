using SavageWorld.Runtime.Console;
using SavageWorld.Runtime.Enums.Network;
using SavageWorld.Runtime.Network.Connection;
using SavageWorld.Runtime.Network.Messages;
using SavageWorld.Runtime.Network.Objects;
using System;
using System.Threading.Tasks;
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

        public bool IsClient => _client.IsActive;

        public NetworkObjects NetworkObjects
        {
            get
            {
                return _networkObjects;
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

        public void StartServer()
        {
            if (!_server.IsActive)
            {
                SetServerConnection();
                string ipAddress = string.IsNullOrEmpty(_ipAddressInputField.text) ? _networkConfiguration.IpAddress : _ipAddressInputField.text;
                _server.Start(ipAddress, _networkConfiguration.Port);
                _networkObjects.CreatePlayerServer(true);
            }
            GameConsole.LogText("Server started", Color.green);
        }

        public void StopServer()
        {
            if (_server.IsActive)
            {
                _server.Stop();
            }
            GameConsole.LogText("Server stopped", Color.green);
        }

        public void ConnectToServer()
        {
            if (!_client.IsActive)
            {
                TCPNetworkConnection connection = new();
                connection.Connected += ConnectedEventHandler;
                connection.Disconnected += DisconnectedEventHandler;
                _client.SetConnection(connection);
                string ipAddress = string.IsNullOrEmpty(_ipAddressInputField.text) ? _networkConfiguration.IpAddress : _ipAddressInputField.text;
                _client.Connect(ipAddress, _networkConfiguration.Port);
            }
        }

        public void DisconnectFromServer()
        {
            if (IsClient)
            {
                SendMessage(NetworkMessageTypes.Disconnect, new MessageData { IntNumber1 = _client.Id }, callback: _client.Disconnect);
                _networkObjects.Reset();
            }
        }

        public void DisconnectClient(int clientId)
        {
            NetworkClient client = _server.GetClient(clientId);
            _networkObjects.DestroyPlayer(client.PlayerId);
            client.Disconnect();
        }

        public async Task SendMessageAsync(NetworkMessageTypes messageType, MessageData messageData, Action callback = null, bool isBroadcast = false, int clientId = -1)
        {
            if (IsServer)
            {
                await Task.Run(() => SendMessageToClients(messageType, messageData, callback, isBroadcast, clientId));
            }
            if (IsClient)
            {
                await Task.Run(() => SendMessageToServer(messageType, messageData, callback));
            }
        }

        public void SendMessage(NetworkMessageTypes messageType, MessageData messageData, Action callback = null, bool isBroadcast = false, int clientId = -1)
        {
            if (IsServer)
            {
                Task.Run(() => SendMessageToClients(messageType, messageData, callback, isBroadcast, clientId));
            }
            if (IsClient)
            {
                Task.Run(() => SendMessageToServer(messageType, messageData, callback));
            }
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
        private async void InitializeClient(int id)
        {
            NetworkClient client = _server.GetClient(id);
            client.State = 0;
            bool clientConnected = false;
            while (!clientConnected)
            {
                switch (client.State)
                {
                    case 0:
                        {
                            await SendMessageAsync(NetworkMessageTypes.SendClientId, new MessageData { IntNumber1 = id }, clientId: id);
                            client.State = 1;
                        }
                        break;
                    case 1:
                        {
                            int chunkX = 36;
                            int chunkY = 22;
                            for (int x = chunkX - 1; x <= chunkX + 1; x++)
                            {
                                for (int y = chunkY - 1; y <= chunkY + 1; y++)
                                {
                                    await SendMessageAsync(NetworkMessageTypes.SendChunkData, new MessageData { IntNumber1 = x, IntNumber2 = y }, clientId: id);
                                }
                            }
                            client.State = 2;
                        }
                        break;
                    case 2:
                        {
                            foreach (NetworkObject networkObject in _networkObjects.ObjectsById.Values)
                            {
                                switch (networkObject.Type)
                                {
                                    case NetworkObjectTypes.Player:
                                        {
                                            MessageData data = new()
                                            {
                                                LongNumber1 = networkObject.Id,
                                                Bool1 = false,
                                                FloatNumber1 = networkObject.Position.x,
                                                FloatNumber2 = networkObject.Position.y,
                                            };
                                            await SendMessageAsync(NetworkMessageTypes.CreatePlayer, data, clientId: id);
                                        }
                                        break;
                                    case NetworkObjectTypes.Drop:
                                        {

                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            client.State = 3;
                        }
                        break;
                    case 3:
                        {
                            long playerId = _networkObjects.CreatePlayerServer();
                            MessageData data = new()
                            {
                                LongNumber1 = playerId,
                                Bool1 = true,
                                FloatNumber1 = 3655,
                                FloatNumber2 = 2200,
                            };
                            await SendMessageAsync(NetworkMessageTypes.CreatePlayer, data, clientId: id);
                            client.PlayerId = playerId;
                            client.State = 4;
                        }
                        break;
                    default:
                        {
                            clientConnected = true;
                            GameConsole.LogText($"Client {id} connected");
                            ClientConnected?.Invoke(id);
                        }
                        break;
                }
            }
        }

        private void SendMessageToClients(NetworkMessageTypes messageType, MessageData messageData, Action callback, bool isBroadcast, int clientId)
        {
            lock (_messanger.WriteBuffer)
            {
                long size = _messanger.Write(messageType, messageData);
                if (isBroadcast)
                {
                    _server.Broadcast(_messanger.WriteBuffer, size);
                    callback?.Invoke();
                }
                else
                {
                    _server.WriteMessageTo(clientId, _messanger.WriteBuffer, size);
                    callback?.Invoke();
                }
            }
        }

        private void SendMessageToServer(NetworkMessageTypes messageType, MessageData messageData, Action callback)
        {
            lock (_messanger.WriteBuffer)
            {
                long size = _messanger.Write(messageType, messageData);
                _client.WriteMessage(_messanger.WriteBuffer, size);
                callback?.Invoke();
            }
        }

        private void SetServerConnection()
        {
            TCPNetworkConnection connection = new();
            connection.ServerStarted += ServerStartedEventHandler;
            connection.ServerStopped += ServerStoppedEventHandler;
            connection.ClientConnected += ClientConnectedEventHandler;
            connection.ClientDisconnected += ClientDisconnectedEventHandler;
            _server.SetConnection(connection, _networkConfiguration.MaxClients);
        }

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
            int id = _server.AddClientToArray(connection);
            if (id != -1)
            {
                InitializeClient(id);
            }
            else
            {
                connection.Disconnect();
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
