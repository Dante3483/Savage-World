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

        }

        protected override void WriteData(MessageData messageData)
        {

        }
        #endregion
    }
}
