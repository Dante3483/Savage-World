using SavageWorld.Runtime.Console;
using SavageWorld.Runtime.Enums.Network;
using System;
using System.IO;

namespace SavageWorld.Runtime.Network.Messages
{
    public abstract class NetworkMessageBase
    {
        #region Fields
        protected BinaryWriter _writer;
        protected BinaryReader _reader;
        protected NetworkManager _networkManager;
        protected byte _senderId;
        #endregion

        #region Properties
        protected abstract NetworkMessageTypes MessageType { get; }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public NetworkMessageBase(BinaryWriter writer, BinaryReader reader)
        {
            _writer = writer;
            _reader = reader;
            _networkManager = NetworkManager.Instance;
        }

        public bool Write(MessageData messageData)
        {
            try
            {
                WriteType();
                WriteId();
                WriteData(messageData);
                return true;
            }
            catch (Exception e)
            {
                GameConsole.LogError(e.Message);
                return false;
            }
        }

        public bool Read()
        {
            try
            {
                _senderId = byte.MaxValue;
                if (_networkManager.IsServer)
                {
                    _senderId = _reader.ReadByte();
                }
                ReadData();
                return true;
            }
            catch (Exception e)
            {
                GameConsole.LogError(e.Message);
                return false;
            }
        }
        #endregion

        #region Private Methods
        protected abstract void WriteData(MessageData messageData);

        protected abstract void ReadData();

        private void WriteType()
        {
            _writer.Write((byte)MessageType);
        }

        private void WriteId()
        {
            if (_networkManager.IsClient)
            {
                _writer.Write(_networkManager.Client.Id);
            }
        }
        #endregion
    }
}
