using SavageWorld.Runtime.Network.Connection;
using SavageWorld.Runtime.Network.Messages;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace SavageWorld.Runtime.Network
{
    public class NetworkServer
    {
        #region Fields
        private NetworkMessanger _messanger;
        private INetworkConnection _connection;
        private NetworkClient[] _clients;
        private int _maxClients;
        #endregion

        #region Properties
        public bool IsActive => _connection != null && _connection.IsServerActive;
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public NetworkServer()
        {
            _messanger = NetworkManager.Instance.Messanger;
        }

        public void SetConnection(INetworkConnection connection, int maxClients)
        {
            _connection = connection;
            _maxClients = maxClients;
            _clients = new NetworkClient[maxClients];
            for (int i = 0; i < _maxClients; i++)
            {
                _clients[i] = new();
            }
        }

        public void Start(string ipAddress, int port)
        {
            _connection.Start(ipAddress, port);
            Task.Run(Loop);
        }

        public void Stop()
        {
            for (int i = 0; i < _maxClients; i++)
            {
                DisconnectClient(i);
            }
            _clients = null;
            _connection.Stop();
        }

        public void DisconnectClient(int clientId)
        {
            NetworkClient client = _clients[clientId];
            if (client.IsActive)
            {
                client.Disconnect();
            }
        }

        public int AddClient(INetworkConnection connection)
        {
            for (int i = 0; i < _maxClients; i++)
            {
                NetworkClient client = _clients[i];
                if (!client.IsActive)
                {
                    client.SetConnection(connection);
                    return i;
                }
            }
            connection.Disconnect();
            return -1;
        }

        public void Broadcast(byte[] buffer, Action callback = null)
        {
            for (int i = 0; i < _maxClients; i++)
            {
                WriteMessageTo(i, buffer, callback);
            }
        }

        public void ReadMessageFromAll(byte[] buffer, Action callback = null)
        {
            for (int i = 0; i < _maxClients; i++)
            {
                ReadMessageFrom(i, buffer, callback);
            }
        }

        public void ReadMessageFrom(int clientId, byte[] buffer, Action callback = null)
        {
            NetworkClient client = _clients[clientId];
            if (client.IsActive)
            {
                client.ReadMessage(buffer, callback);
            }
        }

        public void WriteMessageTo(int clientId, byte[] buffer, Action callback = null)
        {
            NetworkClient client = _clients[clientId];
            if (client.IsActive)
            {
                client.WriteMessage(buffer, callback);
            }
        }
        #endregion

        #region Private Methods
        private void Loop()
        {
            try
            {
                while (IsActive)
                {
                    ReadMessageFromAll(_messanger.ReadBuffer, () =>
                    {
                        _messanger.Read();
                    });
                }
            }
            catch (Exception e)
            {
                Debug.Log($"SERVER LOOP ERROR: {e.Message}");
            }
        }
        #endregion
    }
}
