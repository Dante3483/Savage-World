using SavageWorld.Runtime.Enums.Id;
using SavageWorld.Runtime.Terrain.Objects;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain.Generation.Phases
{
    public class LandscapeGenerationPhase : WorldProcessingPhaseBase
    {
        #region Fields

        #endregion

        #region Properties
        public override string Name => "Landscape generation";
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override void StartPhase()
        {
            int startY = _equator + 1;
            int firstHeight;
            int dif;
            int sign;
            int heightAdder;
            int height = 0;
            int prevHeight = -1;
            int x;
            int y;

            Vector2Int vector = new();

            foreach (BiomeSO biome in _terrainConfiguration.Biomes)
            {
                //Skip NonBiome
                if (biome.Id == BiomesId.NonBiome)
                {
                    continue;
                }
                //Calculate difference of height between two biomes
                firstHeight = (int)(startY + Mathf.PerlinNoise((biome.StartX + _seed) / biome.MountainCompression, _seed / biome.MountainCompression) * biome.MountainHeight);
                dif = prevHeight != -1 ? prevHeight - firstHeight : 0;
                sign = dif < 0 ? 1 : -1;
                heightAdder = dif;

                //Create landscape
                for (x = biome.StartX; x <= biome.EndX; x++)
                {
                    //Calculate maximum height
                    height = (int)(Mathf.PerlinNoise((x + _seed) / biome.MountainCompression, _seed / biome.MountainCompression) * biome.MountainHeight);
                    height += startY + heightAdder;
                    for (y = startY; y <= height; y++)
                    {
                        SetBlockData(x, y, _dirt);
                    }
                    vector.x = x;
                    vector.y = y - 1;
                    _surfaceCoords.Add(vector);

                    //Change diference
                    if (heightAdder != 0)
                    {
                        heightAdder += sign * GetNextRandomValue(0, 2);
                    }
                }
                prevHeight = height;
            }
        }
        #endregion

        #region Private Methods

        #endregion
    }
}