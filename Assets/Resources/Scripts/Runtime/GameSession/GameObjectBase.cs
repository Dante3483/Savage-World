using SavageWorld.Runtime.Enums.Network;
using SavageWorld.Runtime.Network;
using SavageWorld.Runtime.Network.Messages;
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
        public virtual GameObjectBase CreateInstance(Vector3 position, Transform parent = null, bool isOwner = true)
        {
            GameObjectBase instance = Instantiate(this, position, Quaternion.identity, parent);
            instance.name = gameObject.name;
            instance.NetworkObject.IsOwner = isOwner;
            return instance;
        }

        public virtual void DestroySelf()
        {
            if (NetworkManager.Instance.IsMultiplayer)
            {
                if (NetworkObject.IsOwner)
                {
                    MessageData messageData = new()
                    {
                        LongNumber1 = NetworkObject.Id
                    };
                    NetworkManager.Instance.BroadcastMessage(NetworkMessageTypes.DestroyObject, messageData);
                    NetworkManager.Instance.NetworkObjects.DestroyObject(NetworkObject.Id);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
