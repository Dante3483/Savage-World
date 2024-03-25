using UnityEngine;

namespace CustomTilemap
{
    public struct TileSprites
    {
        #region Private fields
        private Sprite _blockSprite;
        private Sprite _backgroundSprite;
        private Sprite _liquidSprite;
        private Sprite _blockDamageSprite;
        private Sprite _backgroundDamageSprite;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public Sprite BlockSprite
        {
            get
            {
                return _blockSprite;
            }

            set
            {
                _blockSprite = value;
            }
        }

        public Sprite LiquidSprite
        {
            get
            {
                return _liquidSprite;
            }

            set
            {
                _liquidSprite = value;
            }
        }

        public Sprite BackgroundSprite
        {
            get
            {
                return _backgroundSprite;
            }

            set
            {
                _backgroundSprite = value;
            }
        }

        public Sprite BlockDamageSprite
        {
            get
            {
                return _blockDamageSprite;
            }

            set
            {
                _blockDamageSprite = value;
            }
        }

        public Sprite BackgroundDamageSprite
        {
            get
            {
                return _backgroundDamageSprite;
            }

            set
            {
                _backgroundDamageSprite = value;
            }
        }
        #endregion

        #region Methods

        #endregion
    }
}
