using System.IO;

namespace SavageWorld.Runtime.Network.Messages
{
    public abstract class CreateObjectMessageBase : NetworkMessageBase
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        protected CreateObjectMessageBase(BinaryWriter writer, BinaryReader reader) : base(writer, reader)
        {

        }
        #endregion

        #region Private Methods
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
