using SavageWorld.Runtime.GameSession;
using SavageWorld.Runtime.Menu.States;
using SavageWorld.Runtime.Network;
using SavageWorld.Runtime.Utilities;
using SavageWorld.Runtime.Utilities.Others;
using SavageWorld.Runtime.Utilities.StateMachine;
using System.IO;

namespace SavageWorld.Runtime.Menu
{

    //TODO: USE QUEUE for states history
    public class MainMenuManager : Singleton<MainMenuManager>, IStateMachine<MainMenuStateBase>
    {
        #region Private fields
        private StateMachine<MainMenuStateBase> _stateMachine;
        private StarterMenuState _starterMenuState;
        private MultiplayerModeSelectionState _multiplayerModeSelectionState;
        private PlayerSelectionState _playerSelectionState;
        private WorldSelectionState _worldSelectionState;
        private SetupNetworkConnectionState _setupNetworkConnectionState;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public StateMachine<MainMenuStateBase> StateMachine => _stateMachine;

        public MainMenuStateBase CurrentState => _stateMachine.CurrentState;

        public MainMenuStateBase PrevState => _stateMachine.PrevState;

        public StarterMenuState StarterMenuState
        {
            get
            {
                return _starterMenuState;
            }
        }

        public MultiplayerModeSelectionState MultiplayerModeSelectionState
        {
            get
            {
                return _multiplayerModeSelectionState;
            }
        }

        public PlayerSelectionState PlayerSelectionState
        {
            get
            {
                return _playerSelectionState;
            }
        }

        public WorldSelectionState WorldSelectionState
        {
            get
            {
                return _worldSelectionState;
            }
        }

        public SetupNetworkConnectionState SetupNetworkConnectionState
        {
            get
            {
                return _setupNetworkConnectionState;
            }

            set
            {
                _setupNetworkConnectionState = value;
            }
        }

        #endregion

        #region Methods
        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new(GetType().Name);
            _starterMenuState = new();
            _multiplayerModeSelectionState = new();
            _playerSelectionState = new();
            _worldSelectionState = new();
            _setupNetworkConnectionState = new();
        }

        public void ChangeState(MainMenuStateBase nextState)
        {
            StateMachine.ChangeState(nextState);
        }

        public void SelectSingleplayerMode()
        {
            NetworkManager.Instance.IsMultiplayer = false;
            StartPlayerSelection();
        }

        public void SelectMultiplayerMode()
        {
            NetworkManager.Instance.IsMultiplayer = true;
            GameManager.Instance.IsClient = false;
            ChangeState(_multiplayerModeSelectionState);
        }

        public void StartSetupConnection()
        {
            ChangeState(_setupNetworkConnectionState);
        }

        public void StartPlayerSelection()
        {
            ChangeState(_playerSelectionState);
        }

        public void StartWorldSelection()
        {
            ChangeState(_worldSelectionState);
        }

        public void BackToPrevState()
        {
            CurrentState.Back();
        }

        //Delete
        public void CreatePlayer()
        {
            string playerName = $"Player {GameManager.Instance.PlayerNames.Count + 1}";
            string playerPath = StaticParameters.PlayersDirectory + $"/{playerName}.sw.player";

            using (BinaryWriter binaryWriter = new(File.Open(playerPath, FileMode.Create)))
            {
                binaryWriter.Write(playerName);
            }

            GameManager.Instance.PlayerNames.Add(playerName);
        }

        //Delete
        public void SelectPlayer(string playerName)
        {
            GameManager.Instance.PlayerName = playerName;
            if (NetworkManager.Instance.IsMultiplayer && GameManager.Instance.IsClient)
            {
                ChangeState(null);
                GameManager.Instance.ChangeState(GameManager.Instance.CreatingPlayerState);
            }
            else
            {
                ChangeState(_worldSelectionState);
            }
        }

        //Delete
        public void DeletePlayer(string playerName)
        {
            string playerPath = StaticParameters.PlayersDirectory + $"/{playerName}.sw.player";
            File.Delete(playerPath);
            GameManager.Instance.PlayerNames.Remove(playerName);
        }

        //Delete
        public void CreateWorld()
        {
            GameManager.Instance.WorldName = $"World {GameManager.Instance.WorldNames.Count + 1}";
            ChangeState(null);
            GameManager.Instance.ChangeState(GameManager.Instance.CreatingWorldState);
        }

        //Delete
        public void SelectWorld(string worldName)
        {
            GameManager.Instance.WorldName = worldName;
            ChangeState(null);
            GameManager.Instance.ChangeState(GameManager.Instance.LoadingWorldState);
        }

        //Delete
        public void DeleteWorld(string worldName)
        {
            string worldPath = StaticParameters.WorldsDirectory + $"/{worldName}";
            Directory.Delete(worldPath, true);
            GameManager.Instance.WorldNames.Remove(worldName);
        }
        #endregion
    }
}