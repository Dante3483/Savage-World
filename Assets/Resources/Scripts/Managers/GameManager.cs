using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private GameState _gameState;
    [SerializeField] private TerrainConfigurationSO _terrainConfiguration;
    private WorldCellData[,] _worldData;
    private Chunk[,] _chunks;

    [Header("World data")]
    [SerializeField] private int _maxTerrainWidth;
    [SerializeField] private int _maxTerrainHeight;
    [SerializeField] private int _currentTerrainWidth;
    [SerializeField] private int _currentTerrainHeight;

    [Header("Session data")]
    [SerializeField] private int _seed;
    [SerializeField] private System.Random _randomVar;

    [Header("Atlasses")]
    [SerializeField] private ObjectsAtlass _objectsAtlass;

    [Header("Terrain")]
    [SerializeField] private GameObject _terrainGameObject;
    private Terrain _terrain;

    [Header("UI")]
    [SerializeField] private Canvas _mainMenu;

    [Header("Conditions")]
    [SerializeField] private bool _isStaticSeed;
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

    public Chunk[,] Chunks
    {
        get
        {
            return _chunks;
        }

        set
        {
            _chunks = value;
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

    public System.Random RandomVar
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            SaveMapToPNG();
        }
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

        //Initialize 2d world data and chunk array
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

        Chunks = new Chunk[TerrainConfiguration.CurrentHorizontalChunksCount, TerrainConfiguration.CurrentVerticalChunksCount];
        for (byte x = 0; x < TerrainConfiguration.CurrentHorizontalChunksCount; x++)
        {
            for (byte y = 0; y < TerrainConfiguration.CurrentVerticalChunksCount; y++)
            {
                Chunk emptyChunk = new Chunk(x, y);
                Chunks[x,y] = emptyChunk;
            }
        }

        watch.Stop();
        Debug.Log($"Game initialization: {watch.Elapsed.TotalSeconds}");

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

    private void SaveMapToPNG()
    {
        Texture2D worldMap = new Texture2D(CurrentTerrainWidth, CurrentTerrainHeight);
        Texture2D biomesMap = new Texture2D(CurrentTerrainWidth, CurrentTerrainHeight);
        for (int x = 0; x < CurrentTerrainWidth; x++)
        {
            for (int y = 0; y < CurrentTerrainHeight; y++)
            {
                Color blockColor = WorldData[x, y].BlockData.ColorOnMap;
                Color gridColor = new Color(blockColor.r - 0.2f, blockColor.g - 0.2f, blockColor.b - 0.2f, blockColor.a);
                Color mapColor = x % 100 == 0 || y % 100 == 0 ? gridColor : blockColor;
                worldMap.SetPixel(x, y, mapColor);

                Color biomeColor = GetChunk(x, y).Biome.ColorOnMap;
                Color gridBiomeColor = new Color(biomeColor.r - 0.2f, biomeColor.g - 0.2f, biomeColor.b - 0.2f, biomeColor.a);
                Color biomeMapColor = x % 100 == 0 || y % 100 == 0 ? gridBiomeColor : biomeColor;
                biomesMap.SetPixel(x, y, biomeMapColor);
            }
        }
        worldMap.Apply();
        biomesMap.Apply();

        byte[] bytesMap = worldMap.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/WorldMap.png", bytesMap);

        byte[] bytesBiome = biomesMap.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/BiomesMap.png", bytesBiome);
    }

    public Chunk GetChunk(int x, int y)
    {
        return Chunks[x / TerrainConfiguration.ChunkSize, y / TerrainConfiguration.ChunkSize];
    }
    #endregion
}
