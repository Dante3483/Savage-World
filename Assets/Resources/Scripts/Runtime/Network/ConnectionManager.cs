using Unity.Netcode;
using UnityEngine;

public class ConnectionManager : Singleton<ConnectionManager>
{
    #region Private fields
    [SerializeField]
    private NetworkManager _networkManager;
    [SerializeField]
    private int _numberOfReconnectAttempts = 2;

    private ConnectionStateBase _currentState;
    private OfflineState _offlineState;
    private ClientConnectingState _clientConnectingState;
    private ClientConnectedState _clientConnectedState;
    private ClientReconnectingState _clientReconnectingState;
    private StartingHostState _startingHostState;
    private HostingState _hostingState;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public NetworkManager NetworkManager
    {
        get
        {
            return _networkManager;
        }
    }

    public OfflineState OfflineState
    {
        get
        {
            return _offlineState;
        }

        set
        {
            _offlineState = value;
        }
    }

    public StartingHostState StartingHostState
    {
        get
        {
            return _startingHostState;
        }
    }

    public HostingState HostingState
    {
        get
        {
            return _hostingState;
        }
    }

    public ClientConnectingState ClientConnectingState
    {
        get
        {
            return _clientConnectingState;
        }
    }

    public ClientConnectedState ClientConnectedState
    {
        get
        {
            return _clientConnectedState;
        }
    }

    public ClientReconnectingState ClientReconnectingState
    {
        get
        {
            return _clientReconnectingState;
        }
    }

    public int NumberOfReconnectAttempts
    {
        get
        {
            return _numberOfReconnectAttempts;
        }
    }
    #endregion

    #region Methods
    protected override void Awake()
    {
        base.Awake();
        _networkManager = GetComponent<NetworkManager>();
        _offlineState = new(this);
        _clientConnectingState = new(this);
        _clientConnectedState = new(this);
        _clientReconnectingState = new(this);
        _startingHostState = new(this);
        _hostingState = new(this);
    }

    private void Start()
    {
        _currentState = OfflineState;

        NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
        NetworkManager.OnServerStarted += OnServerStarted;
        NetworkManager.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.OnTransportFailure += OnTransportFailure;
        NetworkManager.OnServerStopped += OnServerStopped;
    }

    public void ChangeState(ConnectionStateBase nextState)
    {
        Debug.Log($"{name}: Changed connection state from {_currentState.GetType().Name} to {nextState.GetType().Name}.");

        if (_currentState != null)
        {
            _currentState.Exit();
        }
        _currentState = nextState;
        _currentState.Enter();
    }

    private void OnClientDisconnectCallback(ulong clientId)
    {
        _currentState.OnClientDisconnect(clientId);
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        _currentState.OnClientConnected(clientId);
    }

    private void OnServerStarted()
    {
        _currentState.OnServerStarted();
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        _currentState.ApprovalCheck(request, response);
    }

    private void OnTransportFailure()
    {
        _currentState.OnTransportFailure();
    }

    private void OnServerStopped(bool _)
    {
        _currentState.OnServerStopped();
    }

    public void StartClientIp(string playerName, string address = "127.0.0.1", ushort port = 7777)
    {
        _currentState.StartClientIP(playerName, address, port);
    }

    public void StartHostIp(string playerName, string address = "127.0.0.1", ushort port = 7777)
    {
        _currentState.StartHostIP(playerName, address, port);
    }

    public void RequestShutdown()
    {
        _currentState.OnUserRequestedShutdown();
    }
    #endregion
}
