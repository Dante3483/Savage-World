using SavageWorld.Runtime.Atlases;
using SavageWorld.Runtime.Entities.Player;
using SavageWorld.Runtime.Entities.Player.Inventory;
using SavageWorld.Runtime.Entities.Player.Research;
using SavageWorld.Runtime.Enums.Others;
using SavageWorld.Runtime.GameSession.States;
using SavageWorld.Runtime.Terrain;
using SavageWorld.Runtime.Terrain.Generation;
using SavageWorld.Runtime.Utilities;
using SavageWorld.Runtime.Utilities.Others;
using SavageWorld.Runtime.Utilities.StateMachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = System.Random;

namespace SavageWorld.Runtime.GameSession
{

    // KeyCode.H - Low quality of light system
    // KeyCode.J - Medium quality of light system
    // KeyCode.K - High quality of light system
    // KeyCode.G - Switch color light system
    // KeyCode.M - Create and save map
    // KeyCode.P - Set active player
    // KeyCode.L - Create torch
    // KeyCode.O - Enable/Disable debug UI
    public class GameManager : Singleton<GameManager>, IStateMachine<GameStateBase>
    {
        #region Private fields
        [Header("Main")]
        [SerializeField]
        private GameState _currentGameState;
        [SerializeField]
        private TerrainConfigurationSO _terrainConfiguration;
        [SerializeField]
        private PlayerGameObject _playerPrefab;
        [SerializeField]
        private Transform _playerParrent;
        [SerializeField]
        private PlayerGameObject _player;
        [SerializeField]
        private PlayerInputActions _inputActions;

        [Header("Atlases")]
        [SerializeField]
        private TilesAtlasSO _tilesAtlas;
        [SerializeField]
        private TreesAtlasSO _treesAtlas;
        [SerializeField]
        private PickUpItemsAtlasSO _pickUpItemsAtlas;
        [SerializeField]
        private ItemsAtlasSO _itemsAtlas;
        [SerializeField]
        private RecipesAtlasSO _recipesAtlas;

        [Header("Session data")]
        [SerializeField]
        private string _playerName;
        [SerializeField]
        private string _worldName;
        [SerializeField]
        private int _seed;
        [SerializeField]
        private int _currentTerrainWidth;
        [SerializeField]
        private int _currentTerrainHeight;
        [SerializeField]
        private ResearchesModelSO _researches;

        [Header("Terrain")]
        [SerializeField]
        private GameObject _terrainGameObject;

        [Header("Conditions")]
        [SerializeField]
        private bool _isStaticSeed;
        [SerializeField]
        private bool _isMenuActive;
        [SerializeField]
        private bool _isLoadingProgressActive;
        [SerializeField]
        private bool _isInputTextInFocus;

        private StateMachine<GameStateBase> _stateMachine;
        private InitializationState _initializationState;
        private MainMenuState _mainMenuState;
        private CreatingWorldState _creatingWorldState;
        private LoadingWorldState _loadingWorldState;
        private CreatingPlayerState _creatingPlayerState;
        private LoadingPlayerState _loadingPlayerState;
        private LoadingDataFromHostState _loadingDataFromHostState;
        private PlayingState _playingState;
        private QuitGameState _quitGameState;

        private List<string> _playerNames;
        private List<string> _worldNames;
        private Random _randomVar;
        private TerrainBehaviour _terrain;
        private float _loadingValue;
        private string _phasesInfo;

        //TODO:REMOVE
        public bool IsClient;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public TerrainConfigurationSO TerrainConfiguration
        {
            get
            {
                return _terrainConfiguration;
            }

            set
            {
                _terrainConfiguration = value;
            }
        }

        public TilesAtlasSO TilesAtlas
        {
            get
            {
                return _tilesAtlas;
            }

            set
            {
                _tilesAtlas = value;
            }
        }

        public TreesAtlasSO TreesAtlas
        {
            get
            {
                return _treesAtlas;
            }

            set
            {
                _treesAtlas = value;
            }
        }

        public PickUpItemsAtlasSO PickUpItemsAtlas
        {
            get
            {
                return _pickUpItemsAtlas;
            }

            set
            {
                _pickUpItemsAtlas = value;
            }
        }

        public string PlayerName
        {
            get
            {
                return _playerName;
            }

            set
            {
                _playerName = value;
            }
        }

        public string WorldName
        {
            get
            {
                return _worldName;
            }

            set
            {
                _worldName = value;
            }
        }

        public int Seed
        {
            get
            {
                return _seed;
            }

            set
            {
                _seed = value;
            }
        }

        public TerrainBehaviour Terrain
        {
            get
            {
                return _terrain;
            }

            set
            {
                _terrain = value;
            }
        }

        public int CurrentTerrainWidth
        {
            get
            {
                return _currentTerrainWidth;
            }

            set
            {
                _currentTerrainWidth = value;
            }
        }

        public int CurrentTerrainHeight
        {
            get
            {
                return _currentTerrainHeight;
            }

            set
            {
                _currentTerrainHeight = value;
            }
        }

        public bool IsStaticSeed
        {
            get
            {
                return _isStaticSeed;
            }

            set
            {
                _isStaticSeed = value;
            }
        }

