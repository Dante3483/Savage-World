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
            Task.Run(ReadMessageLoop);
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

        public NetworkClient GetClient(int id)
        {
            return _clients[id];
        }

        public void Broadcast(byte[] buffer, int ignoreClientId = -1)
        {
            for (int i = 0; i < _maxClients; i++)
            {
                if (ignoreClientId == i)
                {
                    continue;
                }
                WriteMessageTo(i, buffer);
            }
        }

        public void WriteMessageTo(int clientId, byte[] buffer)
        {
            NetworkClient client = _clients[clientId];
            if (client.IsActive)
            {
                client.WriteMessage(buffer);
            }
        }

        public string GetInfo()
        {
            return _connection.ToString();
        }
        #endregion

        #region Private Methods
        private void ReadMessageLoop()
        {
            Action action = () => _messanger.TryRead();
            try
            {
                while (IsActive)
                {
                    ReadMessageFromAll(_messanger.ReadBuffer, action);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"SERVER LOOP ERROR: {e.Message}");
            }
        }

        private void ReadMessageFromAll(byte[] buffer, Action callback = null)
        {
            for (int i = 0; i < _maxClients; i++)
            {
                ReadMessageFrom(i, buffer, callback);
            }
        }

        private void ReadMessageFrom(int clientId, byte[] buffer, Action callback = null)
        {
            NetworkClient client = _clients[clientId];
            if (client.IsActive)
            {
                client.ReadMessage(buffer, callback);
            }
        }
        #endregion
    }
}
