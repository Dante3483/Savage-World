namespace LightSystem
{
    public struct WorldCellDataGPU
    {
        #region Private fields
        private float _bgLightValue;
        private float _blockLightValue;
        private int _flags;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public float BgLightValue
        {
            get
            {
                return _bgLightValue;
            }

            set
            {
                _bgLightValue = value;
            }
        }

        public float BlockLightValue
        {
            get
            {
                return _blockLightValue;
            }

            set
            {
                _blockLightValue = value;
            }
        }

        public int Flags
        {
            get
            {
                return _flags;
            }

            set
            {
                _flags = value;
            }
        }
        #endregion

        #region Methods

        #endregion
    }
}
