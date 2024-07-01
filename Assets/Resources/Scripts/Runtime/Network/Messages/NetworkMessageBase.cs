using SavageWorld.Runtime.Enums.Network;
using System.IO;

namespace SavageWorld.Runtime.Network.Messages
{
    public abstract class NetworkMessageBase
    {
        #region Fields
        protected BinaryWriter _writer;
        protected BinaryReader _reader;
        #endregion

        #region Properties
        protected abstract NetworkMessageTypes MessageType { get; }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public NetworkMessageBase(BinaryWriter writer, BinaryReader reader)
        {
            _writer = writer;
            _reader = reader;
        }

        public bool Write(int number1)
        {
            if (!WriteType())
            {
                return false;
            }
            return WriteData(number1);
        }

        public bool Read()
        {
            return ReadData();
        }
        #endregion

        #region Private Methods
        protected abstract bool WriteData(int number1);

        protected abstract bool ReadData();

        protected bool WriteType()
        {
            try
            {
                _writer.Write((byte)MessageType);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
