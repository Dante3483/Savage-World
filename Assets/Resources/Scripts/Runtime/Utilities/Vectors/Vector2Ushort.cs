using System;

namespace SavageWorld.Runtime.Utilities.Vectors
{
    [Serializable]
    public struct Vector2Ushort
    {
        #region Private fields

        #endregion

        #region Public fields
        public ushort x;
        public ushort y;
        #endregion

        #region Properties

        #endregion

        #region Methods
        public Vector2Ushort(ushort x, ushort y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2Ushort(int x, int y)
        {
            this.x = (ushort)x;
            this.y = (ushort)y;
        }
        #endregion
    }
}