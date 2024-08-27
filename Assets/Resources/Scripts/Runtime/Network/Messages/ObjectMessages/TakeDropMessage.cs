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
            bool needToRemoveTarget = _reader.ReadBoolean();
            bool isDropReachedTarget = _reader.ReadBoolean();
            bool needToToggleTargetFlag = _reader.ReadBoolean();
            long palyerId = _reader.ReadInt64();
            long dropId = _reader.ReadInt64();
            NetworkObject playerObject = _networkManager.NetworkObjects.GetObjectById(palyerId);
            NetworkObject dropObject = _networkManager.NetworkObjects.GetObjectById(dropId);
            ActionInMainThreadUtil.Instance.InvokeInNextUpdate(() =>
            {
                if (dropObject == null)
                {
                    return;
                }
                Drop drop = dropObject.GetComponent<Drop>();
                if (_networkManager.IsServer)
                {
                    SetTargetOnServer(playerObject, dropObject, drop);
                    RemoveTargetOnServer(playerObject, dropObject, drop, needToRemoveTarget);
                }
                else if (_networkManager.IsClient)
                {
                    CompleteAttractionOnClient(dropObject, drop, isDropReachedTarget);
                    ToggleBusyFlagOnClient(dropObject, drop, isDropReachedTarget);
                }
            });
        }

        protected override void WriteData(MessageData messageData)
        {
            _writer.Write(messageData.Bool1);
            _writer.Write(messageData.Bool2);
            _writer.Write(messageData.Bool3);
            _writer.Write(messageData.LongNumber1);
            _writer.Write(messageData.LongNumber2);
        }

        private void SetTargetOnServer(NetworkObject playerObject, NetworkObject dropObject, Drop drop)
        {
            if (drop.HasTarget)
            {
                return;
            }
            Action<Drop> endAttractionCallback = (drop) => _networkManager.SendMessageTo(NetworkMessageTypes.TakeDrop, new() { Bool2 = true, LongNumber2 = dropObject.Id }, _senderId);
            drop.SetTarget(playerObject.transform, endAttractionCallback);
            _networkManager.BroadcastMessage(NetworkMessageTypes.TakeDrop, new() { Bool3 = true, LongNumber2 = dropObject.Id }, _senderId);
        }

        private void RemoveTargetOnServer(NetworkObject playerObject, NetworkObject dropObject, Drop drop, bool needToRemoveTarget)
        {
            if (!needToRemoveTarget)
            {
                return;
            }
            drop.RemoveTarget(playerObject.transform);
            _networkManager.BroadcastMessage(NetworkMessageTypes.TakeDrop, new() { Bool3 = true, LongNumber2 = dropObject.Id }, _senderId);
        }

        private void CompleteAttractionOnClient(NetworkObject dropObject, Drop drop, bool isDropReachedTarget)
        {
            if (!isDropReachedTarget)
            {
                return;
            }
            drop.EndAttraction();
        }

        private void ToggleBusyFlagOnClient(NetworkObject dropObject, Drop drop, bool needToToggleTargetFlag)
        {
            if (needToToggleTargetFlag)
            {
                drop.IsAnotherObjectTarget = !drop.IsAnotherObjectTarget;
            }

        }
        #endregion
    }
}
