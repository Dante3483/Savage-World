using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;
using Stopwatch = System.Diagnostics.Stopwatch;

// KeyCode.H - Low quality of light system
// KeyCode.J - Medium quality of light system
// KeyCode.K - High quality of light system
// KeyCode.G - Switch color light system
// KeyCode.M - Create and save map
// KeyCode.F - Display chunks
// KeyCode.P - Set active player
// KeyCode.L - Create torch
// KeyCode.V - Player hurt
// KeyCode.Space - Player jump
// KeyCode.LeftShift - Player run
// KeyCode.LeftShift + C - Player slide
public class GameManager : MonoBehaviour
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private GameState _currentGameState;
    [SerializeField] private TerrainConfigurationSO _terrainConfiguration;
    [SerializeField] private GameObject _player;

    [Header("Atlases")]
    [SerializeField] private BlocksAtlas _blocksAtlas;
    [SerializeField] private TreesAtlas _treesAtlas;
    [SerializeField] private PickUpItemsAtlas _pickUpItemsAtlas;

    [Header("World data")]
    [SerializeField] private int _maxTerrainWidth;
    [SerializeField] private int _maxTerrainHeight;

    [Header("Session data")]
    [SerializeField] private string _playerName;
    [SerializeField] private string _worldName;
    [SerializeField] private int _seed;
    [SerializeField] private int _currentTerrainWidth;
    [SerializeField] private int _currentTerrainHeight;
    private Random _randomVar;
    private WorldCellData[,] _worldData;
    private Chunk[,] _chunks;

    [Header("Debug")]
    private string _generalInfo;
    private string _otherInfo;
    private Vector2Int _blockInfoCoords;
    private float _loadingValue;

    [Header("Atlasses")]
    [SerializeField] private ObjectsAtlass _objectsAtlass;

    [Header("Terrain")]
    [SerializeField] private GameObject _terrainGameObject;
    private Terrain _terrain;

    [Header("UI")]
    [SerializeField] private Canvas _mainMenu;
    [SerializeField] private Canvas _loadingProgress;
    [SerializeField] private Slider _loadingSlider;
    [SerializeField] private TextMeshProUGUI _infoText;
    [SerializeField] private TextMeshProUGUI _blockInfoText;
    [SerializeField] private TextMeshProUGUI _otherInformationText;

    [Header("Conditions")]
    [SerializeField] private bool _isStaticSeed;
    [SerializeField] private bool _isMenuActive;
    [SerializeField] private bool _isLoadingProgressActive;
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

    public string GeneralInfo
    {
        get
        {
            return _generalInfo;
        }

        set
        {
            _generalInfo = value;
        }
    }

    public bool IsMenuActive
    {
        get
        {
            return _isMenuActive;
        }

        set
        {
            _isMenuActive = value;
        }
    }

    public bool IsLoadingProgressActive
    {
        get
        {
            return _isLoadingProgressActive;
        }

        set
        {
            _isLoadingProgressActive = value;
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

    public string OtherInfo
    {
        get
        {
            return _otherInfo;
        }

        set
        {
            _otherInfo = value;
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

    public bool IsGameSession
    {
        get
        {
            return _currentGameState == GameState.GameSession;
        }
    }

    public bool IsWorldLoading
    {
        get
        {
            return _currentGameState == GameState.LoadGameState;
        }
    }

    public BlocksAtlas BlocksAtlas
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

    public TreesAtlas TreesAtlas
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

    public PickUpItemsAtlas PickUpItemsAtlas
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
    #endregion

    #region Methods
    private void OnApplicationQuit()
    {
        UpdateGameState(GameState.CloseApplication);
        //IsGameSession = false;
    }

    private void Awake()
    {
        Instance = this;
        _terrain = _terrainGameObject.GetComponent<Terrain>();
        _terrainGameObject.SetActive(false);
    }

    private void Start()
    {
        StartCoroutine(PrintDebugInfo());
        StartCoroutine(UpdateObjects());

        UpdateGameState(GameState.GameInitializationState);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Task.Run(() => DisplayChunks());
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            _player.SetActive(true);
        }

        if (IsGameSession)
        {
            BreakBlock();
            PrintBlockDetail();
            CreateWater();
            CreateTorch();
        }
    }

    public void UpdateGameState(GameState gameState)
    {
        _currentGameState = gameState;
        switch (gameState)
        {
            case GameState.GameInitializationState:
                {
                    Task.Run(() => HandleGameInitializationState());
                }
                break;
            case GameState.MainMenuState:
                {
                    HandleMainMenuState();
                }
                break;
            case GameState.NewGameState:
                {
                    //Set random seed
                    //IsGameSession = false;
                    if (!IsStaticSeed)
                    {
                        Seed = UnityEngine.Random.Range(-1000000, 1000000);
                    }
                    _terrainGameObject.SetActive(true);
                    Task.Run(() => HandleNewGameState());
                    _terrain.StartCoroutinesAndThreads();
                }
                break;
            case GameState.LoadGameState:
                {
                    //Set random seed
                    //IsGameSession = false;
                    _terrainGameObject.SetActive(true);
                    Task.Run(() => HandleLoadGameState());
                    _terrain.StartCoroutinesAndThreads();
                }
                break;
            default:
                break;
        }
    }

    private void HandleGameInitializationState()
    {
        try
        {
            var watch = Stopwatch.StartNew();
            IsMenuActive = false;
            IsLoadingProgressActive = true;

            DefineTerrainWidthAndHeight();
            InitializeWorldData();
            InitializeChunks();
            InitializeAtlases();
            watch.Stop();
            Debug.Log($"Game initialization: {watch.Elapsed.TotalSeconds}");
            GeneralInfo += $"Game initialization: {watch.Elapsed.TotalSeconds}\n";

            UpdateGameState(GameState.MainMenuState);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            throw e;
        }
    }

    private void InitializeAtlases()
    {

        _blocksAtlas.InitializeAtlas();
        _treesAtlas.InitializeAtlas();
        _pickUpItemsAtlas.InitializeAtlas();
        //_objectsAtlass.Initialize();
    }

    private void InitializeChunks()
    {
        Chunks = new Chunk[TerrainConfiguration.CurrentHorizontalChunksCount, TerrainConfiguration.CurrentVerticalChunksCount];
        float step = 50f / TerrainConfiguration.CurrentHorizontalChunksCount;
        for (byte x = 0; x < TerrainConfiguration.CurrentHorizontalChunksCount; x++)
        {
            for (byte y = 0; y < TerrainConfiguration.CurrentVerticalChunksCount; y++)
            {
                Chunks[x, y] = new Chunk(x, y);
            }
            LoadingValue += step;
        }
    }

    private void InitializeWorldData()
    {
        float step = 50f / _maxTerrainWidth;
        WorldData = new WorldCellData[_maxTerrainWidth, _maxTerrainHeight];

        Parallel.For(0, _maxTerrainWidth, (index) =>
        {
            ushort x = (ushort)index;
            for (ushort y = 0; y < _maxTerrainHeight; y++)
            {
                WorldData[x, y] = new WorldCellData(x, y);
            }
            LoadingValue += step;
        });
    }

    private void DefineTerrainWidthAndHeight()
    {
        _maxTerrainWidth = TerrainConfiguration.DefaultHorizontalChunksCount * TerrainConfiguration.ChunkSize;
        _maxTerrainHeight = TerrainConfiguration.DefaultVerticalChunksCount * TerrainConfiguration.ChunkSize;

        _currentTerrainWidth = TerrainConfiguration.CurrentHorizontalChunksCount * TerrainConfiguration.ChunkSize;
        _currentTerrainHeight = TerrainConfiguration.CurrentVerticalChunksCount * TerrainConfiguration.ChunkSize;
    }

    private void HandleMainMenuState()
    {
        TurnOnMenu();
    }

    private void TurnOnMenu()
    {
        IsMenuActive = true;
        IsLoadingProgressActive = false;
    }

    private void TurnOffMenu()
    {
        IsMenuActive = false;
    }

    private void HandleNewGameState()
    {
        Debug.Log("New game state");

        IsMenuActive = false;
        IsLoadingProgressActive = true;
        LoadingValue = 0;

        //Create new world
        Terrain.CreateNewWorld(ref _worldData);
        IsLoadingProgressActive = false;
    }

    private void HandleLoadGameState()
    {
        Debug.Log("Load game state");

        //Load world
        Terrain.LoadWorld(ref _worldData);
        IsMenuActive = false;
        //IsGameSession = true;
    }

    public Vector3 GetPlayerPosition()
    {
        return _player.transform.position;
    }

    public Transform GetPlayerTransform()
    {
        return _player.transform;
    }

    public ref WorldCellData GetWorldCellDataRef(int x, int y)
    {
        return ref WorldData[x, y];
    }

    private void DisplayChunks()
    {
        foreach (Chunk chunk in Chunks)
        {
            Debug.Log(chunk.ToString());
        }
    }

    public Chunk GetChunk(int x, int y)
    {
        return Chunks[x / TerrainConfiguration.ChunkSize, y / TerrainConfiguration.ChunkSize];
    }

    public void SetChunkBiome(int x, int y, BiomeSO biome)
    {
        Chunk chunk = GetChunk(x, y);
        if (chunk.Biome.Id == BiomesID.NonBiome)
        {
            Chunks[x / TerrainConfiguration.ChunkSize, y / TerrainConfiguration.ChunkSize].Biome = biome;
        }
    }

    public void SetChunkBiome(Chunk chunk, BiomeSO biome)
    {
        if (chunk.Biome.Id == BiomesID.NonBiome)
        {
            Chunks[chunk.Coords.x, chunk.Coords.y].Biome = biome;
        }
    }

    public IEnumerator PrintDebugInfo()
    {
        while (true)
        {
            _infoText.text = GeneralInfo;
            _otherInformationText.text = OtherInfo;
            yield return null;
        }
    }

    public IEnumerator UpdateObjects()
    {
        while (true)
        {
            _mainMenu.gameObject.SetActive(IsMenuActive);
            _loadingProgress.gameObject.SetActive(IsLoadingProgressActive);
            _loadingSlider.value = LoadingValue;
            yield return null;
        }
    }

    public void BreakBlock()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 clickPosition = Input.mousePosition;

            Vector3Int intPos = Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(clickPosition));

            Terrain.CreateBlock((ushort)intPos.x, (ushort)intPos.y, ObjectsAtlass.Air);

            Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x, intPos.y));
            Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x, intPos.y + 1));
            Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x, intPos.y - 1));
            Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x - 1, intPos.y));
            Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x + 1, intPos.y));
        }
    }

    public void PrintBlockDetail()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 clickPosition = Input.mousePosition;

            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(clickPosition);

            _blockInfoCoords = Vector2Int.FloorToInt(worldPosition);

            _blockInfoText.text = _worldData[_blockInfoCoords.x, _blockInfoCoords.y].ToString();

        }
    }

    public void CreateWater()
    {
        if (Input.GetMouseButtonDown(2))
        {
            Vector3 clickPosition = Input.mousePosition;

            Vector3Int intPos = Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(clickPosition));

            Terrain.CreateLiquidBlock((ushort)intPos.x, (ushort)intPos.y, (byte)ObjectsAtlass.Water.GetId());

            Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x, intPos.y));
        }
    }

    public void CreateTorch()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Vector3 clickPosition = Input.mousePosition;

            Vector3Int intPos = Vector3Int.FloorToInt(Camera.main.ScreenToWorldPoint(clickPosition));

            Terrain.CreateBlock((ushort)intPos.x, (ushort)intPos.y, ObjectsAtlass.Torch);

            Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x, intPos.y));
            Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x, intPos.y + 1));
            Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x, intPos.y - 1));
            Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x - 1, intPos.y));
            Terrain.NeedToUpdate.Add(new Vector2Ushort(intPos.x + 1, intPos.y));
        }
    }

    #endregion
}
