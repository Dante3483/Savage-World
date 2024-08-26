using SavageWorld.Runtime.Enums.Network;
using System.IO;

namespace SavageWorld.Runtime.Network.Messages
{
    public class DestroyObjectMessage : NetworkMessageBase
    {
        #region Fields

        #endregion

        #region Properties
        protected override NetworkMessageTypes MessageType
        {
            get
            {
                return NetworkMessageTypes.DestroyObject;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public DestroyObjectMessage(BinaryWriter writer, BinaryReader reader) : base(writer, reader)
        {
        }
        #endregion

        #region Private Methods
        protected override void ReadData()
        {
            long objectId = _reader.ReadInt64();
            NetworkManager.Instance.NetworkObjects.DestroyObject(objectId);
            if (_networkManager.IsServer)
            {
                MessageData messageData = new()
                {
                    LongNumber1 = objectId,
                };
                _networkManager.BroadcastMessage(MessageType, messageData);
            }
        }

        protected override void WriteData(MessageData messageData)
        {
            _writer.Write(messageData.LongNumber1);
        }
        #endregion
    }
}
