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
                if (messageType != NetworkMessageTypes.SendTransform &&
                    messageType != NetworkMessageTypes.SendWorldCellData &&
                    messageType != NetworkMessageTypes.AddDamageToTile &&
                    messageType != NetworkMessageTypes.SendEntityAnimation &&
                    messageType != NetworkMessageTypes.TakeDrop)
                {
                    GameConsole.Log(messageType.ToString() + $" {packetSize}", Color.yellow);
                    Debug.Log(messageType.ToString() + $" {packetSize}");
                }
                return packetSize;
            }
            else
            {
                GameConsole.Log($"Message {messageType} not found in dictionary");
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
                NetworkMessageTypes messageType = (NetworkMessageTypes)_binaryReader.ReadByte();
                NetworkMessageBase message = _messageByType[messageType];
                message.Read();
                if (messageType != NetworkMessageTypes.SendTransform &&
                    messageType != NetworkMessageTypes.SendWorldCellData &&
                    messageType != NetworkMessageTypes.AddDamageToTile &&
                    messageType != NetworkMessageTypes.SendEntityAnimation &&
                    messageType != NetworkMessageTypes.TakeDrop)
                {
                    GameConsole.Log(messageType.ToString() + $" {packetSize}", Color.yellow);
                    Debug.Log(messageType.ToString() + $" {packetSize}");
                }
                size -= packetSize;
            }
        }
        #endregion

        #region Private Methods
        private void InitializeMessages()
        {
            _messageByType = new()
            {
                { NetworkMessageTypes.SendClientId, new SendClientIdMessage(_binaryWriter, _binaryReader) },
                { NetworkMessageTypes.SendChunkData, new SendChunkDataMessage(_binaryWriter, _binaryReader) },
                { NetworkMessageTypes.SendTime, new SendTimeMessage(_binaryWriter, _binaryReader) },
                { NetworkMessageTypes.Disconnect, new DisconnectMessage(_binaryWriter, _binaryReader) },
                { NetworkMessageTypes.CreatePlayer, new CreatePlayerMessage(_binaryWriter, _binaryReader) },
                { NetworkMessageTypes.CreateEnvironment, new CreateEnvironmentMessage(_binaryWriter, _binaryReader) },
                { NetworkMessageTypes.CreateDrop, new CreateDropMessage(_binaryWriter, _binaryReader) },
                { NetworkMessageTypes.DestroyObject, new DestroyObjectMessage(_binaryWriter, _binaryReader) },
                { NetworkMessageTypes.SendTransform, new SendTransformMessage(_binaryWriter, _binaryReader) },
                { NetworkMessageTypes.SendWorldCellData, new SendWorldCellDataMessage(_binaryWriter, _binaryReader) },
                { NetworkMessageTypes.SendEntityAnimation, new SendEntityAnimationMessage(_binaryWriter, _binaryReader) },
                { NetworkMessageTypes.AddDamageToTile, new AddDamageToTileMessage(_binaryWriter, _binaryReader) },
                { NetworkMessageTypes.TakeDrop, new TakeDropMessage(_binaryWriter, _binaryReader) },
            };
        }
        #endregion
    }
}
