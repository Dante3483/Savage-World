using SavageWorld.Runtime.Terrain.Objects;

namespace SavageWorld.Runtime.Terrain.Generation.Phases
{
    public class BiomesSettingPhase : WorldProcessingPhaseBase
    {
        #region Fields
        private int _oceanLowestY;
        #endregion

        #region Properties
        public override string Name => "Biomes setting";
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override void StartPhase()
        {
            CreateOcean(_terrainConfiguration.Ocean);
            CreateDesert(_terrainConfiguration.Desert);
            CreateSavannah(_terrainConfiguration.Savannah);
            CreateMeadow(_terrainConfiguration.Meadow);
            CreateForest(_terrainConfiguration.Forest);
            CreateSwamp(_terrainConfiguration.Swamp);
            CreateConiferousForest(_terrainConfiguration.ConiferousForest);
        }
        #endregion

        #region Private Methods
        private void CreateOcean(BiomeSO biome)
        {
            int x;
            int y;
            int startX = biome.EndX - 50;
            int startY = _equator;
            int downHeight = 0;
            int smoothStartY = 1;
            int maxLength = 6;
            int currentLength = 0;
            int chanceToMoveDown;
            int chanceToMoveUp;

            //Hole generation
            for (x = startX; x > 5; x--)
            {
                //Clear dirt above ocean
                for (y = startY; y <= startY + biome.MountainHeight; y++)
                {
                    SetBlockData(x, y, _air);
                }

                //Create hole
                for (y = startY; y >= startY - downHeight; y--)
                {
                    SetBlockData(x, y, _air);
                    SetLiquidData(x, y, _water);
                    SetChunkBiome(x, y, biome);
                }

                chanceToMoveDown = GetNextRandomValue(0, 6);
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
            _oceanLowestY = startY - downHeight;

            //Smooth beach and hole
            for (x = startX + 1; ; x++)
            {
                if (CompareBlock(x, startY + smoothStartY, _air))
                {
                    break;
                }

                for (y = startY + smoothStartY; ; y++)
                {
                    if (CompareBlock(x, y, _air))
                    {
                        break;
                    }
                    SetBlockData(x, y, _air);
                }

                chanceToMoveUp = GetNextRandomValue(0, 3);
                if (chanceToMoveUp % 2 == 0)
                {
                    smoothStartY++;
                }
            }
        }

        private void CreateDesert(BiomeSO biome)
        {
            int startX = biome.EndX;
            int startY = _equator + (int)biome.MountainHeight;
            int additionalHeight = 20;
            int x;
            int y;

            //Replace dirt with sand (inclusive ocean biome)
            for (x = startX; x > 5; x--)
            {
                for (y = startY; y > _oceanLowestY - additionalHeight; y--)
                {
                    if (CompareBlock(x, y, _dirt))
                    {
                        SetBlockData(x, y, _sand);
                        SetChunkBiome(x, y, biome);
                    }
                }
            }

            //Pulverize
            int minLength = 10;
            int maxLength = 21;
            int lengthOfPulverizing;
            int additionalHeightPulverize = GetNextRandomValue(10, 21) + additionalHeight;
            int chanceToPulverize;

            //Vertical
            for (y = startY; y > _oceanLowestY - additionalHeightPulverize; y--)
            {
                lengthOfPulverizing = GetNextRandomValue(minLength, maxLength);
                for (x = startX; x > startX - lengthOfPulverizing; x--)
                {
                    chanceToPulverize = GetNextRandomValue(0, 6);
                    if (CompareBlock(x, y, _sand) &&
                        chanceToPulverize % 5 == 0)
                    {
                        SetBlockData(x, y, _dirt);
                    }
                }

                for (x = startX; x < startX + lengthOfPulverizing; x++)
                {
                    chanceToPulverize = GetNextRandomValue(0, 6);
                    if (CompareBlock(x, y, _dirt) &&
                        chanceToPulverize % 5 == 0)
                    {
                        SetBlockData(x, y, _sand);
                    }
                }
            }

            //Horizontal
            startY = _oceanLowestY - additionalHeight;
            for (x = startX; x > 5; x--)
            {
                lengthOfPulverizing = GetNextRandomValue(minLength, maxLength);
                for (y = startY; y > startY - lengthOfPulverizing; y--)
                {
                    chanceToPulverize = GetNextRandomValue(0, 6);
                    if (CompareBlock(x, y, _dirt) &&
                        chanceToPulverize % 5 == 0)
                    {
                        SetBlockData(x, y, _sand);
                    }
                }

                for (y = startY; y < startY + lengthOfPulverizing; y++)
                {
                    chanceToPulverize = GetNextRandomValue(0, 6);
                    if (CompareBlock(x, y, _sand) &&
                        chanceToPulverize % 5 == 0)
                    {
                        SetBlockData(x, y, _dirt);
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
                SetChunkBiome(x, _terrainConfiguration.Equator, biome);
            }
        }

        private void SetChunkBiome(int x, int y, BiomeSO biome)
        {
            _chunksManager.SetChunkBiome(x, y, biome);
        }
        #endregion
    }
}