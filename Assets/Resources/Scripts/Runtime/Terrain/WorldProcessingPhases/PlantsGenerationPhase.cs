using System.Collections.Generic;

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
            _gameManager.BlocksAtlas.GetPlantsByBiome(BiomesID.NonBiome),
            //_gameManager.ObjectsAtlass.GetAllBiomePlants(BiomesID.Ocean),
            _gameManager.BlocksAtlas.GetPlantsByBiome(BiomesID.Desert),
            _gameManager.BlocksAtlas.GetPlantsByBiome(BiomesID.Savannah),
            _gameManager.BlocksAtlas.GetPlantsByBiome(BiomesID.Meadow),
            _gameManager.BlocksAtlas.GetPlantsByBiome(BiomesID.Forest),
            //_gameManager.ObjectsAtlass.GetAllBiomePlants(BiomesID.Swamp),
            _gameManager.BlocksAtlas.GetPlantsByBiome(BiomesID.ConiferousForest),
        };

        foreach (List<BlockSO> plants in allPlants)
        {
            foreach (PlantSO plant in plants)
            {
                if (plant.BiomeId == BiomesID.NonBiome)
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
