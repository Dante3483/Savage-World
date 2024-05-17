using System.Threading.Tasks;

public class FlatWorldGenerationPhase : IWorldProcessingPhase
{
    #region Private fields

    #endregion

    #region Public fields

    #endregion

    #region Properties
    public string Name => "Flat world generation";
    #endregion

    #region Methods

    public void StartPhase()
    {
        TerrainConfigurationSO terrainConfiguration = GameManager.Instance.TerrainConfiguration;

        TerrainLevelSO airLevel = terrainConfiguration.AirLevel;
        TerrainLevelSO surfaceLevel = terrainConfiguration.SurfaceLevel;
        TerrainLevelSO preUndergroundLevel = terrainConfiguration.PreUndergroundLevel;
        TerrainLevelSO undergroundLevel = terrainConfiguration.UndergroundLevel;

        int terrainWidth = GameManager.Instance.CurrentTerrainWidth;
        int terrainHeight = GameManager.Instance.CurrentTerrainHeight;
        int terrainEquator = terrainConfiguration.Equator;

        Terrain terrain = GameManager.Instance.Terrain;

        BlockSO dirtBlock = GameManager.Instance.BlocksAtlas.Dirt;
        BlockSO airBlock = GameManager.Instance.BlocksAtlas.Air;

        Parallel.For(0, terrainWidth, (index) =>
        {
            int x = index;

            GenerateLevel(x, undergroundLevel.StartY, undergroundLevel.EndY, dirtBlock, undergroundLevel.DefaultWall);
            GenerateLevel(x, preUndergroundLevel.StartY, preUndergroundLevel.EndY, dirtBlock, preUndergroundLevel.DefaultWall);
            GenerateLevel(x, surfaceLevel.StartY, terrainEquator, dirtBlock, surfaceLevel.DefaultWall);
            GenerateLevel(x, terrainEquator + 1, airLevel.EndY, airBlock, airLevel.DefaultWall);
        });

        void GenerateLevel(int x, int startY, int endY, BlockSO block, BlockSO wall)
        {
            for (int y = startY; y <= endY; y++)
            {
                terrain.CreateBlock(x, y, block);
                terrain.CreateWall(x, y, wall);
            }
        }
    }
    #endregion
}
