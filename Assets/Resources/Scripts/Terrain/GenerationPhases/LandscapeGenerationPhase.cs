using UnityEngine;
using Random = System.Random;

public class LandscapeGenerationPhase : IGenerationPhase
{
    #region Private fields

    #endregion

    #region Public fields

    #endregion

    #region Properties
    public string Name => "Landscape generation";
    #endregion

    #region Methods
    public void StartPhase()
    {
        TerrainConfigurationSO terrainConfiguration = GameManager.Instance.TerrainConfiguration;
        Terrain terrain = GameManager.Instance.Terrain;
        Random randomVar = GameManager.Instance.RandomVar;

        BlockSO dirtBlock = GameManager.Instance.ObjectsAtlass.Dirt;

        int startY = terrainConfiguration.Equator + 1;
        int seed = GameManager.Instance.Seed;
        int firstHeight;
        int dif;
        int sign;
        int heightAdder;
        int height = 0;
        int prevHeight = -1;
        int x;
        int y;
        
        Vector2Int vector = new Vector2Int();

        foreach (BiomeSO biome in terrainConfiguration.Biomes)
        {
            //Skip NonBiome
            if (biome.Id == BiomesID.NonBiome)
            {
                continue;
            }
            //Calculate difference of height between two biomes
            firstHeight = (int)(startY + Mathf.PerlinNoise((biome.StartX + seed) / biome.MountainCompression, seed / biome.MountainCompression) * biome.MountainHeight);
            dif = prevHeight != -1 ? prevHeight - firstHeight : 0;
            sign = dif < 0 ? 1 : -1;
            heightAdder = dif;

            //Create landscape
            for (x = biome.StartX; x <= biome.EndX; x++)
            {
                //Calculate maximum height
                height = (int)(Mathf.PerlinNoise((x + seed) / biome.MountainCompression, seed / biome.MountainCompression) * biome.MountainHeight);
                height += startY + heightAdder;
                for (y = startY; y <= height; y++)
                {
                    terrain.CreateBlock(x, y, dirtBlock);
                }
                vector.x = x;
                vector.y = y - 1;
                TerrainGeneration.SurfaceCoords.Add(vector);

                //Change diference
                if (heightAdder != 0)
                {
                    heightAdder += sign * randomVar.Next(0, 2);
                }
            }
            prevHeight = height;
        }
    }
    #endregion
}
