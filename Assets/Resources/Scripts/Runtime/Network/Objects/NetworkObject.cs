using UnityEngine;

namespace SavageWorld.Runtime.Network.Objects
{
    public class NetworkObject : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private long _id;
        [SerializeField]
        private bool _isOwner = false;
        private NetworkTransform _networkTransform;
        #endregion

        #region Properties
        public bool IsOwner
        {
            get
            {
                return _isOwner;
            }
        }

        public long Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
        private void Awake()
        {
            _networkTransform = GetComponent<NetworkTransform>();
        }

        public void SetOwner(bool value)
        {
            _isOwner = value;
        }

        public void UpdatePosition(float x, float y)
        {
            if (_networkTransform != null)
            {
                _networkTransform.NextPosition = new(x, y);
            }
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
