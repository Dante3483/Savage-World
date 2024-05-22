using System.Threading.Tasks;

public abstract class ConnectionMethodBase
{
    #region Private fields
    protected ConnectionManager _connectionManager;
    protected readonly string _playerName;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public abstract Task SetupHostConnectionAsync();

    public abstract Task SetupClientConnectionAsync();

    public abstract Task<(bool success, bool shouldTryAgain)> SetupClientReconnectionAsync();

    public ConnectionMethodBase(ConnectionManager connectionManager, string playerName)
    {
        _connectionManager = connectionManager;
        _playerName = playerName;
    }
    #endregion
}
