using UnityEngine;
using UnityEngine.UI;

namespace SavageWorld.Runtime.Player.Research.UI
{
    public class UILine : MonoBehaviour
    {
        #region Private fields
        [SerializeField]
        private RectTransform _rectTransform;
        [SerializeField]
        private Image _lineImage;

        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void SetData(Color color, Vector3 fromPosition, Vector3 toPosition)
        {
            _rectTransform.localPosition = fromPosition;
            _rectTransform.sizeDelta = new(Vector2.Distance(fromPosition, toPosition), _rectTransform.sizeDelta.y);
            _rectTransform.rotation = Quaternion.FromToRotation(Vector3.left, toPosition - fromPosition);
            SetColor(color);
        }

        public void SetColor(Color color)
        {
            _lineImage.color = color;
        }
        #endregion
    }
}