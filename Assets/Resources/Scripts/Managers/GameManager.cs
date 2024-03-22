using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = System.Random;
using Stopwatch = System.Diagnostics.Stopwatch;

// KeyCode.H - Low quality of light system
// KeyCode.J - Medium quality of light system
// KeyCode.K - High quality of light system
// KeyCode.G - Switch color light system
// KeyCode.M - Create and save map
// KeyCode.P - Set active player
// KeyCode.L - Create torch
// KeyCode.O - Enable/Disable debug UI
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

    [Header("Session data")]
    [SerializeField] private string _playerName;
    [SerializeField] private string _worldName;
    [SerializeField] private int _seed;
    [SerializeField] private int _currentTerrainWidth;
    [SerializeField] private int _currentTerrainHeight;

    [Header("Terrain")]
    [SerializeField] private GameObject _terrainGameObject;

    [Header("Conditions")]
    [SerializeField] private bool _isStaticSeed;
    [SerializeField] private bool _isMenuActive;
    [SerializeField] private bool _isLoadingProgressActive;
    [SerializeField] private bool _isInputTextInFocus;

    private List<string> _playerNames;
    private List<string> _worldNames;
    private Random _randomVar;
    private Terrain _terrain;
    private float _loadingValue;
    private string _phasesInfo;
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
    #endregion

    #region Methods
    private void OnApplicationQuit()
    {
        UpdateGameState(GameState.CloseApplication);
    }

    private void Awake()
    {
        Instance = this;

        _terrain = _terrainGameObject.GetComponent<Terrain>();
        _terrainGameObject.SetActive(false);
        InputSystem.EnableDevice(Keyboard.current);
        StaticInfo.Initialize();
    }

    private void Start()
    {
        UpdateGameState(GameState.GameInitializationState);
    }

    private void Update()
    {
        if (!IsInputTextInFocus && Input.GetKeyDown(KeyCode.P))
        {
            _player.SetActive(true);
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
            List<Action> initializationSteps = new List<Action>()
            {
                InitializeAtlases,
                InitializePlayersData,
                InitializeWorldsData,
                WorldDataManager.Instance.Initialize,
                ChunksManager.Instance.Initialize,
                _terrain.Initialize,
            };
            
            float loadingStep = 100f / initializationSteps.Count;

            UIManager.Instance.MainMenuUI.IsActive = false;
            UIManager.Instance.MainMenuProgressBarUI.IsActive = true;

            foreach (Action initializationStep in initializationSteps)
            {
                initializationStep?.Invoke();
                _loadingValue += loadingStep;
            }

            watch.Stop();
            Debug.Log($"Game initialization: {watch.Elapsed.TotalSeconds}");
            _phasesInfo += $"Game initialization: {watch.Elapsed.TotalSeconds}\n";

            UpdateGameState(GameState.MainMenuState);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void HandleMainMenuState()
    {
        UIManager.Instance.MainMenuUI.IsActive = true;
        UIManager.Instance.MainMenuProgressBarUI.IsActive = false;
    }

    private void HandleNewGameState()
    {
        UIManager.Instance.MainMenuUI.IsActive = false;
        UIManager.Instance.MainMenuProgressBarUI.IsActive = true;
        ResetLoadingValue();

        Terrain.CreateNewWorld();
        UIManager.Instance.MainMenuProgressBarUI.IsActive = false;

        UpdateGameState(GameState.GameSession);
    }

    private void HandleLoadGameState()
    {
        UIManager.Instance.MainMenuProgressBarUI.IsActive = true;
        ResetLoadingValue();

        Terrain.LoadWorld();
        UIManager.Instance.MainMenuProgressBarUI.IsActive = false;

        UpdateGameState(GameState.GameSession);
    }

    private void InitializeAtlases()
    {
        _blocksAtlas.InitializeAtlas();
        _treesAtlas.InitializeAtlas();
        _pickUpItemsAtlas.InitializeAtlas();
    }

    private void InitializePlayersData()
    {
        GetAllPlayerNames();
    }

    private void InitializeWorldsData()
    {
        GetAllWorldNames();
    }

    private void GetAllPlayerNames()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(StaticInfo.PlayersDirectory);
        FileInfo[] filesInfo = directoryInfo.GetFiles("*.sw.player");
        _playerNames = new List<string>();
        foreach (FileInfo fileInfo in filesInfo)
        {
            _playerNames.Add(fileInfo.Name.Replace(".sw.player", ""));
        }
    }

    private void GetAllWorldNames()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(StaticInfo.WorldsDirectory);
        DirectoryInfo[] directoriesInfo = directoryInfo.GetDirectories();
        _worldNames = new List<string>();
        foreach (DirectoryInfo directoryIndo in directoriesInfo)
        {
            _worldNames.Add(directoryIndo.Name);
        }
    }

    private void ResetLoadingValue()
    {
        LoadingValue = 0;
    }

    public bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < _currentTerrainWidth && y >= 0 && y < _currentTerrainHeight;
    }

    //Delete
    public Transform GetPlayerTransform()
    {
        return _player.transform;
    }
    #endregion
}
