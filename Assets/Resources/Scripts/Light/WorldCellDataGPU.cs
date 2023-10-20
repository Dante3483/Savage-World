using System.Drawing;
using Unity.Mathematics;
using UnityEngine;

namespace LightSystem
{
    public struct WorldCellDataGPU
    {
        #region Private fields
        private float _bgLightValue;
        private float _blockLightValue;
        private Color32 _bgLightColor;
        private Color32 _blockLightColor;
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

        public Color32 BgLightColor
        {
            get
            {
                return _bgLightColor;
            }

            set
            {
                _bgLightColor = value;
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
