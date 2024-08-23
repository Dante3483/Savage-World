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
        private byte _id;
        private long _playerId;
        private int _state;
        #endregion

        #region Properties
        public bool IsActive => _connection != null && _connection.IsClientActive;

        public bool IsReading => _connection.IsReading;

        public byte Id
        {
            get
            {
                return _id;
            }
        }

        public long PlayerId
        {
            get
            {
                return _playerId;
            }

            set
            {
                _playerId = value;
            }
        }

        public int State
        {
            get
            {
                return _state;
            }

            set
            {
                _state = value;
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

        public void SetId(byte id)
        {
            if (id == byte.MaxValue)
            {
                _connection.Disconnect();
            }
            else
            {
                _id = id;
            }
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

        public void WriteMessage(byte[] buffer, long size)
        {
            _connection?.Write(buffer, size);
        }

        public void ReadMessage(byte[] buffer, Action<object> callback = null)
        {
            _connection?.Read(buffer, callback);
        }
        #endregion

        #region Private Methods
        private void Loop()
        {
            Action<object> action = (size) =>
            {
                _messanger.TryRead((int)size);
            };
            try
            {
                while (IsActive)
                {
                    ReadMessage(_messanger.ReadBuffer, action);
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
