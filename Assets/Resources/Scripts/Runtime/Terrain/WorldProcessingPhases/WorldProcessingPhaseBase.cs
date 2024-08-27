using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public abstract class WorldProcessingPhaseBase : IWorldProcessingPhase
{
    #region Fields
    protected GameManager _gameManager;
    protected WorldDataManager _worldDataManager;
    protected ChunksManager _chunksManager;
    protected TerrainConfigurationSO _terrainConfiguration;
    protected Random _randomVar;
    protected BlockSO _air;
    protected BlockSO _dirt;
    protected BlockSO _stone;
    protected BlockSO _sand;
    protected BlockSO _water;
    protected BlockSO _dirtWall;
    protected int _terrainWidth;
    protected int _terrainHeight;
    protected int _equator;
    protected int _chunkSize;
    protected int _seed;
    protected static List<Vector2Int> _surfaceCoords;
    protected static Dictionary<(ushort, BlockTypes), BlockSO> _cache;
    #endregion

    #region Properties
    public abstract string Name { get; }
    #endregion

    #region Events / Delegates

    #endregion

    #region Public Methods
    public WorldProcessingPhaseBase()
    {
        _gameManager = GameManager.Instance;
        _worldDataManager = WorldDataManager.Instance;
        _chunksManager = ChunksManager.Instance;

        _terrainConfiguration = _gameManager.TerrainConfiguration;
        _randomVar = _gameManager.RandomVar;

        _air = _gameManager.BlocksAtlas.Air;
        _dirt = _gameManager.BlocksAtlas.Dirt;
        _stone = _gameManager.BlocksAtlas.Stone;
        _sand = _gameManager.BlocksAtlas.Sand;
        _water = _gameManager.BlocksAtlas.Water;
        _dirtWall = _gameManager.BlocksAtlas.DirtWall;

        _terrainWidth = _gameManager.CurrentTerrainWidth;
        _terrainHeight = _gameManager.CurrentTerrainHeight;
        _equator = _terrainConfiguration.Equator;
        _chunkSize = _terrainConfiguration.ChunkSize;
        _seed = _gameManager.Seed;
    }

    public abstract void StartPhase();
    #endregion

    #region Private Methods
    static WorldProcessingPhaseBase()
    {
        _surfaceCoords = new();
        _cache = new();
    }

    protected void SetBlockData(int x, int y, BlockSO data)
    {
        _worldDataManager.SetBlockData(x, y, data);
    }

    protected void SetWallData(int x, int y, BlockSO data)
    {
        _worldDataManager.SetWallData(x, y, data);
    }

    protected void SetLiquidData(int x, int y, BlockSO data)
    {
        _worldDataManager.SetLiquidData(x, y, data);
    }

    protected void SetOccupied(int x, int y)
    {
        _worldDataManager.SetOccupiedFlag(x, y, true);
    }

    protected BlockSO GetBlockData(int x, int y)
    {
        return _worldDataManager.GetBlockData(x, y);
    }

    protected BlockSO GetWallData(int x, int y)
    {
        return _worldDataManager.GetWallData(x, y);
    }

    protected BlockSO GetLiquidData(int x, int y)
    {
        return _worldDataManager.GetLiquidData(x, y);
    }

    protected int GetNextRandomValue(int start, int end)
    {
        return _randomVar.Next(start, end);
    }

    protected bool IsEmpty(int x, int y)
    {
        return _worldDataManager.IsEmpty(x, y);
    }

    protected bool IsSolid(int x, int y)
    {
        return _worldDataManager.IsSolid(x, y);
    }

    protected bool IsDust(int x, int y)
    {
        return _worldDataManager.IsDust(x, y);
    }

    protected bool IsLiquid(int x, int y)
    {
        return _worldDataManager.IsLiquid(x, y);
    }

    protected bool IsValidForTree(int x, int y)
    {
        return _worldDataManager.IsValidForTree(x, y);
    }

    protected bool IsPhysicallySolid(int x, int y)
    {
        return _worldDataManager.IsPhysicallySolidBlock(x, y);
    }

    protected bool CompareBlock(int x, int y, BlockSO data)
    {
        return _worldDataManager.CompareBlock(x, y, data);
    }
    #endregion
}
