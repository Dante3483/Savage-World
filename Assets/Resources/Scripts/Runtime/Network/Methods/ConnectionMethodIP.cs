using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;

namespace SavageWorld.Runtime.Network.Methods
{
    public class ConnectionMethodIP : ConnectionMethodBase
    {
        #region Private fields
        private string _address;
        private ushort _port;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        public ConnectionMethodIP(string address, ushort port, NetworkManager connectionManager, string playerName) : base(connectionManager, playerName)
        {
            _address = address;
            _port = port;
        }

        public override async Task SetupClientConnectionAsync()
        {
            var utp = (UnityTransport)_connectionManager.NetworkManagerOld.NetworkConfig.NetworkTransport;
            utp.SetConnectionData(_address, _port);
        }

        public override async Task<(bool success, bool shouldTryAgain)> SetupClientReconnectionAsync()
        {
            return (true, true);
        }

        public override async Task SetupHostConnectionAsync()
        {
            var utp = (UnityTransport)_connectionManager.NetworkManagerOld.NetworkConfig.NetworkTransport;
            utp.SetConnectionData(_address, _port);
        }
        #endregion
    }
}