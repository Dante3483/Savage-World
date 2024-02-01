using System.Collections.Generic;
using Random = System.Random;

public class PlantsGenerationPhase : IGenerationPhase
{
    #region Private fields
    private WorldCellData[,] _worldData = GameManager.Instance.WorldData;
    private TerrainConfigurationSO _terrainConfiguration = GameManager.Instance.TerrainConfiguration;
    private Terrain _terrain = GameManager.Instance.Terrain;
    private Random _randomVar = GameManager.Instance.RandomVar;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public string Name => "Plants generation";
    #endregion

    #region Methods
    public void StartPhase()
    {
        //Create surface plants
        List<List<PlantSO>> allPlants = new List<List<PlantSO>>()
        {
            GameManager.Instance.ObjectsAtlass.GetAllBiomePlants(BiomesID.NonBiome),
            //GameManager.Instance.ObjectsAtlass.GetAllBiomePlants(BiomesID.Ocean),
            GameManager.Instance.ObjectsAtlass.GetAllBiomePlants(BiomesID.Desert),
            GameManager.Instance.ObjectsAtlass.GetAllBiomePlants(BiomesID.Savannah),
            GameManager.Instance.ObjectsAtlass.GetAllBiomePlants(BiomesID.Meadow),
            GameManager.Instance.ObjectsAtlass.GetAllBiomePlants(BiomesID.Forest),
            //GameManager.Instance.ObjectsAtlass.GetAllBiomePlants(BiomesID.Swamp),
            GameManager.Instance.ObjectsAtlass.GetAllBiomePlants(BiomesID.ConiferousForest),
        };
        BiomeSO currentBiome;
        int chance;
        int startX;
        int endX;
        int x;
        int y;
        int terrainWidth = GameManager.Instance.CurrentTerrainWidth;

        foreach (List<PlantSO> plants in allPlants)
        {
            foreach (PlantSO plant in plants)
            {
                if (plant.BiomeId == BiomesID.NonBiome)
                {
                    startX = 0;
                    endX = terrainWidth - 1;
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
                        chance = _randomVar.Next(0, 101);
                        if (plant.AllowedToSpawnOn.Contains(_worldData[x, y].BlockData) && chance <= plant.ChanceToSpawn)
                        {
                            if (plant.IsBottomBlockSolid)
                            {
                                if (!_worldData[x, y + 1].IsEmpty())
                                {
                                    continue;
                                }
                                if (_worldData[x, y + 1].IsLiquid())
                                {
                                    continue;
                                }
                                _terrain.CreateBlock(x, y + 1, plant);
                            }
                            if (plant.IsTopBlockSolid)
                            {
                                if (!_worldData[x, y - 1].IsEmpty())
                                {
                                    continue;
                                }
                                if (_worldData[x, y - 1].IsLiquid())
                                {
                                    continue;
                                }
                                _terrain.CreateBlock(x, y - 1, plant);
                            }
                        }
                    }
                }
            }
        }
    }
    #endregion
}
