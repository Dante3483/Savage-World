using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = System.Random;

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
    private Player _playerPrefab;
    [SerializeField]
    private Transform _playerParrent;
    [SerializeField]
    private Player _player;
    [SerializeField]
    private PlayerInputActions _playerInputActions;

    [Header("Atlases")]
    [SerializeField]
    private BlocksAtlasSO _blocksAtlas;
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
    private LoadingDataFromHostState _loadingDataFromHostState;
    private PlayingState _playingState;
    private QuitGameState _quitGameState;

    private List<string> _playerNames;
    private List<string> _worldNames;
    private Random _randomVar;
    private Terrain _terrain;
    private float _loadingValue;
    private string _phasesInfo;
    private bool _isMultiplayer;
    private bool _isClient;
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

    public BlocksAtlasSO BlocksAtlas
    {
        get
        {
            return _blocksAtlas;
        }

        set
        {
            _blocksAtlas = value;
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

    public Terrain Terrain
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

    public bool IsMultiplayer
    {
        get
        {
            return _isMultiplayer;
        }

        set
        {
            _isMultiplayer = value;
        }
    }

    public Player Player
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

    public bool IsClient
    {
        get
        {
            return _isClient;
        }

        set
        {
            _isClient = value;
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

    public PlayerInputActions PlayerInputActions
    {
        get
        {
            if (_playerInputActions is null)
            {
                _playerInputActions = new();
            }
            return _playerInputActions;
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
        _loadingDataFromHostState = new();
        _playingState = new();
        _terrain = _terrainGameObject.GetComponent<Terrain>();
        _terrainGameObject.SetActive(false);
        InputSystem.EnableDevice(Keyboard.current);
        StaticInfo.Initialize();
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

    public void InitializePlayer(int x, int y, Player player)
    {
        _player = player;
        Camera.main.GetComponent<FollowObject>().Target = _player.transform;
        SetPlayerPosition(x, y);
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
