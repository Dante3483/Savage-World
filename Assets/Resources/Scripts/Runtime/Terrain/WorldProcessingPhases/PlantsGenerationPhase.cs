using SavageWorld.Runtime.Enums.Id;
using SavageWorld.Runtime.Terrain.Blocks;
using SavageWorld.Runtime.Terrain.Objects;
using System.Collections.Generic;

namespace SavageWorld.Runtime.Terrain.WorldProcessingPhases
{
    public class PlantsGenerationPhase : WorldProcessingPhaseBase
    {
        #region Fields

        #endregion

        #region Properties
        public override string Name => "Plants generation";
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override void StartPhase()
        {
            BiomeSO currentBiome;
            int chance;
            int startX;
            int endX;
            int x;
            int y;

            List<List<BlockSO>> allPlants = new()
        {
            _gameManager.BlocksAtlas.GetPlantsByBiome(BiomesId.NonBiome),
            //_gameManager.ObjectsAtlass.GetAllBiomePlants(BiomesID.Ocean),
            _gameManager.BlocksAtlas.GetPlantsByBiome(BiomesId.Desert),
            _gameManager.BlocksAtlas.GetPlantsByBiome(BiomesId.Savannah),
            _gameManager.BlocksAtlas.GetPlantsByBiome(BiomesId.Meadow),
            _gameManager.BlocksAtlas.GetPlantsByBiome(BiomesId.Forest),
            //_gameManager.ObjectsAtlass.GetAllBiomePlants(BiomesID.Swamp),
            _gameManager.BlocksAtlas.GetPlantsByBiome(BiomesId.ConiferousForest),
        };

            foreach (List<BlockSO> plants in allPlants)
            {
                foreach (PlantSO plant in plants)
                {
                    if (plant.BiomeId == BiomesId.NonBiome)
                    {
                        startX = 0;
                        endX = _terrainWidth - 1;
                    }
                    else
                    {
                        currentBiome = _terrainConfiguration.GetBiome(plant.BiomeId);
                        startX = currentBiome.StartX;
                        endX = currentBiome.EndX;
                    }

                    for (x = startX; x <= endX; x++)
                    {
                        for (y = _terrainConfiguration.SurfaceLevel.StartY; y < _terrainConfiguration.SurfaceLevel.EndY; y++)
                        {
                            chance = GetNextRandomValue(0, 101);
                            if (plant.AllowedToSpawnOn.Contains(GetBlockData(x, y)) && chance <= plant.ChanceToSpawn)
                            {
                                if (plant.IsBottomBlockSolid)
                                {
                                    if (!IsEmpty(x, y + 1))
                                    {
                                        continue;
                                    }
                                    if (IsLiquid(x, y + 1))
                                    {
                                        continue;
                                    }
                                    SetBlockData(x, y + 1, plant);
                                }
                                if (plant.IsTopBlockSolid)
                                {
                                    if (!IsEmpty(x, y - 1))
                                    {
                                        continue;
                                    }
                                    if (IsLiquid(x, y - 1))
                                    {
                                        continue;
                                    }
                                    SetBlockData(x, y - 1, plant);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Private Methods

        #endregion
    }
}