using UnityEngine;

namespace SavageWorld.Runtime.Network.States
{
    public class HostingState : OnlineState
    {
        #region Private fields

        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        public HostingState(NetworkManager connectionManager) : base(connectionManager)
        {

        }

        public override void Enter()
        {

        }

        public override void Exit()
        {

        }

        public override void OnUserRequestedShutdown()
        {
            var reason = JsonUtility.ToJson(ConnectStatus.HostEndedSession);
            for (var i = _connectionManager.NetworkManagerOld.ConnectedClientsIds.Count - 1; i >= 0; i--)
            {
                var id = _connectionManager.NetworkManagerOld.ConnectedClientsIds[i];
                if (id != _connectionManager.NetworkManagerOld.LocalClientId)
                {
                    _connectionManager.NetworkManagerOld.DisconnectClient(id, reason);
                }
            }
            _connectionManager.ChangeState(_connectionManager.OfflineState);
        }

        public override void OnServerStopped()
        {
            _connectionManager.ChangeState(_connectionManager.OfflineState);
        }

        public override void OnClientConnected(ulong clientId)
        {
            if (clientId != _connectionManager.NetworkManagerOld.LocalClientId)
            {
                EventManager.OnPlayerConnected(clientId);
            }
        }
        #endregion
    }
}