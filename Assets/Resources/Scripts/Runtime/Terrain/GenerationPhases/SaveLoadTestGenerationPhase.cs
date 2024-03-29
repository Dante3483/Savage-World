using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SaveLoadTestGenerationPhase : IGenerationPhase
{
    #region Private fields

    #endregion

    #region Public fields

    #endregion

    #region Properties
    public string Name => "Save/Load generation";
    #endregion

    #region Methods
    public void StartPhase()
    {
        TerrainConfigurationSO terrainConfiguration = GameManager.Instance.TerrainConfiguration;

        int terrainWidth = GameManager.Instance.CurrentTerrainWidth;
        int terrainHeight = GameManager.Instance.CurrentTerrainHeight;
        int terrainEquator = terrainConfiguration.Equator;

        Terrain terrain = GameManager.Instance.Terrain;

        BlockSO dirtBlock = GameManager.Instance.BlocksAtlas.Dirt;
        BlockSO stoneBlock = GameManager.Instance.BlocksAtlas.Stone;
        BlockSO dirtWall = GameManager.Instance.BlocksAtlas.DirtWall;
        BlockSO waterBlock = GameManager.Instance.BlocksAtlas.Water;

        Parallel.For(0, terrainWidth, (index) =>
        {
            int x = index;
            for (int y = 0; y < terrainHeight; y++)
            {
                if (y % 2 == 0)
                {
                    terrain.CreateBlock(x, y, dirtBlock);
                    terrain.CreateWall(x, y, dirtWall);
                    terrain.CreateLiquidBlock(x, y, (byte)waterBlock.GetId());
                }
                else
                {
                    terrain.CreateBlock(x, y, stoneBlock);
                    terrain.CreateWall(x, y, dirtWall);
                    terrain.CreateLiquidBlock(x, y, (byte)waterBlock.GetId());
                }
            }
        });
    }
    #endregion
}
