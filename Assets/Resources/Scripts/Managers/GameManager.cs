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

    [Header("Atlasses")]
    [SerializeField] private ObjectsAtlass _objectsAtlass;
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
    #endregion

    #region Methods
    private void Awake()
    {
        Instance = this;
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

        //Initialize 2d world data array
        var watch = System.Diagnostics.Stopwatch.StartNew();

        _worldData = new WorldCellData[_maxTerrainWidth, _maxTerrainHeight];
        for (ushort x = 0; x < _maxTerrainWidth; x++)
        {
            for (ushort y = 0; y < _maxTerrainHeight; y++)
            {
                WorldCellData emptyWorldCellData = new WorldCellData(x,y);
                _worldData[x, y] = emptyWorldCellData;
            }
        }

        watch.Stop();
        Debug.Log($"Terrain generation complete: {watch.Elapsed.TotalSeconds}");

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
    }
    #endregion
}
