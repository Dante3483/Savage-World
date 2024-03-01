using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap
{
    public class TileLayer : MonoBehaviour
    {
        #region Private fields
        [SerializeField] private SpriteRenderer _spriteRenderer;
        private LayerMask _defaultLayerMask;
        private LayerMask _blockLayerMask;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        private void Awake()
        {
            _defaultLayerMask = LayerMask.NameToLayer("Default");
            _blockLayerMask = LayerMask.NameToLayer("Block");
        }

        public void UpdateSprite(Sprite sprite)
        {
            gameObject.layer = sprite == null ? _defaultLayerMask : _blockLayerMask;
            _spriteRenderer.sprite = sprite;
        }

        public void UpdateOrderInLayer(int orderInLayer)
        {
            _spriteRenderer.sortingOrder = orderInLayer;
        }
        #endregion
    }
}
