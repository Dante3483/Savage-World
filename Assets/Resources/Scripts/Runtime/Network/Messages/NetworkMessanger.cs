using SavageWorld.Runtime.Console;
using SavageWorld.Runtime.Enums.Network;
using System;
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
            _writeBuffer = new byte[32];
            _readBuffer = new byte[32];
            _writeMemoryStream = new(_writeBuffer);
            _readMemoryStream = new(_readBuffer);
            _binaryWriter = new(_writeMemoryStream);
            _binaryReader = new(_readMemoryStream);
            InitializeMessages();
        }

        public void Write(NetworkMessageTypes messageType, MessageData messageData)
        {
            if (_messageByType.TryGetValue(messageType, out NetworkMessageBase message))
            {
                _binaryWriter.BaseStream.Position = 0;
                message.Write(messageData);
                if (messageType != NetworkMessageTypes.SendPosition)
                {
                    GameConsole.LogText(messageType.ToString(), Color.yellow);
                }
            }
        }

        public bool TryRead()
        {
            try
            {
                Read();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                return false;
            }
            return true;
        }

        public void Read()
        {
            _binaryReader.BaseStream.Position = 0;
            NetworkMessageTypes messageType = (NetworkMessageTypes)_binaryReader.ReadByte();
            NetworkMessageBase message = _messageByType[messageType];
            message.Read();
            if (messageType != NetworkMessageTypes.SendPosition)
            {
                GameConsole.LogText(messageType.ToString(), Color.yellow);
            }
        }
        #endregion

        #region Private Methods
        private void InitializeMessages()
        {
            _messageByType = new()
            {
                { NetworkMessageTypes.SendClientId, new SendClientIdMessage(_binaryWriter, _binaryReader) },
                { NetworkMessageTypes.CreatePlayer, new CreatePlayerMessage(_binaryWriter, _binaryReader) },
                { NetworkMessageTypes.Disconnect, new DisconnectMessage(_binaryWriter, _binaryReader) },
                { NetworkMessageTypes.SendPosition, new SendPositionMessage(_binaryWriter, _binaryReader) },
            };
        }
        #endregion
    }
}
