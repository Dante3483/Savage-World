using UnityEngine;

public class HostingState : OnlineState
{
    #region Private fields

    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public HostingState(ConnectionManager connectionManager) : base(connectionManager)
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
        for (var i = _connectionManager.NetworkManager.ConnectedClientsIds.Count - 1; i >= 0; i--)
        {
            var id = _connectionManager.NetworkManager.ConnectedClientsIds[i];
            if (id != _connectionManager.NetworkManager.LocalClientId)
            {
                _connectionManager.NetworkManager.DisconnectClient(id, reason);
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
        if (clientId != _connectionManager.NetworkManager.LocalClientId)
        {
            EventManager.OnPlayerConnected(clientId);
        }
    }
    #endregion
}
