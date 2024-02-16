using Random = System.Random;

public class BiomesGenerationPhase : IGenerationPhase
{
    #region Private fields
    private WorldCellData[,] _worldData = WorldDataManager.Instance.WorldData;
    private TerrainConfigurationSO _terrainConfiguration = GameManager.Instance.TerrainConfiguration;
    private Terrain _terrain = GameManager.Instance.Terrain;
    private Random _randomVar = GameManager.Instance.RandomVar;
    private BlockSO _dirtBlock = GameManager.Instance.BlocksAtlas.Dirt;
    private BlockSO _sandBlock = GameManager.Instance.BlocksAtlas.Sand;
    private BlockSO _waterBlock = GameManager.Instance.BlocksAtlas.Water;
    private BlockSO _airBlock = GameManager.Instance.BlocksAtlas.Air;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public string Name => "Biomes generation";
    #endregion

    #region Methods
    public void StartPhase()
    {
        TerrainConfigurationSO terrainConfiguration = GameManager.Instance.TerrainConfiguration;
        CreateOcean(terrainConfiguration.Ocean);
        CreateDesert(terrainConfiguration.Desert);
        CreateSavannah(terrainConfiguration.Savannah);
        CreateMeadow(terrainConfiguration.Meadow);
        CreateForest(terrainConfiguration.Forest);
        CreateSwamp(terrainConfiguration.Swamp);
        CreateConiferousForest(terrainConfiguration.ConiferousForest);
    }

    private void CreateOcean(BiomeSO biome)
    {
        int x;
        int y;
        int startX = biome.EndX - 50;
        int startY = _terrainConfiguration.Equator;
        int downHeight = 0;
        int smoothStartY = 1;
        int maxLength = 6;
        int currentLength = 0;
        int chanceToMoveDown;
        int chanceToMoveUp;
        byte waterId = (byte)_waterBlock.GetId();

        //Hole generation
        for (x = startX; x > 5; x--)
        {
            //Clear dirt above ocean
            for (y = startY; y <= startY + biome.MountainHeight; y++)
            {
                _terrain.CreateBlock(x, y, _airBlock);
            }

            //Create hole
            for (y = startY; y >= startY - downHeight; y--)
            {
                _terrain.CreateBlock(x, y, _airBlock);
                _terrain.CreateLiquidBlock(x, y, waterId);
                ChunksManager.Instance.SetChunkBiome(x, y, biome);
            }

            chanceToMoveDown = (byte)_randomVar.Next(0, 6);
            if (chanceToMoveDown % 5 == 0 || currentLength == maxLength)
            {
                downHeight++;
                currentLength = 0;
            }
            else
            {
                currentLength++;
            }
        }
        //Define the lowest y position
        _terrainConfiguration.DeepOceanY = startY - downHeight;

        //Smooth beach and hole
        for (x = startX + 1; ; x++)
        {
            if (_worldData[x, startY + smoothStartY].CompareBlock(_airBlock))
            {
                break;
            }

            for (y = startY + smoothStartY; ; y++)
            {
                if (_worldData[x, y].CompareBlock(_airBlock))
                {
                    break;
                }
                _terrain.CreateBlock(x, y, _airBlock);
            }

            chanceToMoveUp = _randomVar.Next(0, 3);
            if (chanceToMoveUp % 2 == 0)
            {
                smoothStartY++;
            }
        }
    }

    private void CreateDesert(BiomeSO biome)
    {
        int startX = biome.EndX;
        int startY = (int)(_terrainConfiguration.Equator + biome.MountainHeight);
        int additionalHeight = 20;
        int x;
        int y;

        //Replace dirt with sand (inclusive ocean biome)
        for (x = startX; x > 5; x--)
        {
            for (y = startY; y > _terrainConfiguration.DeepOceanY - additionalHeight; y--)
            {
                if (_worldData[x, y].CompareBlock(_dirtBlock))
                {
                    _terrain.CreateBlock(x, y, _sandBlock);
                    ChunksManager.Instance.SetChunkBiome(x, y, biome);
                }
            }
        }

        //Pulverize
        int minLength = 10;
        int maxLength = 21;
        int lengthOfPulverizing;
        int additionalHeightPulverize = _randomVar.Next(10, 21) + additionalHeight;
        int chanceToPulverize;

        //Vertical
        for (y = startY; y > _terrainConfiguration.DeepOceanY - additionalHeightPulverize; y--)
        {
            lengthOfPulverizing = _randomVar.Next(minLength, maxLength);
            for (x = startX; x > startX - lengthOfPulverizing; x--)
            {
                chanceToPulverize = _randomVar.Next(0, 6);
                if (_worldData[x, y].CompareBlock(_sandBlock) &&
                    chanceToPulverize % 5 == 0)
                {
                    _terrain.CreateBlock(x, y, _dirtBlock);
                }
            }

            for (x = startX; x < startX + lengthOfPulverizing; x++)
            {
                chanceToPulverize = (byte)_randomVar.Next(0, 6);
                if (_worldData[x, y].CompareBlock(_dirtBlock) &&
                    chanceToPulverize % 5 == 0)
                {
                    _terrain.CreateBlock(x, y, _sandBlock);
                }
            }
        }

        //Horizontal
        startY = _terrainConfiguration.DeepOceanY - additionalHeight;
        for (x = startX; x > 5; x--)
        {
            lengthOfPulverizing = _randomVar.Next(minLength, maxLength);
            for (y = startY; y > startY - lengthOfPulverizing; y--)
            {
                chanceToPulverize = _randomVar.Next(0, 6);
                if (_worldData[x, y].CompareBlock(_dirtBlock) &&
                    chanceToPulverize % 5 == 0)
                {
                    _terrain.CreateBlock(x, y, _sandBlock);
                }
            }

            for (y = startY; y < startY + lengthOfPulverizing; y++)
            {
                chanceToPulverize = _randomVar.Next(0, 6);
                if (_worldData[x, y].CompareBlock(_sandBlock) &&
                    chanceToPulverize % 5 == 0)
                {
                    _terrain.CreateBlock(x, y, _dirtBlock);
                }
            }
        }
    }

    private void CreateSavannah(BiomeSO biome)
    {
        SetBiomeIntoChunk(biome);
    }

    private void CreateMeadow(BiomeSO biome)
    {
        SetBiomeIntoChunk(biome);
    }

    private void CreateForest(BiomeSO biome)
    {
        SetBiomeIntoChunk(biome);
    }

    private void CreateSwamp(BiomeSO biome)
    {
        SetBiomeIntoChunk(biome);
    }

    private void CreateConiferousForest(BiomeSO biome)
    {
        SetBiomeIntoChunk(biome);
    }

    private void SetBiomeIntoChunk(BiomeSO biome)
    {
        for (int x = biome.StartX; x < biome.EndX; x += _terrainConfiguration.ChunkSize)
        {
            ChunksManager.Instance.SetChunkBiome(x, _terrainConfiguration.Equator, biome);
        }
    }
    #endregion
}
