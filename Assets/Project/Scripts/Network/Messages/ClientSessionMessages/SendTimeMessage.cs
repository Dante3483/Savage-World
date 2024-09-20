using SavageWorld.Runtime.Enums.Network;
using SavageWorld.Runtime.GameSession;
using System.IO;

namespace SavageWorld.Runtime.Network.Messages
{
    public class SendTimeMessage : NetworkMessageBase
    {
        #region Fields

        #endregion

        #region Properties
        protected override NetworkMessageTypes MessageType
        {
            get
            {
                return NetworkMessageTypes.SendTime;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public SendTimeMessage(BinaryWriter writer, BinaryReader reader) : base(writer, reader)
        {
        }
        #endregion

        #region Private Methods
        protected override void ReadData()
        {
            ulong globalTime = _reader.ReadUInt64();
            GameTime.Instance.SetTime(globalTime);
        }

        protected override void WriteData(MessageData messageData)
        {
            _writer.Write(GameTime.Instance.GlobalTime);
        }
        #endregion
    }
}
