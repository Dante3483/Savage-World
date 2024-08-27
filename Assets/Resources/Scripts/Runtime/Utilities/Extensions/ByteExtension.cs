namespace SavageWorld.Runtime.Utilities.Extensions
{
    public static class ByteExtension
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public static byte SetBit(this byte data, int bitNumber, bool bitValue)
        {
            if (bitNumber >= 8 || bitNumber < 0)
            {
                return 0;
            }
            return (byte)(bitValue ? data | (1 << bitNumber) : data & ~(1 << bitNumber));
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
