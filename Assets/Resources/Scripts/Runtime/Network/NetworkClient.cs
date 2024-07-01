using SavageWorld.Runtime.Network.Connection;
using SavageWorld.Runtime.Network.Messages;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace SavageWorld.Runtime.Network
{
    public class NetworkClient
    {
        #region Fields
        private NetworkMessanger _messanger;
        private INetworkConnection _connection;
        private int _id;
        #endregion

        #region Properties
        public bool IsActive => _connection != null && _connection.IsClientActive;
        public bool IsReading => _connection.IsReading;
        public int Id
        {
            get
            {
                return _id;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public NetworkClient()
        {
            _messanger = NetworkManager.Instance.Messanger;
        }

        public void SetConnection(INetworkConnection connection)
        {
            _connection = connection;
        }

        public void SetId(int id)
        {
            if (id == -1)
            {
                _connection.Disconnect();
            }
            else
            {
                _id = id;
            }
            Debug.Log(id);
        }

        public void Connect(string ipAddress, int port)
        {
            _connection.Connect(ipAddress, port);
            Task.Run(Loop);
        }

        public void Disconnect()
        {
            _connection.Disconnect();
            _connection = null;
        }

        public void WriteMessage(byte[] buffer, Action callback = null)
        {
            _connection?.Write(buffer, callback);
        }

        public void ReadMessage(byte[] buffer, Action callback = null)
        {
            _connection?.Read(buffer, callback);
        }
        #endregion

        #region Private Methods
        private void Loop()
        {
            try
            {
                while (IsActive)
                {
                    ReadMessage(_messanger.ReadBuffer, () =>
                    {
                        _messanger.Read();
                    });
                }
            }
            catch (Exception e)
            {
                Debug.Log($"CLIENT LOOP ERROR: {e.Message}");
            }
        }
        #endregion
    }
}
