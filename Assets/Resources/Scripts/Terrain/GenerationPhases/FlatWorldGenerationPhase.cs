using System.Threading.Tasks;

public class FlatWorldGenerationPhase : IGenerationPhase
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

            GenerateLevel(x, undergroundLevel.StartY, undergroundLevel.EndY, dirtBlock, undergroundLevel.DefaultBackground);
            GenerateLevel(x, preUndergroundLevel.StartY, preUndergroundLevel.EndY, dirtBlock, preUndergroundLevel.DefaultBackground);
            GenerateLevel(x, surfaceLevel.StartY, terrainEquator, dirtBlock, surfaceLevel.DefaultBackground);
            GenerateLevel(x, terrainEquator + 1, airLevel.EndY, airBlock, airLevel.DefaultBackground);
        });

        void GenerateLevel(int x, int startY, int endY, BlockSO block, BlockSO background)
        {
            for (int y = startY; y <= endY; y++)
            {
                terrain.CreateBlock(x, y, block);
                terrain.CreateBackground(x, y, background);
            }
        }
    }
    #endregion
}
