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
        private bool _isBlocked;
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

        public bool IsBlocked
        {
            get
            {
                return _isBlocked;
            }

            set
            {
                _isBlocked = value;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public NetworkMessanger()
        {
            _writeBuffer = new byte[133120];
            _readBuffer = new byte[133120];
            _writeMemoryStream = new(_writeBuffer);
            _readMemoryStream = new(_readBuffer);
            _binaryWriter = new(_writeMemoryStream);
            _binaryReader = new(_readMemoryStream);
            InitializeMessages();
        }

        public long Write(NetworkMessageTypes messageType, MessageData messageData)
        {
            if (_messageByType.TryGetValue(messageType, out NetworkMessageBase message))
            {
                _isBlocked = true;
                _binaryWriter.BaseStream.Position = 2;
                message.Write(messageData);
                long packetSize = _binaryWriter.BaseStream.Position;
                _binaryWriter.BaseStream.Position = 0;
                _binaryWriter.Write((ushort)packetSize);
                if (messageType != NetworkMessageTypes.SendTransform)
                {
                    GameConsole.LogText(messageType.ToString() + $" {packetSize}", Color.yellow);
                }
                return packetSize;
            }
            return 0L;
        }

        public bool TryRead(int size)
        {
            try
            {
                Read(size);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                return false;
            }
            return true;
        }

        public void Read(int size)
        {
            _binaryReader.BaseStream.Position = 0;
            while (size > 0)
            {
                ushort packetSize = _binaryReader.ReadUInt16();
                Debug.Log($"Packet size: {packetSize}");
                Debug.Log($"Position: {_binaryReader.BaseStream.Position}");
                NetworkMessageTypes messageType = (NetworkMessageTypes)_binaryReader.ReadByte();
                Debug.Log($"Message type: {messageType}");
                Debug.Log($"Position: {_binaryReader.BaseStream.Position}");
                NetworkMessageBase message = _messageByType[messageType];
                message.Read();
                Debug.Log($"Position: {_binaryReader.BaseStream.Position}");
                if (messageType != NetworkMessageTypes.SendTransform)
                {
                    GameConsole.LogText(messageType.ToString() + $" {packetSize}", Color.yellow);
                }
                size -= packetSize;
                Debug.Log($"Current size: {size}");
            }
            //long size = _binaryReader.BaseStream.Position - 1;
        }
        #endregion

        #region Private Methods
        private void InitializeMessages()
        {
            _messageByType = new()
            {
                { NetworkMessageTypes.SendClientId, new SendClientIdMessage(_binaryWriter, _binaryReader) },
                { NetworkMessageTypes.SendChunkData, new SendChunkDataMessage(_binaryWriter, _binaryReader) },
                { NetworkMessageTypes.CreatePlayer, new CreatePlayerMessage(_binaryWriter, _binaryReader) },
                { NetworkMessageTypes.Disconnect, new DisconnectMessage(_binaryWriter, _binaryReader) },
                { NetworkMessageTypes.SendTransform, new SendTransformMessage(_binaryWriter, _binaryReader) },
            };
        }
        #endregion
    }
}
