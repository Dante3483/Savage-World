using SavageWorld.Runtime.Enums.Network;
using System.IO;

namespace SavageWorld.Runtime.Network.Messages
{
    public class SendTransformMessage : NetworkMessageBase
    {
        #region Fields
        private NetworkManager _networkManager;
        #endregion

        #region Properties
        protected override NetworkMessageTypes MessageType
        {
            get
            {
                return NetworkMessageTypes.SendTransform;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public SendTransformMessage(BinaryWriter writer, BinaryReader reader) : base(writer, reader)
        {
            _networkManager = NetworkManager.Instance;
        }
        #endregion

        #region Private Methods
        protected override void ReadData()
        {
            byte transformType = _reader.ReadByte();
            long id = _reader.ReadInt64();
            float x = _reader.ReadSingle();
            float y = _reader.ReadSingle();
            switch (transformType)
            {
                case 1:
                    {
                        _networkManager.NetworkObjects.UpdatePosition(id, x, y);
                    }
                    break;
                case 2:
                    {
                        _networkManager.NetworkObjects.UpdateRotation(id, x, y);
                    }
                    break;
                case 3:
                    {
                        _networkManager.NetworkObjects.UpdateScale(id, x, y);
                    }
                    break;
                default:
                    break;
            }
        }

        protected override void WriteData(MessageData messageData)
        {
            _writer.Write((byte)messageData.IntNumber1);
            _writer.Write(messageData.LongNumber1);
            _writer.Write(messageData.FloatNumber1);
            _writer.Write(messageData.FloatNumber2);
        }
        #endregion

    }
}
