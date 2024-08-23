using SavageWorld.Runtime.Enums.Network;
using System.IO;

namespace SavageWorld.Runtime.Network.Messages
{
    public class CreateEnvironmentMessage : NetworkMessageBase
    {
        #region Fields

        #endregion

        #region Properties
        protected override NetworkMessageTypes MessageType
        {
            get
            {
                return NetworkMessageTypes.CreateEnvironment;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public CreateEnvironmentMessage(BinaryWriter writer, BinaryReader reader) : base(writer, reader)
        {
        }
        #endregion

        #region Private Methods
        protected override void ReadData()
        {
            long globalObjectId = _reader.ReadInt64();
            long objectId = _reader.ReadInt64();
            bool isOwner = _reader.ReadBoolean();
            float x = _reader.ReadSingle();
            float y = _reader.ReadSingle();
            NetworkManager.Instance.NetworkObjects.CreateEnvironment(globalObjectId, objectId, new(x, y), isOwner);
        }

        protected override void WriteData(MessageData messageData)
        {
            _writer.Write(messageData.LongNumber1);
            _writer.Write(messageData.LongNumber2);
            _writer.Write(messageData.Bool1);
            _writer.Write(messageData.FloatNumber1);
            _writer.Write(messageData.FloatNumber2);
        }
        #endregion
    }
}
