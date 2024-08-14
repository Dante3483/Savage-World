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

        #endregion
    }
}
