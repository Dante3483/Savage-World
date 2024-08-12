using SavageWorld.Runtime.Network.Connection;
using SavageWorld.Runtime.Network.States;
using UnityEngine;

namespace SavageWorld.Runtime.Network
{
    public class ConnectionManager : Singleton<ConnectionManager>, IStateMachine<ConnectionStateBase>
    {
        #region Fields
        [SerializeField]
        private NetworkManager _networkManager;

        private StateMachine<ConnectionStateBase> _stateMachine;
        private OfflineState _offlineState;
        private StartingHostState _startingServerState;
        private HostStartedState _serverStartedState;
        private ClientConnectingState _clientConnectingState;
        private ClientConnectedState _clientConnectedState;
        private ClientReconnectingState _clientReconnectingState;
        #endregion

        #region Properties
        public StateMachine<ConnectionStateBase> StateMachine => _stateMachine;

        public ConnectionStateBase CurrentState => _stateMachine.CurrentState;

        public ConnectionStateBase PrevState => _stateMachine.PrevState;

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
        }

        public StartingHostState StartingServerState
        {
            get
            {
                return _startingServerState;
            }
        }

        public HostStartedState ServerStartedState
        {
            get
            {
                return _serverStartedState;
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
        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods
        protected override void Awake()
        {
            base.Awake();
            _networkManager = NetworkManager.Instance;
            _stateMachine = new(GetType().Name);
            _offlineState = new(this);
            _clientConnectingState = new(this);
            _clientConnectedState = new(this);
            _clientReconnectingState = new(this);
            _startingServerState = new(this);
            _serverStartedState = new(this);
        }

        private void Start()
        {
            ChangeState(OfflineState);
            _networkManager.ServerStarted += ServerStartedEventHandler;
            _networkManager.ServerStopped += ServerStoppedEventHandler;
            _networkManager.ClientConnected += ClientConnectedEventHandler;
            _networkManager.ClientDisconnected += ClientDisconnectedEventHandler;
            _networkManager.Connected += ConnectedEventHandler;
            _networkManager.Disconnected += DisconnectedEventHandler;
        }
        #endregion

        #region Public Methods
        public void ChangeState(ConnectionStateBase nextState)
        {
            StateMachine.ChangeState(nextState);
        }

        public void StartClientIp()
        {
            CurrentState.StartClientIP("127.0.0.1", 7777);
        }

        public void StartServerIp()
        {
            CurrentState.StartServerIp("127.0.0.1", 7777);
        }

        public void RequestShutdown()
        {
            CurrentState.OnUserRequestedShutdown();
        }
        #endregion

        #region Private Methods
        private void ServerStartedEventHandler(INetworkConnection _)
        {
            CurrentState.OnHostStarted();
        }

        private void ServerStoppedEventHandler(INetworkConnection _)
        {
            CurrentState.OnHostStopped();
        }

        private void ClientConnectedEventHandler(int clientId)
        {
            CurrentState.OnClientConnected(clientId);
        }

        private void ClientDisconnectedEventHandler(int clientId)
        {
            CurrentState.OnClientDisconnected(clientId);
        }

        private void ConnectedEventHandler(INetworkConnection _)
        {
            CurrentState.OnConnected();
        }

        private void DisconnectedEventHandler(INetworkConnection _)
        {
            CurrentState.OnDisconnected();
        }
        #endregion
    }
}
