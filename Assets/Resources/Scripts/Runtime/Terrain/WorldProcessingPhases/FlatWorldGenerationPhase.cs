using System.Threading.Tasks;

public class FlatWorldGenerationPhase : WorldProcessingPhaseBase
{
    #region Fields

    #endregion

    #region Properties
    public override string Name => "Flat world generation";
    #endregion

    #region Events / Delegates

    #endregion

    #region Public Methods
    public override void StartPhase()
    {
        TerrainLevelSO airLevel = _terrainConfiguration.AirLevel;
        TerrainLevelSO surfaceLevel = _terrainConfiguration.SurfaceLevel;
        TerrainLevelSO preUndergroundLevel = _terrainConfiguration.PreUndergroundLevel;
        TerrainLevelSO undergroundLevel = _terrainConfiguration.UndergroundLevel;

        Parallel.For(0, _terrainWidth, (index) =>
        {
            int x = index;

            GenerateLevel(x, undergroundLevel.StartY, undergroundLevel.EndY, _dirt, undergroundLevel.DefaultWall);
            GenerateLevel(x, preUndergroundLevel.StartY, preUndergroundLevel.EndY, _dirt, preUndergroundLevel.DefaultWall);
            GenerateLevel(x, surfaceLevel.StartY, _equator, _dirt, surfaceLevel.DefaultWall);
            GenerateLevel(x, _equator + 1, airLevel.EndY, _air, airLevel.DefaultWall);
        });
    }
    #endregion

    #region Private Methods
    private void GenerateLevel(int x, int startY, int endY, BlockSO block, BlockSO wall)
    {
        for (int y = startY; y <= endY; y++)
        {
            SetBlockData(x, y, block);
            SetWallData(x, y, wall);
        }
    }
    #endregion
}
