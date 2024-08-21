using SavageWorld.Runtime.Network.Objects;
using UnityEngine;

namespace SavageWorld.Runtime
{
    public class NetworkAnimator : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private NetworkObject _networkObject;
        [SerializeField]
        private Animator _animator;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods
        private void Awake()
        {
            _networkObject = GetComponent<NetworkObject>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {

        }
        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion
    }
}
