using UnityEngine;

namespace CustomTilemap
{
    public struct TileSprites
    {
        #region Private fields
        private Sprite _blockSprite;
        private Sprite _wallSprite;
        private Sprite _liquidSprite;
        private Sprite _blockDamageSprite;
        private Sprite _wallDamageSprite;
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

        public Sprite WallSprite
        {
            get
            {
                return _wallSprite;
            }

            set
            {
                _wallSprite = value;
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

        public Sprite WallDamageSprite
        {
            get
            {
                return _wallDamageSprite;
            }

            set
            {
                _wallDamageSprite = value;
            }
        }
        #endregion

        #region Methods

        #endregion
    }
}
