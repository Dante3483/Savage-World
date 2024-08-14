using SavageWorld.Runtime.Enums.Network;
using System.IO;

namespace SavageWorld.Runtime.Network.Messages
{
    public class SendClientIdMessage : NetworkMessageBase
    {
        #region Fields

        #endregion

        #region Properties
        protected override NetworkMessageTypes MessageType
        {
            get
            {
                return NetworkMessageTypes.SendClientId;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public SendClientIdMessage(BinaryWriter writer, BinaryReader reader) : base(writer, reader)
        {

        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Client set its id
        /// </summary>
        protected override void ReadData()
        {
            int id = _reader.ReadInt32();
            NetworkManager.Instance.Client.SetId(id);
        }

        /// <summary>
        /// Server writes client's id
        /// </summary>
        /// <param name="messageData">client id</param>
        protected override void WriteData(MessageData messageData)
        {
            _writer.Write(messageData.IntNumber1);
        }
        #endregion
    }
}
