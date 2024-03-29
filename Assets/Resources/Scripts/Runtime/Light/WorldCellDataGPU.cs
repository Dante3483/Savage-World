using UnityEngine;

namespace LightSystem
{
    public struct WorldCellDataGPU
    {
        #region Private fields
        private float _wallLightValue;
        private float _blockLightValue;
        private Color32 _wallLightColor;
        private Color32 _blockLightColor;
        private int _flags;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public float WallLightValue
        {
            get
            {
                return _wallLightValue;
            }

            set
            {
                _wallLightValue = value;
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

        public Color32 WallLightColor
        {
            get
            {
                return _wallLightColor;
            }

            set
            {
                _wallLightColor = value;
            }
        }

        public Color32 BlockLightColor
        {
            get
            {
                return _blockLightColor;
            }

            set
            {
                _blockLightColor = value;
            }
        }
        #endregion

        #region Methods

        #endregion
    }
}
