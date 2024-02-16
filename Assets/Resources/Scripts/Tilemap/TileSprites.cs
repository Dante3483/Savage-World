using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap
{
    public struct TileSprites
    {
        #region Private fields
        private Sprite _blockSprite;
        private Sprite _backgroundSprite;
        private Sprite _liquidSprite;
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
        #endregion

        #region Methods

        #endregion
    }
}
