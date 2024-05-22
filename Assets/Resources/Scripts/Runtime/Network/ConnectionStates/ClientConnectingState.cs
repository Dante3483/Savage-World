using System;
using System.Threading.Tasks;
using UnityEngine;

public class ClientConnectingState : OnlineState
{
    #region Private fields
    protected ConnectionMethodBase _connectionMethod;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public ClientConnectingState(ConnectionManager connectionManager) : base(connectionManager)
    {

    }

    public override void Enter()
    {
        ConnectClientAsync();
    }

    public override void Exit()
    {

    }

    public ClientConnectingState Configure(ConnectionMethodBase baseConnectionMethod)
    {
        _connectionMethod = baseConnectionMethod;
        return this;
    }

    public override void OnClientConnected(ulong _)
    {
        _connectionManager.ChangeState(_connectionManager.ClientConnectedState);
    }

    public override void OnClientDisconnect(ulong _)
    {
        StartingClientFailed();
    }

    protected async Task ConnectClientAsync()
    {
        try
        {
            await _connectionMethod.SetupClientConnectionAsync();

            if (!_connectionManager.NetworkManager.StartClient())
            {
                throw new("NetworkManager StartClient failed");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error connecting client, see following exception");
            Debug.LogException(e);
            StartingClientFailed();
            throw;
        }
    }

    private void StartingClientFailed()
    {
        _connectionManager.ChangeState(_connectionManager.OfflineState);
    }
    #endregion
}
