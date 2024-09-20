using SavageWorld.Runtime.Enums.Network;
using System.IO;

namespace SavageWorld.Runtime.Network.Messages
{
    public class SendEntityAnimationMessage : NetworkMessageBase
    {
        #region Fields

        #endregion

        #region Properties
        protected override NetworkMessageTypes MessageType
        {
            get
            {
                return NetworkMessageTypes.SendEntityAnimation;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public SendEntityAnimationMessage(BinaryWriter writer, BinaryReader reader) : base(writer, reader)
        {
        }
        #endregion

        #region Private Methods
        protected override void ReadData()
        {
            long objectId = _reader.ReadInt64();
            int animationHash = _reader.ReadInt32();

            _networkManager.NetworkObjects.GetObjectById(objectId).UpdateAnimation(animationHash);
            if (_networkManager.IsServer)
            {
                MessageData messageData = new()
                {
                    LongNumber1 = objectId,
                    IntNumber1 = animationHash,
                };
                _networkManager.BroadcastMessage(NetworkMessageTypes.SendEntityAnimation, messageData, _senderId);
            }
        }

        protected override void WriteData(MessageData messageData)
        {
            _writer.Write(messageData.LongNumber1);
            _writer.Write(messageData.IntNumber1);
        }
        #endregion
    }
}
