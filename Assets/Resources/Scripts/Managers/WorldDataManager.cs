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
        if (!GameManager.Instance.IsInputTextInFocus && Input.GetMouseButtonDown(1))
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
        BlockSO airWall = GameManager.Instance.BlocksAtlas.AirWall;

        Parallel.For(0, terrainWidth, (index) =>
        {
            ushort x = (ushort)index;
            for (ushort y = 0; y < terrainHeight; y++)
            {
                _worldData[x, y] = new WorldCellData(x, y, airBlock, airWall);
            }
        });
    }

    public ref WorldCellData GetWorldCellData(int x, int y)
    {
        return ref _worldData[x, y];
    }

    public ref WorldCellData GetAdjacentWorldCellData(int x, int y, Vector2Int direction)
    {
        return ref _worldData[x + direction.x, y + direction.y];
    }

    public void SetBlockInfoByCoords(Vector2Int blockCoords)
    {
        _blockInfo = _worldData[blockCoords.x, blockCoords.y].ToString();
    }

    public void MakeOccupied(int x, int y)
    {
        _worldData[x, y].MakeOccupied();
    }

    public void MakeFree(int x, int y)
    {
        _worldData[x, y].MakeFree();
    }

    public void MakeUnbreakable(int x, int y)
    {
        _worldData[x, y].MakeUnbreakable();
    }

    public void MakeBreakable(int x, int y)
    {
        _worldData[x, y].MakeBreakable();
    }

    public void MakeTree(int x, int y)
    {
        _worldData[x, y].MakeTree();
    }

    public void RemoveTree(int x, int y)
    {
        _worldData[x, y].RemoveTree();
    }

    public void MakeTreeTrunk(int x, int y)
    {
        _worldData[x, y].MakeTreeTrunk();
    }

    public void RemoveTreeTrunk(int x, int y)
    {
        _worldData[x, y].RemoveTreeTrunk();
    }

    public bool IsEmpty(int x, int y)
    {
        return _worldData[x, y].IsEmpty();
    }

    public bool IsSolid(int x, int y)
    {
        return _worldData[x, y].IsSolid();
    }

    public bool IsWall(int x, int y)
    {
        return _worldData[x, y].IsWall();
    }

    public bool IsSolidAnyNeighbor(int x, int y)
    {
        return _worldData[x - 1, y].IsSolid() || _worldData[x + 1, y].IsSolid() ||
            _worldData[x, y + 1].IsSolid() || _worldData[x, y - 1].IsSolid();
    }

    public bool IsFree(int x, int y)
    {
        return _worldData[x, y].IsFree();
    }

    public bool IsBreakable(int x, int y)
    {
        return _worldData[x, y].IsBreakable();
    }
    #endregion
}
