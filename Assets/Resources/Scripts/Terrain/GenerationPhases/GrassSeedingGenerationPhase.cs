using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSeedingGenerationPhase : IGenerationPhase
{
    #region Private fields
    private WorldCellData[,] _worldData = GameManager.Instance.WorldData;
    private TerrainConfigurationSO _terrainConfiguration = GameManager.Instance.TerrainConfiguration;
    private Terrain _terrain = GameManager.Instance.Terrain;
    private BlockSO _dirtBlock = GameManager.Instance.BlocksAtlas.Dirt;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public string Name => "Grass seeding";
    #endregion

    #region Methods
    public void StartPhase()
    {
        TerrainGeneration.SurfaceCoords.Clear();
        BiomesID currentBiomeId;
        Vector2Int vector = new Vector2Int();
        int x;
        int y;
        int terrainWidth = GameManager.Instance.CurrentTerrainWidth;

        for (x = 0; x < terrainWidth; x++)
        {
            for (y = _terrainConfiguration.Equator; y < _terrainConfiguration.SurfaceLevel.EndY; y++)
            {
                currentBiomeId = GameManager.Instance.GetChunk(x, y).Biome.Id;
                if (!_worldData[x, y + 1].IsEmpty())
                {
                    continue;
                }
                if (_worldData[x, y + 1].IsLiquid())
                {
                    continue;
                }
                vector.x = x;
                vector.y = y;
                TerrainGeneration.SurfaceCoords.Add(vector);
                if (_worldData[x, y].CompareBlock(_dirtBlock))
                {
                    _terrain.CreateBlock(x, y, GameManager.Instance.BlocksAtlas.GetGrassByBiome(currentBiomeId));
                }
            }
        }
    }
    #endregion
}
