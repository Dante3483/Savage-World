using SavageWorld.Runtime.Enums.Network;
using System.IO;

namespace SavageWorld.Runtime.Network.Messages
{
    public class SendPositionMessage : NetworkMessageBase
    {
        #region Fields
        private NetworkManager _networkManager;
        #endregion

        #region Properties
        protected override NetworkMessageTypes MessageType
        {
            get
            {
                return NetworkMessageTypes.SendPosition;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public SendPositionMessage(BinaryWriter writer, BinaryReader reader) : base(writer, reader)
        {
            _networkManager = NetworkManager.Instance;
        }
        #endregion

        #region Private Methods
        protected override void ReadData()
        {
            long id = _reader.ReadInt64();
            float x = _reader.ReadSingle();
            float y = _reader.ReadSingle();
            _networkManager.UpdateObjectPosition(id, x, y);
        }

        protected override void WriteData(MessageData messageData)
        {
            _writer.Write(messageData.LongNumber1);
            _writer.Write(messageData.FloatNumber1);
            _writer.Write(messageData.FloatNumber2);
        }
        #endregion

    }
}
