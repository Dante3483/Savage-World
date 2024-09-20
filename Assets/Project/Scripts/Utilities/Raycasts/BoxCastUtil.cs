using SavageWorld.Runtime.Utilities.DebugOnly;
using System;
using UnityEngine;

namespace SavageWorld.Runtime.Utilities.Raycasts
{
    [Serializable]
    public class BoxCastUtil
    {
        #region Fields
        [SerializeField]
        private bool _needVisualize;
        [SerializeField]
        private bool _checkYourself;
        [SerializeField]
        private bool _result;
        [SerializeField]
        private Vector2 _originOffset;
        [Min(0.001f)]
        [SerializeField]
        private Vector2 _size;
        [SerializeField]
        private float _angle;
        [SerializeField]
        private Vector2 _direction;
        [SerializeField]
        private float _distance;
        [SerializeField]
        private LayerMask _layerMask;
        [SerializeField]
        private Color _hitColor;
        [SerializeField]
        private Color _notHitColor;
        private GameObject _self;
        #endregion

        #region Properties
        public bool NeedVisualize
        {
            set
            {
                _needVisualize = value;
            }
        }

        public bool CheckYourself
        {
            set
            {
                _checkYourself = value;
            }
        }

        public bool Result
        {
            get
            {
                return _result;
            }
        }

        public Vector2 OriginOffset
        {
            set
            {
                _originOffset = value;
            }
        }

        public Vector2 Size
        {
            set
            {
                _size = value;
            }
        }

        public float Angle
        {
            set
            {
                _angle = value;
            }
        }

        public Vector2 Direction
        {
            set
            {
                _direction = value;
            }
        }

        public float Distance
        {
            set
            {
                _distance = value;
            }
        }

        public LayerMask LayerMask
        {
            set
            {
                _layerMask = value;
            }
        }

        public Color HitColor
        {
            set
            {
                _hitColor = value;
            }
        }

        public Color NotHitColor
        {
            set
            {
                _notHitColor = value;
            }
        }

        public GameObject Self
        {
            set
            {
                _self = value;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
        public BoxCastUtil()
        {
            _size = new(1, 1);
            _hitColor = Color.green;
            _notHitColor = Color.red;
        }

        public RaycastHit2D BoxCast(Vector2 origin)
        {
            Color rayColor;
            origin += _originOffset;
            RaycastHit2D hit = Physics2D.BoxCast(origin, _size, _angle, _direction, _distance, _layerMask);

            _result = hit;
            if (_checkYourself && _self != null)
            {
                _result = _result && hit.collider.gameObject != _self;
            }
            rayColor = _result ? _hitColor : _notHitColor;

            if (_needVisualize)
            {
                DebugUtil.DrawBox(origin, _size, _direction, _distance, rayColor);
            }
            return hit;
        }

        public RaycastHit2D[] BoxCastAll(Vector2 origin)
        {
            Color rayColor;
            origin += _originOffset;
            RaycastHit2D[] hits = Physics2D.BoxCastAll(origin, _size, _angle, _direction, _distance, _layerMask);

            if (_checkYourself && _self != null)
            {
                _result = hits.Length > 1;
            }
            else
            {
                _result = hits.Length != 0;
            }
            rayColor = _result ? _hitColor : _notHitColor;

            if (_needVisualize)
            {
                DebugUtil.DrawBox(origin, _size, _direction, _distance, rayColor);
            }
            return hits;
        }

        public void SetSizeX(float xValue)
        {
            _size.x = xValue;
        }

        public void SetSizeY(float yValue)
        {
            _size.y = yValue;
        }

        public void SetOffsetX(float xValue)
        {
            _originOffset.x = xValue;
        }

        public void SetOffsetY(float yValue)
        {
            _originOffset.y = yValue;
        }
        #endregion

        #region Private Methods

        #endregion
    }
}