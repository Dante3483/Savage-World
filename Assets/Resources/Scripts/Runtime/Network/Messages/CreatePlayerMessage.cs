using SavageWorld.Runtime.Enums.Network;
using System.IO;

namespace SavageWorld.Runtime.Network.Messages
{
    public class CreatePlayerMessage : NetworkMessageBase
    {
        #region Fields

        #endregion

        #region Properties
        protected override NetworkMessageTypes MessageType
        {
            get
            {
                return NetworkMessageTypes.CreatePlayer;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public CreatePlayerMessage(BinaryWriter writer, BinaryReader reader) : base(writer, reader)
        {
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Client reads id and create player
        /// </summary>
        protected override void ReadData()
        {
            long id = _reader.ReadInt64();
            bool isOwner = _reader.ReadBoolean();
            NetworkManager.Instance.CreatePlayer(id, isOwner);
        }

        /// <summary>
        /// Server sends unique id for new player
        /// </summary>
        /// <param name="messageData">player id</param>
        protected override void WriteData(MessageData messageData)
        {
            _writer.Write(messageData.LongNumber1);
            _writer.Write(messageData.Bool1);
        }
        #endregion
    }
}
