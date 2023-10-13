using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap
{
    public class TileLayer : MonoBehaviour
    {
        #region Private fields
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private SpriteMask _spriteMask;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void UpdateSprite(Sprite sprite)
        {
            if (sprite == null)
            {
                _spriteRenderer.enabled = false;
            }
            else
            {
                _spriteRenderer.enabled = true;
            }
            _spriteRenderer.sprite = sprite;
        }
        #endregion
    }
}
