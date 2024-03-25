using UnityEngine;

namespace CustomTilemap
{
    public class TileLayer : MonoBehaviour
    {
        #region Private fields
        [SerializeField] private SpriteRenderer _mainSpriteRenderer;
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
        public void UpdateSprite(Sprite mainSprite)
        {
            gameObject.layer = mainSprite == null ? _defaultLayerMask : _blockLayerMask;
            _mainSpriteRenderer.sprite = mainSprite;
        }
        #endregion
    }
}
