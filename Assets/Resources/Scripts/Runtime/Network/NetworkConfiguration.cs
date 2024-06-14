using UnityEngine;

namespace SavageWorld.Runtime.Network
{
    public class NetworkConfiguration : MonoBehaviour
    {
        #region Fields
        [SerializeField]
        private string _ipAddress;
        [SerializeField]
        private ushort _port;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
        public NetworkConfiguration()
        {
            _ipAddress = "127.0.0.1";
            _port = 7777;
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
