using System;
using Unity.Netcode;

public class StartingHostState : OnlineState
{
    #region Private fields
    private ConnectionMethodBase _connectionMethod;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public StartingHostState(ConnectionManager connectionManager) : base(connectionManager)
    {

    }

    public override void Enter()
    {
        StartHost();
    }

    public override void Exit()
    {

    }

    public override void OnServerStarted()
    {
        _connectionManager.ChangeState(_connectionManager.HostingState);
    }

    public override void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        var connectionData = request.Payload;
        var clientId = request.ClientNetworkId;
        if (clientId == _connectionManager.NetworkManager.LocalClientId)
        {
            var payload = System.Text.Encoding.UTF8.GetString(connectionData);
            //var connectionPayload = JsonUtility.FromJson<ConnectionPayload>(payload);
            //SessionManager<SessionPlayerData>.Instance.SetupConnectingPlayerSessionData(clientId, connectionPayload.playerId,
            //    new SessionPlayerData(clientId, connectionPayload.playerName, new NetworkGuid(), 0, true));

            response.Approved = true;
            response.CreatePlayerObject = true;
        }
    }

    public override void OnServerStopped()
    {
        StartHostFailed();
    }

    public StartingHostState Configure(ConnectionMethodBase baseConnectionMethod)
    {
        _connectionMethod = baseConnectionMethod;
        return this;
    }

    private async void StartHost()
    {
        try
        {
            await _connectionMethod.SetupHostConnectionAsync();

            if (!_connectionManager.NetworkManager.StartHost())
            {
                StartHostFailed();
            }
        }
        catch (Exception)
        {
            StartHostFailed();
            throw;
        }
    }

    private void StartHostFailed()
    {
        _connectionManager.ChangeState(_connectionManager.OfflineState);
    }
    #endregion
}
