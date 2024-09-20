using SavageWorld.Runtime.Enums.Network;
using SavageWorld.Runtime.Network.Messages;
using UnityEngine;

namespace SavageWorld.Runtime.Network.Objects
{
    public class NetworkAnimator : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private NetworkObject _networkObject;
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private int _currentAnimationHash;
        [SerializeField]
        private bool _isAnimationChanged;
        [SerializeField]
        private int _networkAnimationHash;
        #endregion

        #region Properties
        public int NetworkAnimationHash
        {
            get
            {
                return _networkAnimationHash;
            }

            set
            {
                _networkAnimationHash = value;
            }
        }
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
            if (!NetworkManager.Instance.IsMultiplayer)
            {
                return;
            }
            if (_networkObject.IsOwner)
            {
                SyncAnimation();
            }
            else
            {
                if (_isAnimationChanged)
                {
                    _animator.Play(_networkAnimationHash);
                    _isAnimationChanged = false;
                }
            }
        }
        #endregion

        #region Public Methods
        public void SetNewAnimation(int animationHash)
        {
            _networkAnimationHash = animationHash;
            _isAnimationChanged = true;
        }
        #endregion

        #region Private Methods
        private void SyncAnimation()
        {
            int animationHash = _animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
            if (_currentAnimationHash != animationHash)
            {
                _currentAnimationHash = animationHash;
                MessageData messageData = new()
                {
                    LongNumber1 = _networkObject.Id,
                    IntNumber1 = _currentAnimationHash,
                };
                NetworkManager.Instance.BroadcastMessage(NetworkMessageTypes.SendEntityAnimation, messageData);
            }
        }
        #endregion
    }
}
