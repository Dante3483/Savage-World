public class OfflineState : ConnectionStateBase
{
    #region Private fields

    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Method
    public OfflineState(ConnectionManager connectionManager) : base(connectionManager)
    {

    }

    public override void Enter()
    {
        _connectionManager.NetworkManager.Shutdown();
    }

    public override void Exit()
    {

    }

    public override void StartClientIP(string playerName, string address, ushort port)
    {
        ConnectionMethodIP connectionMethod = new(address, port, _connectionManager, playerName);
        _connectionManager.ClientReconnectingState.Configure(connectionMethod);
        _connectionManager.ChangeState(_connectionManager.ClientConnectingState.Configure(connectionMethod));
    }

    public override void StartHostIP(string playerName, string address, ushort port)
    {
        ConnectionMethodIP connectionMethod = new(address, port, _connectionManager, playerName);
        _connectionManager.ChangeState(_connectionManager.StartingHostState.Configure(connectionMethod));
    }
    #endregion
}