using UnityEngine;
using UnityEngine.UI;

namespace SavageWorld.Runtime.UI
{
    public class UIAnimator : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private Sprite[] _sprites;
        [SerializeField]
        private float _timeToSetNewSprite;
        [SerializeField]
        private bool _loop = true;

        private int _index = 0;
        private Image _image;
        private float _time = 0;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods
        void Awake()
        {
            _image = GetComponent<Image>();
        }

        void FixedUpdate()
        {
            if (!_loop && _index == _sprites.Length)
            {
                return;
            }
            _time += Time.fixedDeltaTime;
            if (_time < _timeToSetNewSprite)
            {
                return;
            }
            _image.sprite = _sprites[_index];
            _time = 0;
            _index++;
            if (_index >= _sprites.Length)
            {
                if (_loop)
                {
                    _index = 0;
                }
            }
        }
        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion
    }
}
