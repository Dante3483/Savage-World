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
        [SerializeField]
        [Min(1)]
        private int _maxClients;
        #endregion

        #region Properties
        public string IpAddress
        {
            get
            {
                return _ipAddress;
            }
        }

        public ushort Port
        {
            get
            {
                return _port;
            }
        }

        public int MaxClients
        {
            get
            {
                return _maxClients;
            }
        }
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
            _maxClients = 1;
        }

        public void Configure(string ipAddress, ushort port)
        {
            _ipAddress = ipAddress;
            _port = port;
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
