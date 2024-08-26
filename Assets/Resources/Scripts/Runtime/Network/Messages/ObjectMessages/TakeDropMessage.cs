using SavageWorld.Runtime.Enums.Network;
using SavageWorld.Runtime.Network.Objects;
using System;
using System.IO;

namespace SavageWorld.Runtime.Network.Messages
{
    public class TakeDropMessage : NetworkMessageBase
    {
        #region Fields

        #endregion

        #region Properties
        protected override NetworkMessageTypes MessageType
        {
            get
            {
                return NetworkMessageTypes.TakeDrop;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public TakeDropMessage(BinaryWriter writer, BinaryReader reader) : base(writer, reader)
        {
        }
        #endregion

        #region Private Methods
        protected override void ReadData()
        {
            bool stopTaking = _reader.ReadBoolean();
            bool isTaken = _reader.ReadBoolean();
            long palyerId = _reader.ReadInt64();
            long dropId = _reader.ReadInt64();
            NetworkObject dropObject = _networkManager.NetworkObjects.GetObjectById(dropId);
            if (_networkManager.IsServer)
            {
                NetworkObject playerObject = _networkManager.NetworkObjects.GetObjectById(palyerId);
                ActionInMainThreadUtil.Instance.InvokeInNextUpdate(() =>
                {
                    Drop drop = dropObject.GetComponent<Drop>();
                    if (!drop.HasTarget)
                    {
                        Action<Drop> endAttractionCallback = (drop) =>
                        {
                            MessageData messageData = new()
                            {
                                Bool2 = true,
                                LongNumber2 = dropObject.Id
                            };
                            _networkManager.SendMessageTo(NetworkMessageTypes.TakeDrop, messageData, _senderId);
                        };
                        drop.SetTarget(playerObject.transform, endAttractionCallback);
                        MessageData messageData = new()
                        {
                            Bool2 = false,
                            LongNumber2 = dropObject.Id
                        };
                        _networkManager.BroadcastMessage(NetworkMessageTypes.TakeDrop, messageData, _senderId);
                    }
                    if (stopTaking)
                    {
                        drop.RemoveTarget(playerObject.transform);
                        MessageData messageData = new()
                        {
                            Bool2 = false,
                            LongNumber2 = dropObject.Id
                        };
                        _networkManager.BroadcastMessage(NetworkMessageTypes.TakeDrop, messageData, _senderId);
                    }
                });
            }
            else if (_networkManager.IsClient)
            {
                if (isTaken)
                {
                    ActionInMainThreadUtil.Instance.InvokeInNextUpdate(() =>
                    {
                        Drop drop = dropObject.GetComponent<Drop>();
                        drop.EndAttraction();
                    });
                }
                else
                {
                    ActionInMainThreadUtil.Instance.InvokeInNextUpdate(() =>
                    {
                        Drop drop = dropObject.GetComponent<Drop>();
                        drop.IsAnotherObjectTarget = !drop.IsAnotherObjectTarget;
                    });
                }
            }

        }

        protected override void WriteData(MessageData messageData)
        {
            _writer.Write(messageData.Bool1);
            _writer.Write(messageData.Bool2);
            _writer.Write(messageData.LongNumber1);
            _writer.Write(messageData.LongNumber2);
        }
        #endregion
    }
}
