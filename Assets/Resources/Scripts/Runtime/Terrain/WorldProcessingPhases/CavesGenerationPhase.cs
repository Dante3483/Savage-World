using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CavesGenerationPhase : WorldProcessingPhaseBase
{
    #region Fields
    byte[,] _visitedCaveMap;
    #endregion

    #region Properties
    public override string Name => "Caves generation";
    #endregion

    #region Events / Delegates

    #endregion

    #region Public Methods
    public override void StartPhase()
    {
        int count;
        int octaves = _terrainConfiguration.Octaves;
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;
        float scale = _terrainConfiguration.Scale;
        float persistance = _terrainConfiguration.Persistance;
        float lacunarity = _terrainConfiguration.Lacunarity;
        bool isNonChunk;

        float[,] caveMap = new float[_terrainWidth, _terrainHeight];
        _visitedCaveMap = new byte[_terrainWidth, _terrainHeight];
        Vector2[] octaveOffset = new Vector2[octaves];

        List<Vector2Int> caveCoords = new();

        for (int i = 0; i < octaves; i++)
        {
            octaveOffset[i].x = GetNextRandomValue(-100000, 100000);
            octaveOffset[i].y = GetNextRandomValue(-100000, 100000);
        }

        //Create noise map
        Parallel.For(0, _terrainWidth, (index) =>
        {
            int x = index;
            float amplitude;
            float frequency;
            float noiseHeight;
            float sampleX;
            float sampleY;
            float perlinValue;

            for (int y = 0; y < _terrainHeight; y++)
            {
                amplitude = 1;
                frequency = 1;
                noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    sampleX = x / scale * frequency + octaveOffset[i].x;
                    sampleY = y / scale * frequency + octaveOffset[i].y;

                    perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                caveMap[x, y] = noiseHeight;
            }
        });

        //Find max and min noise height
        Parallel.For(0, _terrainWidth, (index) =>
        {
            float localMin = float.MaxValue;
            float localMax = float.MinValue;

            for (int y = 0; y < _terrainHeight; y++)
            {
                float value = caveMap[index, y];
                localMin = Math.Min(localMin, value);
                localMax = Math.Max(localMax, value);
            }

            lock (this)
            {
                minNoiseHeight = Math.Min(minNoiseHeight, localMin);
                maxNoiseHeight = Math.Max(maxNoiseHeight, localMax);
            }
        });

        //Lerp noise
        Parallel.For(0, _terrainWidth, (index) =>
        {
            int x = index;
            for (int y = 0; y < _terrainHeight; y++)
            {
                caveMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, caveMap[x, y]);
            }
        });

        //Create cave in conditions
        for (int x = 0; x < _terrainWidth; x++)
        {
            for (int y = 0; y < _terrainHeight; y++)
            {
                if (caveMap[x, y] <= _terrainConfiguration.Intensity && _visitedCaveMap[x, y] == 0)
                {
                    (count, isNonChunk) = FloodFill(x, y, caveMap, ref caveCoords);
                    if (!isNonChunk)
                    {
                        caveCoords.Clear();
                        continue;
                    }
                    if ((count > _terrainConfiguration.MinSmallCaveSize && count < _terrainConfiguration.MaxSmallCaveSize) ||
                        (count > _terrainConfiguration.MinLargeCaveSize && count < _terrainConfiguration.MaxLargeCaveSize))
                    {
                        foreach (Vector2Int coord in caveCoords)
                        {
                            SetBlockData(coord.x, coord.y, _air);
                        }
                    }
                    caveCoords.Clear();
                }
            }
        }
    }
    #endregion

    #region Private Methods
    private (int, bool) FloodFill(int startX, int startY, float[,] map, ref List<Vector2Int> connected)
    {
        try
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            int regionSize = 0;
            int x;
            int y;
            bool isNonChunk = true;
            Queue<(int, int)> queue = new();
            Vector2Int vector = new();
            queue.Enqueue((startX, startY));

            while (queue.Count > 0)
            {
                (x, y) = queue.Dequeue();

                if (x < 0 || x >= width || y < 0 || y >= height || map[x, y] >= _terrainConfiguration.Intensity || _visitedCaveMap[x, y] == 1)
                {
                    continue;
                }

                _visitedCaveMap[x, y] = 1;
                vector.x = x;
                vector.y = y;
                connected.Add(new Vector2Int(x, y));
                if (_chunksManager.GetChunk(x, y).Biome.Id != BiomesID.NonBiome)
                {
                    isNonChunk = false;
                }
                regionSize++;

                queue.Enqueue((x + 1, y));
                queue.Enqueue((x - 1, y));
                queue.Enqueue((x, y + 1));
                queue.Enqueue((x, y - 1));
            }

            queue = null;

            return (regionSize, isNonChunk);
        }
        catch (Exception e)
        {
            throw e;
        }
    }
    #endregion
}
