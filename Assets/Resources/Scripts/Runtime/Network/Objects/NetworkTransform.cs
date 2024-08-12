using SavageWorld.Runtime.Enums.Network;
using System;
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
        private Vector3 _prevPosition;
        private Vector2 _networkPosition;

        [Header("Rotation")]
        [SerializeField]
        private bool _syncRotation;
        private Vector3 _prevRotation;
        private Vector2 _networkRotation;

        [Header("Scale")]
        [SerializeField]
        private bool _syncScale;
        private Vector3 _prevScale;
        private Vector2 _networkScale;

        private NetworkObject _networkObject;
        #endregion

        #region Properties
        public Vector2 NextPosition
        {
            get
            {
                return _networkPosition;
            }

            set
            {
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
                _networkScale = value;
            }
        }
        #endregion

        #region Events / Delegates
        public event Action<float, float> TransformChanged;
        #endregion

        #region Monobehaviour Methods
        private void Awake()
        {
            _networkObject = GetComponent<NetworkObject>();
        }

        private void Update()
        {
            if (_networkObject.IsOwner)
            {
                if (transform.hasChanged)
                {
                    SyncPosition();
                    SyncRotation();
                    SyncScale();
                }
            }
            else
            {
                transform.position = _networkPosition;
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
                    NetworkManager.Instance.SendMessageToServer(NetworkMessageTypes.SendPosition, new MessageData { LongNumber1 = _networkObject.Id, FloatNumber1 = position.x, FloatNumber2 = position.y });
                    _prevPosition = position;
                }
            }
        }

        private void SyncRotation()
        {
            //if (!_syncRotation)
            //{
            //    return;
            //}
            //if (_prevRotation != transform.rotation.eulerAngles)
            //{
            //    Vector2 position = transform.position;
            //    float difference = Vector2.Distance(position, _prevPosition);
            //    if (difference >= _positionThreshold)
            //    {
            //        TransformChanged?.Invoke(position.x, position.y);
            //        _prevPosition = position;
            //    }
            //}
        }

        private void SyncScale()
        {
            //if (!_syncScale)
            //{
            //    return;
            //}
            //if (_prevPosition != transform.position)
            //{
            //    Vector2 position = transform.position;
            //    float difference = Vector2.Distance(position, _prevPosition);
            //    if (difference >= _positionThreshold)
            //    {
            //        TransformChanged?.Invoke(position.x, position.y);
            //        _prevPosition = position;
            //    }
            //}
        }
        #endregion
    }
}
