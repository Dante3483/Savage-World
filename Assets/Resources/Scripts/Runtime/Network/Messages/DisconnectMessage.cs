using SavageWorld.Runtime.Enums.Network;
using System;
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
        /// <returns></returns>
        protected override bool ReadData()
        {
            try
            {
                int id = _reader.ReadInt32();
                NetworkManager.Instance.Server.DisconnectClient(id);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Client send id to server
        /// </summary>
        /// <param name="number1">Client id</param>
        /// <returns></returns>
        protected override bool WriteData(int number1)
        {
            try
            {
                _writer.Write(number1);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
    }
}