        public Random RandomVar
        {
            get
            {
                if (_randomVar is null)
                {
                    _randomVar = new(_seed);
                }
                return _randomVar;
            }

            set
            {
                _randomVar = value;
            }
        }

        public float LoadingValue
        {
            get
            {
                return _loadingValue;
            }

            set
            {
                _loadingValue = value;
            }
        }

        public bool IsPlayingState
        {
            get
            {
                return CurrentState == _playingState;
            }
        }

        public string PhasesInfo
        {
            get
            {
                return _phasesInfo;
            }

            set
            {
                _phasesInfo = value;
            }
        }

        public List<string> PlayerNames
        {
            get
            {
                return _playerNames;
            }

            set
            {
                _playerNames = value;
            }
        }

        public List<string> WorldNames
        {
            get
            {
                return _worldNames;
            }

            set
            {
                _worldNames = value;
            }
        }

        public bool IsInputTextInFocus
        {
            get
            {
                return _isInputTextInFocus;
            }

            set
            {
                if (value)
                {
                    InputSystem.DisableDevice(Keyboard.current);
                }
                else
                {
                    InputSystem.EnableDevice(Keyboard.current);
                }
                _isInputTextInFocus = value;
            }
        }

        public ItemsAtlasSO ItemsAtlas
        {
            get
            {
                return _itemsAtlas;
            }

            set
            {
                _itemsAtlas = value;
            }
        }

        public PlayerGameObject Player
        {
            get
            {
                return _player;
            }

            set
            {
                _player = value;
            }
        }

        public InitializationState InitializationState
        {
            get
            {
                return _initializationState;
            }
        }

        public MainMenuState MainMenuState
        {
            get
            {
                return _mainMenuState;
            }
        }

        public StateMachine<GameStateBase> StateMachine => _stateMachine;

        public GameStateBase CurrentState => StateMachine.CurrentState;

        public GameStateBase PrevState => StateMachine.PrevState;

        public CreatingWorldState CreatingWorldState
        {
            get
            {
                return _creatingWorldState;
            }
        }

        public LoadingWorldState LoadingWorldState
        {
            get
            {
                return _loadingWorldState;
            }
        }

        public GameObject TerrainGameObject
        {
            get
            {
                return _terrainGameObject;
            }
        }

        public PlayingState PlayingState
        {
            get
            {
                return _playingState;
            }
        }

        public LoadingDataFromHostState LoadingDataFromHostState
        {
            get
            {
                return _loadingDataFromHostState;
            }

            set
            {
                _loadingDataFromHostState = value;
            }
        }

        public PlayerInputActions InputActions
        {
            get
            {
                if (_inputActions is null)
                {
                    _inputActions = new();
                }
                return _inputActions;
            }
        }

        public RecipesAtlasSO RecipesAtlas
        {
            get
            {
                return _recipesAtlas;
            }

            set
            {
                _recipesAtlas = value;
            }
        }

        public CreatingPlayerState CreatingPlayerState
        {
            get
            {
                return _creatingPlayerState;
            }

            set
            {
                _creatingPlayerState = value;
            }
        }

        public LoadingPlayerState LoadingPlayerState
        {
            get
            {
                return _loadingPlayerState;
            }

            set
            {
                _loadingPlayerState = value;
            }
        }

        public PlayerGameObject PlayerPrefab
        {
            get
            {
                return _playerPrefab;
            }

            set
            {
                _playerPrefab = value;
            }
        }
        #endregion

        #region Methods
        private void OnApplicationQuit()
        {
            ChangeState(_quitGameState);
            _currentGameState = GameState.CloseApplication;
            GetPlayerInventory().IsInitialized = false;
        }

        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new(GetType().Name);
            _initializationState = new();
            _mainMenuState = new();
            _creatingWorldState = new();
            _loadingWorldState = new();
            _creatingPlayerState = new();
            _loadingPlayerState = new();
            _loadingDataFromHostState = new();
            _playingState = new();
            _terrain = _terrainGameObject.GetComponent<TerrainBehaviour>();
            _terrainGameObject.SetActive(false);
            InputSystem.EnableDevice(Keyboard.current);
            StaticParameters.Initialize();
            GameTime.Instance.enabled = false;
        }

        private void Start()
        {
            ChangeState(_initializationState);
        }

        private void Update()
        {
            if (!IsInputTextInFocus && Input.GetKeyDown(KeyCode.P))
            {
                _player.EnableMovement();
                _terrain.gameObject.SetActive(true);
            }
        }

        public void ChangeState(GameStateBase nextState)
        {
            StateMachine.ChangeState(nextState);
        }

        public void ResetLoadingValue()
        {
            LoadingValue = 0;
        }

        public bool IsInMapRange(int x, int y)
        {
            return x >= 0 && x < _currentTerrainWidth && y >= 0 && y < _currentTerrainHeight;
        }

        public Transform GetPlayerTransform()
        {
            return _player.transform;
        }

        public InventoryModel GetPlayerInventory()
        {
            return _player.Inventory;
        }

        public ResearchesModelSO GetResearches()
        {
            return _researches;
        }

        public void SetPlayerPosition(float x, float y)
        {
            _player.transform.position = new(x, y);
        }
        #endregion
    }
}