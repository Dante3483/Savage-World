using Unity.Netcode;
using UnityEngine;

public class ConnectionManager : Singleton<ConnectionManager>, IStateMachine<ConnectionStateBase>
{
    #region Private fields
    [SerializeField]
    private NetworkManager _networkManager;
    [SerializeField]
    private int _numberOfReconnectAttempts = 2;

    private StateMachine<ConnectionStateBase> _stateMachine;
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
    public StateMachine<ConnectionStateBase> StateMachine => _stateMachine;

    public ConnectionStateBase CurrentState => _stateMachine.CurrentState;

    public ConnectionStateBase PrevState => _stateMachine.PrevState;


    #endregion

    #region Methods
    protected override void Awake()
    {
        base.Awake();
        _networkManager = GetComponent<NetworkManager>();
        _stateMachine = new(GetType().Name);
        _offlineState = new(this);
        _clientConnectingState = new(this);
        _clientConnectedState = new(this);
        _clientReconnectingState = new(this);
        _startingHostState = new(this);
        _hostingState = new(this);
    }

    private void Start()
    {
        ChangeState(OfflineState);

        NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
        NetworkManager.OnServerStarted += OnServerStarted;
        NetworkManager.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.OnTransportFailure += OnTransportFailure;
        NetworkManager.OnServerStopped += OnServerStopped;
    }

    public void ChangeState(ConnectionStateBase nextState)
    {
        StateMachine.ChangeState(nextState);
    }

    private void OnClientDisconnectCallback(ulong clientId)
    {
        CurrentState.OnClientDisconnect(clientId);
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        CurrentState.OnClientConnected(clientId);
    }

    private void OnServerStarted()
    {
        CurrentState.OnServerStarted();
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        CurrentState.ApprovalCheck(request, response);
    }

    private void OnTransportFailure()
    {
        CurrentState.OnTransportFailure();
    }

    private void OnServerStopped(bool _)
    {
        CurrentState.OnServerStopped();
    }

    public void StartClientIp(string playerName, string address = "127.0.0.1", ushort port = 7777)
    {
        CurrentState.StartClientIP(playerName, address, port);
    }

    public void StartHostIp(string playerName, string address = "127.0.0.1", ushort port = 7777)
    {
        CurrentState.StartHostIP(playerName, address, port);
    }

    public void RequestShutdown()
    {
        CurrentState.OnUserRequestedShutdown();
    }
    #endregion
}
