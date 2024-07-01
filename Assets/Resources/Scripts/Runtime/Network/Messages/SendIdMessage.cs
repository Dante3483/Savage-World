using SavageWorld.Runtime.Enums.Network;
using System;
using System.IO;

namespace SavageWorld.Runtime.Network.Messages
{
    public class SendIdMessage : NetworkMessageBase
    {
        #region Fields

        #endregion

        #region Properties
        protected override NetworkMessageTypes MessageType
        {
            get
            {
                return NetworkMessageTypes.SendId;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public SendIdMessage(BinaryWriter writer, BinaryReader reader) : base(writer, reader)
        {

        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Client read id and set it
        /// </summary>
        /// <returns></returns>
        protected override bool ReadData()
        {
            try
            {
                int id = _reader.ReadInt32();
                NetworkManager.Instance.Client.SetId(id);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Server send new client its id
        /// </summary>
        /// <param name="number1">Id of client</param>
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
