using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap
{
    public class TileLayer : MonoBehaviour
    {
        #region Private fields
        [SerializeField] private SpriteRenderer _spriteRenderer;
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
            _spriteRenderer.sprite = sprite;
        }

        public void UpdateOrderInLayer(int orderInLayer)
        {
            _spriteRenderer.sortingOrder = orderInLayer;
        }
        #endregion
    }
}
