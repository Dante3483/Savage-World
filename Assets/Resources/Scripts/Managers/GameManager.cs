using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private GameState _gameState;

    [Header("Initialization")]
    [SerializeField] private int _maxXWorldSize;
    [SerializeField] private int _maxYWorldSize;
    [SerializeField] private int _defaultChunkSize;

    [Header("Level")]
    [SerializeField] private GameObject _terrainPrefab;
    [SerializeField] private GameObject _playerPrefab;

    [Header("UI")]
    [SerializeField] private Canvas _mainMenuCanvas;
    [SerializeField] private Canvas _inGameCanvas;
    [SerializeField] private UIInventoryPage _inventoryUI;
    [SerializeField] private UIHotbar _hotbarUI;
    #endregion

    #region Public fields
    public static GameManager Instance;
    public SaveLoadSystem SaveLoadManager;
    public Interactions PlayerInteractions;
    public World World;
    public GameObject Player;
    public ObjectData[,] ObjectsData;
    public Chunk[,] Chunks;
    public GameObject TreesSection;
    public GameObject DropSection;
    public GameObject PickableItemsSection;
    public Camera MainCamera;
    public MouseFollowerOutline mouseFollowerOutline;
    public WorldObjectsAtlas WorldObjectAtlas;
    public ItemsAtlas ItemsAtlas;
    public bool IsPlayerCreated;
    #endregion

    #region Properties
    public GameState GameState
    {
        get
        {
            return _gameState;
        }

        set
        {
            _gameState = value;
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
        UpdateGameState(GameState.FirstInitialization);
    }

    public void UpdateGameState(GameState newState)
    {
        GameState = newState;

        switch (GameState)
        {
            case GameState.FirstInitialization:
                {
                    HandleFirstInitializationState();
                }
                break;
            case GameState.MainMenu:
                {
                    HandleMainMenuState();
                }
                break;
            case GameState.NewLevel:
                {
                    HandleNewLevelState();
                }
                break;
            case GameState.LoadLevel:
                {
                    HandleLoadLevelState();
                }
                break;
            case GameState.InLevel:
                {
                    StartCoroutine(HandleInLevelState());
                }
                break;
            case GameState.Quit:
                {
                    HandleQuitState();
                }
                break;
            default:
                break;
        }
    }

    private void HandleQuitState()
    {

    }

    private IEnumerator HandleInLevelState()
    {
        IsPlayerCreated = true;
        while (!World.IsLevelFullyCreated)
        {
            yield return null;
        }
        Player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
    }

    private void HandleLoadLevelState()
    {
        GameObject terrain = Instantiate(_terrainPrefab);
        DropSection = terrain.transform.Find("Drop").gameObject;
        TreesSection = terrain.transform.Find("Trees").gameObject;
        PickableItemsSection = terrain.transform.Find("PickableItems").gameObject;
        World = terrain.GetComponent<World>();

        Player = Instantiate(_playerPrefab);
        Player.GetComponent<InventoryController>().InventoryUI = _inventoryUI;
        Player.GetComponent<HotbarController>().HotbarUI = _hotbarUI;
        PlayerInteractions = Player.GetComponent<Interactions>();
        MainCamera.GetComponent<CameraFollow>().Target = Player;

        World.LoadWorld();

        _mainMenuCanvas.gameObject.SetActive(false);
        _inGameCanvas.gameObject.SetActive(true);

        UpdateGameState(GameState.InLevel);
    }

    private void HandleNewLevelState()
    {
        GameObject terrain = Instantiate(_terrainPrefab);
        DropSection = terrain.transform.Find("Drop").gameObject;
        TreesSection = terrain.transform.Find("Trees").gameObject;
        PickableItemsSection = terrain.transform.Find("PickableItems").gameObject;
        World = terrain.GetComponent<World>();
        World.CreateNewWorld();

        int playerSpawnX = World.TerrainConfiguration.SpawnXPosition;
        int playerSpawnY;
        for (playerSpawnY = World.TerrainConfiguration.Equator; playerSpawnY < World.TerrainConfiguration.WorldHeight; playerSpawnY++)
        {
            if (ObjectsData[playerSpawnX, playerSpawnY].Type == ObjectType.Empty)
            {
                break;
            }
        }
        playerSpawnY += 3;
        Player = Instantiate(_playerPrefab, new Vector3(playerSpawnX, playerSpawnY), Quaternion.identity);
        Player.GetComponent<InventoryController>().InventoryUI = _inventoryUI;
        Player.GetComponent<InventoryController>().PrepareInventoryData();
        Player.GetComponent<HotbarController>().HotbarUI = _hotbarUI;
        PlayerInteractions = Player.GetComponent<Interactions>();
        MainCamera.GetComponent<CameraFollow>().Target = Player;

        _mainMenuCanvas.gameObject.SetActive(false);
        _inGameCanvas.gameObject.SetActive(true);

        UpdateGameState(GameState.InLevel);
    }

    private void HandleMainMenuState()
    {
        _mainMenuCanvas.gameObject.SetActive(true);
        _inGameCanvas.gameObject.SetActive(false);
    }

    private void HandleFirstInitializationState()
    {
        ObjectsData = new ObjectData[_maxXWorldSize, _maxYWorldSize];
        for (int x = 0; x < _maxXWorldSize; x++)
        {
            for (int y = 0; y < _maxYWorldSize; y++)
            {
                ObjectData objectData = new ObjectData(new Vector3Int(x, y));
                ObjectsData[x, y] = objectData;
            }
        }

        Chunks = new Chunk[_maxXWorldSize / _defaultChunkSize, _maxYWorldSize / _defaultChunkSize];
        for (int x = 0; x < _maxXWorldSize / _defaultChunkSize; x++)
        {
            for (int y = 0; y < _maxYWorldSize / _defaultChunkSize; y++)
            {
                Chunk newChunk = new Chunk();
                newChunk.x = x * _defaultChunkSize;
                newChunk.y = y * _defaultChunkSize;
                Chunks[x, y] = newChunk;
            }
        }

        WorldObjectAtlas.LoadResources();
        ItemsAtlas.LoadResources();

        UpdateGameState(GameState.MainMenu);
    }
    #endregion
}
