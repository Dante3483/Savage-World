using SavageWorld.Runtime.Enums.Types;
using SavageWorld.Runtime.GameSession;
using SavageWorld.Runtime.Managers;
using SavageWorld.Runtime.Terrain.Tiles;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace SavageWorld.Runtime.Terrain.Generation.Phases
{
    public abstract class WorldProcessingPhaseBase : IWorldProcessingPhase
    {
        #region Fields
        protected GameManager _gameManager;
        protected TilesManager _tilesManager;
        protected ChunksManager _chunksManager;
        protected TerrainConfigurationSO _terrainConfiguration;
        protected Random _randomVar;
        protected TileBaseSO _air;
        protected TileBaseSO _dirt;
        protected TileBaseSO _stone;
        protected TileBaseSO _sand;
        protected TileBaseSO _water;
        protected TileBaseSO _dirtWall;
        protected int _terrainWidth;
        protected int _terrainHeight;
        protected int _equator;
        protected int _chunkSize;
        protected int _seed;
        protected static List<Vector2Int> _surfaceCoords;
        protected static Dictionary<(ushort, TileTypes), TileBaseSO> _cache;
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
            _tilesManager = TilesManager.Instance;
            _chunksManager = ChunksManager.Instance;

            _terrainConfiguration = _gameManager.TerrainConfiguration;
            _randomVar = _gameManager.RandomVar;

            _air = _gameManager.TilesAtlas.Air;
            _dirt = _gameManager.TilesAtlas.Dirt;
            _stone = _gameManager.TilesAtlas.Stone;
            _sand = _gameManager.TilesAtlas.Sand;
            _water = _gameManager.TilesAtlas.Water;
            _dirtWall = _gameManager.TilesAtlas.DirtWall;

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

        protected void SetBlockData(int x, int y, TileBaseSO data)
        {
            _tilesManager.SetBlockData(x, y, data);
        }

        protected void SetWallData(int x, int y, TileBaseSO data)
        {
            _tilesManager.SetWallData(x, y, data);
        }

        protected void SetLiquidData(int x, int y, TileBaseSO data)
        {
            _tilesManager.SetLiquidData(x, y, data);
        }

        protected void SetOccupied(int x, int y)
        {
            _tilesManager.SetOccupiedFlag(x, y, true);
        }

        protected TileBaseSO GetBlockData(int x, int y)
        {
            return _tilesManager.GetBlockData(x, y);
        }

        protected TileBaseSO GetWallData(int x, int y)
        {
            return _tilesManager.GetWallData(x, y);
        }

        protected TileBaseSO GetLiquidData(int x, int y)
        {
            return _tilesManager.GetLiquidData(x, y);
        }

        protected int GetNextRandomValue(int start, int end)
        {
            return _randomVar.Next(start, end);
        }

        protected bool IsEmpty(int x, int y)
        {
            return _tilesManager.IsAbstract(x, y);
        }

        protected bool IsSolid(int x, int y)
        {
            return _tilesManager.IsSolid(x, y);
        }

        protected bool IsDust(int x, int y)
        {
            return _tilesManager.IsDust(x, y);
        }

        protected bool IsLiquid(int x, int y)
        {
            return _tilesManager.IsLiquid(x, y);
        }

        protected bool IsValidForTree(int x, int y)
        {
            return _tilesManager.IsValidForTree(x, y);
        }

        protected bool IsPhysicallySolid(int x, int y)
        {
            return _tilesManager.IsPhysicallySolidBlock(x, y);
        }

        protected bool CompareBlock(int x, int y, TileBaseSO data)
        {
            return _tilesManager.CompareBlock(x, y, data);
        }
        #endregion
    }
}