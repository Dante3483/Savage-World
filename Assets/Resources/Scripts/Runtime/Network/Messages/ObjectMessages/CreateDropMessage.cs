using Items;
using SavageWorld.Runtime.Enums.Network;
using SavageWorld.Runtime.Network.Objects;
using System;
using System.IO;

namespace SavageWorld.Runtime.Network.Messages
{
    public class CreateDropMessage : CreateObjectMessageBase
    {
        #region Fields

        #endregion

        #region Properties
        protected override NetworkMessageTypes MessageType
        {
            get
            {
                return NetworkMessageTypes.CreateDrop;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public CreateDropMessage(BinaryWriter writer, BinaryReader reader) : base(writer, reader)
        {
        }
        #endregion

        #region Private Methods
        protected override void ReadData()
        {
            long _ = _reader.ReadInt64();
            long objectId = _reader.ReadInt64();
            bool __ = _reader.ReadBoolean();
            float x = _reader.ReadSingle();
            float y = _reader.ReadSingle();
            ItemsID itemId = (ItemsID)_reader.ReadUInt16();
            int quantity = _reader.ReadInt32();
            ItemSO item = GameManager.Instance.ItemsAtlas.GetItemByID(itemId);
            Action createDropAction;
            if (_networkManager.IsServer)
            {
                NetworkObject existingObject = _networkManager.NetworkObjects.GetObjectById(objectId);
                if (existingObject != null)
                {
                    createDropAction = () =>
                    {
                        Drop drop = existingObject.GetComponent<Drop>();
                        drop.Quantity = quantity;
                    };
                }
                else
                {
                    float directionX = _reader.ReadSingle();
                    float directionY = _reader.ReadSingle();
                    createDropAction = () =>
                    {
                        Drop drop = DropManager.Instance.CreateDropWithForce(new(x, y), new(directionX, directionY), item, quantity);
                    };
                }

            }
            else
            {
                createDropAction = () =>
                {
                    NetworkObject existingObject = _networkManager.NetworkObjects.GetObjectById(objectId);
                    if (existingObject != null)
                    {
                        Drop drop = existingObject.GetComponent<Drop>();
                        drop.Quantity = quantity;
                    }
                    else
                    {
                        Drop drop = DropManager.Instance.CreateDropWithoutMessage(new(x, y), item, quantity, false);
                        NetworkManager.Instance.NetworkObjects.AddObjectToDictionary(objectId, drop.NetworkObject);
                    }
                };
            }
            ActionInMainThreadUtil.Instance.InvokeInNextUpdate(createDropAction);
        }

        protected override void WriteData(MessageData messageData)
        {
            base.WriteData(messageData);
            _writer.Write((ushort)messageData.IntNumber1);
            _writer.Write(messageData.IntNumber2);
            bool isThrowing = messageData.Bool1;
            if (isThrowing)
            {
                _writer.Write(messageData.FloatNumber3);
                _writer.Write(messageData.FloatNumber4);
            }
        }
        #endregion
    }
}
