using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

public class CavesGenerationPhase : IGenerationPhase
{
    #region Private fields
    byte[,] _visitedCaveMap;
    private TerrainConfigurationSO _terrainConfiguration = GameManager.Instance.TerrainConfiguration;
    private Terrain _terrain = GameManager.Instance.Terrain;
    private Random _randomVar = GameManager.Instance.RandomVar;
    private BlockSO _airBlock = GameManager.Instance.BlocksAtlas.Air;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public string Name => "Caves generation";
    #endregion

    #region Methods
    public void StartPhase()
    {
        int terrainWidth = GameManager.Instance.CurrentTerrainWidth;
        int terrainHeight = GameManager.Instance.CurrentTerrainHeight;
        int count;
        int octaves = _terrainConfiguration.Octaves;
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;
        float scale = _terrainConfiguration.Scale;
        float persistance = _terrainConfiguration.Persistance;
        float lacunarity = _terrainConfiguration.Lacunarity;
        bool isNonChunk;

        float[,] caveMap = new float[terrainWidth, terrainHeight];
        _visitedCaveMap = new byte[terrainWidth, terrainHeight];
        Vector2[] octaveOffset = new Vector2[octaves];

        List<Vector2Int> caveCoords = new List<Vector2Int>();

        for (int i = 0; i < octaves; i++)
        {
            octaveOffset[i].x = _randomVar.Next(-100000, 100000);
            octaveOffset[i].y = _randomVar.Next(-100000, 100000);
        }

        //Create noise map
        Parallel.For(0, terrainWidth, (index) =>
        {
            int x = index;
            float amplitude;
            float frequency;
            float noiseHeight;
            float sampleX;
            float sampleY;
            float perlinValue;

            for (int y = 0; y < terrainHeight; y++)
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
        Parallel.For(0, terrainWidth, (index) =>
        {
            float localMin = float.MaxValue;
            float localMax = float.MinValue;

            for (int y = 0; y < terrainHeight; y++)
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
        Parallel.For(0, terrainWidth, (index) =>
        {
            int x = index;
            for (int y = 0; y < terrainHeight; y++)
            {
                caveMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, caveMap[x, y]);
            }
        });

        //Create cave in conditions
        for (int x = 0; x < terrainWidth; x++)
        {
            for (int y = 0; y < terrainHeight; y++)
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
                        foreach (Vector2Int v in caveCoords)
                        {
                            _terrain.CreateBlock(v.x, v.y, _airBlock);
                        }
                    }
                    caveCoords.Clear();
                }
            }
        }
    }

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
            Queue<(int, int)> queue = new Queue<(int, int)>();
            Vector2Int vector = new Vector2Int();
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
                if (GameManager.Instance.GetChunk(x, y).Biome.Id != BiomesID.NonBiome)
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
