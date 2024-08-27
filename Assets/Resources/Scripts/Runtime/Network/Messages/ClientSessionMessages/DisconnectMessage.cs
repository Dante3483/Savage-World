using SavageWorld.Runtime.Enums.Network;
using SavageWorld.Runtime.Utilities;
using System.IO;

namespace SavageWorld.Runtime.Network.Messages
{
    public class DisconnectMessage : NetworkMessageBase
    {
        #region Fields

        #endregion

        #region Properties
        protected override NetworkMessageTypes MessageType
        {
            get
            {
                return NetworkMessageTypes.Disconnect;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public DisconnectMessage(BinaryWriter writer, BinaryReader reader) : base(writer, reader)
        {

        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Server read client's id and disconnect it
        /// </summary>
        protected override void ReadData()
        {
            MainThreadUtility.Instance.InvokeAndWait(() => NetworkManager.Instance.DisconnectClient(_senderId));
        }

        /// <summary>
        /// Client send id to server
        /// </summary>
        /// <param name="messageData">Client's id</param>
        protected override void WriteData(MessageData messageData)
        {

        }
        #endregion
    }
}
