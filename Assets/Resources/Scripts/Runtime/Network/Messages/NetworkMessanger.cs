using SavageWorld.Runtime.Enums.Network;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SavageWorld.Runtime.Network.Messages
{
    public class NetworkMessanger
    {
        #region Fields
        private byte[] _writeBuffer;
        private byte[] _readBuffer;
        private MemoryStream _writeMemoryStream;
        private MemoryStream _readMemoryStream;
        private BinaryWriter _binaryWriter;
        private BinaryReader _binaryReader;
        private Dictionary<NetworkMessageTypes, NetworkMessageBase> _messageByType;
        #endregion

        #region Properties
        public byte[] WriteBuffer
        {
            get
            {
                return _writeBuffer;
            }

            set
            {
                _writeBuffer = value;
            }
        }

        public byte[] ReadBuffer
        {
            get
            {
                return _readBuffer;
            }

            set
            {
                _readBuffer = value;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public NetworkMessanger()
        {
            _writeBuffer = new byte[1024];
            _readBuffer = new byte[1024];
            _writeMemoryStream = new(_writeBuffer);
            _readMemoryStream = new(_readBuffer);
            _binaryWriter = new(_writeMemoryStream);
            _binaryReader = new(_readMemoryStream);
            InitializeMessages();
        }

        public void Write(NetworkMessageTypes messageType, int number1 = 0)
        {
            if (_messageByType.TryGetValue(messageType, out NetworkMessageBase message))
            {
                _binaryWriter.BaseStream.Position = 0;
                message.Write(number1);
            }
        }

        public void Read()
        {
            _binaryReader.BaseStream.Position = 0;
            NetworkMessageTypes messageType = (NetworkMessageTypes)_binaryReader.ReadByte();
            Debug.Log($"Read message: {messageType}");
            if (_messageByType.TryGetValue(messageType, out NetworkMessageBase message))
            {
                message.Read();
            }
        }
        #endregion

        #region Private Methods
        private void InitializeMessages()
        {
            _messageByType = new()
            {
                { NetworkMessageTypes.SendId, new SendIdMessage(_binaryWriter, _binaryReader) },
                { NetworkMessageTypes.Disconnect, new DisconnectMessage(_binaryWriter, _binaryReader) }
            };
        }
        #endregion
    }
}
