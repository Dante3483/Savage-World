using SavageWorld.Runtime.Network.States;
using System;
using UnityEngine;

namespace SavageWorld.Runtime.Network
{
    [RequireComponent(typeof(NetworkConfiguration))]
    public class NetworkManager : Singleton<NetworkManager>, IStateMachine<ConnectionStateBase>
    {
        #region Fields
        [SerializeField]
        private NetworkConfiguration _networkConfiguration;
        [SerializeField]
        private Unity.Netcode.NetworkManager _networkManager;
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

        #region Properties
        public Unity.Netcode.NetworkManager NetworkManagerOld
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

        #region Events / Delegates
        public event Action
        #endregion

        #region Monobehaviour Methods
        protected override void Awake()
        {
            base.Awake();
            _networkConfiguration = GetComponent<NetworkConfiguration>();
            _networkManager = GetComponent<Unity.Netcode.NetworkManager>();
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

            NetworkManagerOld.OnClientConnectedCallback += OnClientConnectedCallback;
            NetworkManagerOld.OnClientDisconnectCallback += OnClientDisconnectCallback;
            NetworkManagerOld.OnServerStarted += OnServerStarted;
            NetworkManagerOld.ConnectionApprovalCallback += ApprovalCheck;
            NetworkManagerOld.OnTransportFailure += OnTransportFailure;
            NetworkManagerOld.OnServerStopped += OnServerStopped;
        }
        #endregion

        #region Public Methods
        public void ChangeState(ConnectionStateBase nextState)
        {
            StateMachine.ChangeState(nextState);
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

        #region Private Methods
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

        private void ApprovalCheck(Unity.Netcode.NetworkManager.ConnectionApprovalRequest request, Unity.Netcode.NetworkManager.ConnectionApprovalResponse response)
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
        #endregion
    }
}
