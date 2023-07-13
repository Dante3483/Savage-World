using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private GameState _gameState;
    [SerializeField] private TerrainConfigurationSO _terrainConfiguration;
    private WorldCellData[,] _worldData;

    [Header("World data")]
    [SerializeField] private int _maxTerrainWidth;
    [SerializeField] private int _maxTerrainHeight;
    [SerializeField] private int _currentTerrainWidth;
    [SerializeField] private int _currentTerrainHeight;

    [Header("Session data")]
    [SerializeField] private int _seed;

    [Header("Atlasses")]
    [SerializeField] private ObjectsAtlass _objectsAtlass;

    [Header("Terrain")]
    [SerializeField] private GameObject _terrainGameObject;
    private Terrain _terrain;

    [Header("UI")]
    [SerializeField] private Canvas _mainMenu;
    #endregion

    #region Public fields
    public static GameManager Instance;
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

    public ObjectsAtlass ObjectsAtlass
    {
        get
        {
            return _objectsAtlass;
        }

        set
        {
            _objectsAtlass = value;
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
            if (_terrain == null)
            {
                _terrain = _terrainGameObject.GetComponent<Terrain>();
            }
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

    public WorldCellData[,] WorldData
    {
        get
        {
            return _worldData;
        }

        set
        {
            _worldData = value;
        }
    }
    #endregion

    #region Methods
    private void Awake()
    {
        Instance = this;
        _terrainGameObject.SetActive(false);
    }

    private void Start()
    {
        UpdateGameState(global::GameState.GameInitializationState);
    }

    public void UpdateGameState(GameState newState)
    {
        _gameState = newState;

        switch (_gameState)
        {
            case GameState.GameInitializationState:
                {
                    HandleGameInitialization();
                }
                break;
            case GameState.MainMenuState:
                {
                    HandleMainMenu();
                }
                break;
            case GameState.NewGameState:
                {
                    HandleNewGameState();
                }
                break;
            default:
                break;
        }
    }

    private void HandleGameInitialization()
    {
        Debug.Log("Initialization state");

        //Define Terrain width and height
        _maxTerrainWidth = TerrainConfiguration.DefaultHorizontalChunksCount * TerrainConfiguration.ChunkSize;
        _maxTerrainHeight = TerrainConfiguration.DefaultVerticalChunksCount * TerrainConfiguration.ChunkSize;

        CurrentTerrainWidth = TerrainConfiguration.CurrentHorizontalChunksCount * TerrainConfiguration.ChunkSize;
        CurrentTerrainHeight= TerrainConfiguration.CurrentVerticalChunksCount * TerrainConfiguration.ChunkSize;

        //Initialize 2d world data array
        var watch = System.Diagnostics.Stopwatch.StartNew();

        WorldData = new WorldCellData[_maxTerrainWidth, _maxTerrainHeight];
        for (ushort x = 0; x < _maxTerrainWidth; x++)
        {
            for (ushort y = 0; y < _maxTerrainHeight; y++)
            {
                WorldCellData emptyWorldCellData = new WorldCellData(x,y);
                WorldData[x, y] = emptyWorldCellData;
            }
        }

        watch.Stop();
        Debug.Log($"Game initialication: {watch.Elapsed.TotalSeconds}");

        //Initialize atlasses
        ObjectsAtlass.Initialize();

        UpdateGameState(GameState.MainMenuState);
    }

    private void HandleMainMenu()
    {
        Debug.Log("Menu state");
    }

    private void HandleNewGameState()
    {
        Debug.Log("New game state");

        _mainMenu.gameObject.SetActive(false);
        _terrainGameObject.SetActive(true);
        //Create new world
        Terrain.CreateNewWorld();

    }
    #endregion
}
