using System.Threading.Tasks;
using UnityEngine;

public class WorldDataManager : MonoBehaviour
{
    #region Private fields
    private WorldCellData[,] _worldData;
    private string _blockInfo;
    #endregion

    #region Public fields
    public static WorldDataManager Instance;
    #endregion

    #region Properties
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

    public string BlockInfo
    {
        get
        {
            return _blockInfo;
        }

        set
        {
            _blockInfo = value;
        }
    }
    #endregion

    #region Methods
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 clickPosition = Input.mousePosition;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(clickPosition);
            SetBlockInfoByCoords(Vector2Int.FloorToInt(worldPosition));
        }
    }

    public void Initialize()
    {
        TerrainConfigurationSO terrainConfiguration = GameManager.Instance.TerrainConfiguration;
        int terrainWidth = terrainConfiguration.TerrainWidth;
        int terrainHeight = terrainConfiguration.TerrainHeight;

        _worldData = new WorldCellData[terrainWidth, terrainHeight];
        BlockSO airBlock = GameManager.Instance.BlocksAtlas.Air;
        BlockSO airBGBlock = GameManager.Instance.BlocksAtlas.AirBG;

        Parallel.For(0, terrainWidth, (index) =>
        {
            ushort x = (ushort)index;
            for (ushort y = 0; y < terrainHeight; y++)
            {
                _worldData[x, y] = new WorldCellData(x, y, airBlock, airBGBlock);
            }
        });
    }

    public ref WorldCellData GetWorldCellData(int x, int y)
    {
        return ref _worldData[x, y];
    }

    public void SetBlockInfoByCoords(Vector2Int blockCoords)
    {
        _blockInfo = _worldData[blockCoords.x, blockCoords.y].ToString();
    }
    #endregion
}
