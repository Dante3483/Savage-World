using Items;
using SavageWorld.Runtime.Enums.Network;
using SavageWorld.Runtime.Network.Objects;
using System.IO;
using UnityEngine;

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
            byte flags = _reader.ReadByte();
            int quantity = _reader.ReadInt32();
            bool isThrowing = _reader.ReadBoolean();
            float directionX = isThrowing ? _reader.ReadSingle() : 0f;
            float directionY = isThrowing ? _reader.ReadSingle() : 0f;
            ItemSO item = GameManager.Instance.ItemsAtlas.GetItemByID(itemId);
            ActionInMainThreadUtil.Instance.InvokeInNextUpdate(() =>
            {
                NetworkObject dropObject = _networkManager.NetworkObjects.GetObjectById(objectId);
                AddQuantityToExistingObject(dropObject, quantity);
                if (_networkManager.IsServer)
                {
                    CreateNewDropWithForceOnServer(dropObject, new(x, y), new(directionX, directionY), item, quantity);
                }
                else if (_networkManager.IsClient)
                {
                    CreateNewDropOnClient(dropObject, objectId, new(x, y), item, quantity, flags);
                }
            });
        }

        protected override void WriteData(MessageData messageData)
        {
            base.WriteData(messageData);
            _writer.Write((ushort)messageData.IntNumber1);
            _writer.Write((byte)messageData.IntNumber2);
            _writer.Write(messageData.IntNumber3);
            _writer.Write(messageData.Bool1);
            if (messageData.Bool1)
            {
                _writer.Write(messageData.FloatNumber3);
                _writer.Write(messageData.FloatNumber4);
            }
        }

        private void AddQuantityToExistingObject(NetworkObject dropObject, int quantity)
        {
            if (dropObject == null)
            {
                return;
            }
            Drop drop = dropObject.GetComponent<Drop>();
            drop.Quantity = quantity;
        }

        private void CreateNewDropWithForceOnServer(NetworkObject dropObject, Vector2 position, Vector2 direction, ItemSO item, int quantity)
        {
            if (dropObject != null)
            {
                return;
            }
            Drop drop = DropManager.Instance.CreateDropWithForce(position, direction, item, quantity);
        }

        private void CreateNewDropOnClient(NetworkObject dropObject, long dropId, Vector2 position, ItemSO item, int quantity, byte flags)
        {
            if (dropObject != null)
            {
                return;
            }
            Drop drop = DropManager.Instance.CreateDropWithoutMessage(position, item, quantity, false);
            drop.Flags = flags;
            drop.StartAttractionCooldown();
            NetworkManager.Instance.NetworkObjects.AddObjectToDictionary(dropId, drop.NetworkObject);
        }
        #endregion
    }
}
