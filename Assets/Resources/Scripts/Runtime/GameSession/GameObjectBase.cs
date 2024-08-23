using SavageWorld.Runtime.Network.Objects;
using UnityEngine;

namespace SavageWorld.Runtime
{
    [RequireComponent(typeof(NetworkObject))]
    public abstract class GameObjectBase : MonoBehaviour
    {
        #region Fields
        private NetworkObject _networkObject;
        #endregion

        #region Properties
        public NetworkObject NetworkObject
        {
            get
            {
                if (_networkObject == null)
                {
                    _networkObject = GetComponent<NetworkObject>();
                }
                return _networkObject;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
        public abstract GameObjectBase CreateInstance(Vector3 position, bool isOwner = true);
        #endregion

        #region Private Methods

        #endregion
    }
}
