using SavageWorld.Runtime.Enums.Network;
using SavageWorld.Runtime.Network.Messages;
using UnityEngine;

namespace SavageWorld.Runtime.Network.Objects
{
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkTransform : MonoBehaviour
    {
        #region Fields
        [Header("Position")]
        [SerializeField]
        private bool _syncPosition;
        [SerializeField]
        [Min(0.001f)]
        private float _positionThreshold;
        private bool _isPositionChanged;
        private Vector3 _prevPosition;
        private Vector2 _networkPosition;

        [Header("Rotation")]
        [SerializeField]
        private bool _syncRotation;
        private bool _isRotationChanged;
        private Vector3 _prevRotation;
        private Vector2 _networkRotation;

        [Header("Scale")]
        [SerializeField]
        private bool _syncScale;
        private bool _isScaleChanged;
        private Vector3 _prevScale;
        private Vector2 _networkScale = Vector2.one;

        private NetworkObject _networkObject;
        #endregion

        #region Properties
        public Vector2 NetworkPosition
        {
            get
            {
                return _networkPosition;
            }

            set
            {
                _isPositionChanged = true;
                _networkPosition = value;
            }
        }

        public Vector2 NetworkRotation
        {
            get
            {
                return _networkRotation;
            }

            set
            {
                _isRotationChanged = true;
                _networkRotation = value;
            }
        }

        public Vector2 NetworkScale
        {
            get
            {
                return _networkScale;
            }

            set
            {
                _isScaleChanged = true;
                _networkScale = value;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods
        private void Awake()
        {
            _networkObject = GetComponent<NetworkObject>();
        }

        private void Update()
        {
            if (!NetworkManager.Instance.IsMultiplayer)
            {
                return;
            }
            if (_networkObject.IsOwner)
            {
                if (transform.hasChanged)
                {
                    SyncPosition();
                    SyncRotation();
                    SyncScale();
                    transform.hasChanged = false;
                }
            }
            else
            {
                transform.position = _isPositionChanged ? _networkPosition : transform.position;
                transform.rotation = _isRotationChanged ? Quaternion.Euler(_networkRotation) : transform.rotation;
                transform.localScale = _isScaleChanged ? _networkScale : transform.localScale;
            }
        }
        #endregion

        #region Public Methods

        #endregion

        #region Private Methods
        private void SyncPosition()
        {
            if (!_syncPosition)
            {
                return;
            }
            if (_prevPosition != transform.position)
            {
                Vector2 position = transform.position;
                float difference = Vector2.Distance(position, _prevPosition);
                if (difference >= _positionThreshold)
                {
                    MessageData messageData = new()
                    {
                        IntNumber1 = 1,
                        LongNumber1 = _networkObject.Id,
                        FloatNumber1 = position.x,
                        FloatNumber2 = position.y,
                    };
                    NetworkManager.Instance.BroadcastMessage(NetworkMessageTypes.SendTransform, messageData);
                    _prevPosition = position;
                }
            }
        }

        private void SyncRotation()
        {
            if (!_syncRotation)
            {
                return;
            }
            if (_prevRotation != transform.rotation.eulerAngles)
            {
                Vector2 rotation = transform.rotation.eulerAngles;
                MessageData messageData = new()
                {
                    IntNumber1 = 2,
                    LongNumber1 = _networkObject.Id,
                    FloatNumber1 = rotation.x,
                    FloatNumber2 = rotation.y,
                };
                NetworkManager.Instance.BroadcastMessage(NetworkMessageTypes.SendTransform, messageData);
                _prevPosition = rotation;
            }
        }

        private void SyncScale()
        {
            if (!_syncScale)
            {
                return;
            }
            if (_prevScale != transform.localScale)
            {
                Vector2 scale = transform.localScale;
                MessageData messageData = new()
                {
                    IntNumber1 = 3,
                    LongNumber1 = _networkObject.Id,
                    FloatNumber1 = scale.x,
                    FloatNumber2 = scale.y,
                };
                NetworkManager.Instance.BroadcastMessage(NetworkMessageTypes.SendTransform, messageData);
                _prevPosition = scale;
            }
        }
        #endregion
    }
}
