using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TerrainGeneration
{
    #region Private fields
    private TerrainConfigurationSO _terrainConfiguration;
    private int _seed;
    private System.Random _randomVar;
    private float[,] _caveMap;
    private float _maxNoiseHeight = float.MinValue;
    private float _minNoiseHeight = float.MaxValue;
    private object _lockObject = new object();
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public TerrainConfigurationSO TerrainConfiguration
    {
        get
        {
            return _terrainConfiguration;
        }

        set
        {
            _terrainConfiguration = value;
        }
    }

    public System.Random RandomVar
    {
        get
        {
            return _randomVar;
        }

        set
        {
            _randomVar = value;
        }
    }

    public int Seed
    {
        get
        {
            return _seed;
        }

        set
        {
            _seed = value;
        }
    }
    #endregion

    #region Methods

    #region General
    public TerrainGeneration(int seed)
    {
        TerrainConfiguration = GameManager.Instance.TerrainConfiguration;
        Seed = seed;
        RandomVar = GameManager.Instance.RandomVar;
    }

    public void StartTerrainGeneration()
    {
        double totalTime = 0f;
        float step = 100f / 5;

        #region Phase 1 - Flat world generation
        var watch = System.Diagnostics.Stopwatch.StartNew();

        CreateFlatWorld();

        watch.Stop();
        Debug.Log($"Phase 1: {watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 1: {watch.Elapsed.TotalSeconds}\n";
        totalTime += watch.Elapsed.TotalSeconds;
        GameManager.Instance.LoadingValue += step;
        #endregion

        #region Phase 2 - Landscape generation
        watch.Restart();

        CreateLandscape();

        watch.Stop();
        Debug.Log($"Phase 2: {watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 2: {watch.Elapsed.TotalSeconds}\n";
        totalTime += watch.Elapsed.TotalSeconds;
        GameManager.Instance.LoadingValue += step;
        #endregion

        #region Phase 3 - Biomes generation
        watch.Restart();

        CreateBiomes();

        watch.Stop();
        Debug.Log($"Phase 3: {watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 3: {watch.Elapsed.TotalSeconds}\n";
        totalTime += watch.Elapsed.TotalSeconds;
        GameManager.Instance.LoadingValue += step;
        #endregion

        #region Phase 4 - Clusters generation
        watch.Restart();

        CreateClusters();

        watch.Stop();
        Debug.Log($"Phase 4: {watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 4: {watch.Elapsed.TotalSeconds}\n";
        totalTime += watch.Elapsed.TotalSeconds;
        GameManager.Instance.LoadingValue += step;
        #endregion

        #region Phase 5 - Caves generation
        watch.Restart();

        CreateCaves();

        watch.Stop();
        Debug.Log($"Phase 5: {watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 5: {watch.Elapsed.TotalSeconds}\n";
        totalTime += watch.Elapsed.TotalSeconds;
        GameManager.Instance.LoadingValue += step;
        #endregion

        Debug.Log($"Total time: {totalTime}");
        GameManager.Instance.GeneralInfo += $"Total time: {totalTime}\n";
    }
    #endregion

    #region Phases

    #region Phase 1
    private void CreateFlatWorld()
    {
        BlockSO block = GameManager.Instance.ObjectsAtlass.Dirt;
        for (ushort x = 0; x < GameManager.Instance.CurrentTerrainWidth; x++)
        {
            for (ushort y = 0; y <= TerrainConfiguration.Equator; y++)
            {
                Terrain.CreateBlock(x, y, block);
            }
        }
    }
    #endregion

    #region Phase 2
    private void CreateLandscape()
    {
        BlockSO block = GameManager.Instance.ObjectsAtlass.Dirt;
        ushort startY = (ushort)(TerrainConfiguration.Equator + 1);
        short prevHeight = -1;
        short firstHeight = -1;
        short sign = 0;
        short heightAdder = 0;
        short height = 0;

        foreach (BiomeSO biome in TerrainConfiguration.Biomes)
        {
            //Calculate difference of height between two biomes
            firstHeight = (short)(startY + Mathf.PerlinNoise((biome.StartX + Seed) / biome.MountainCompression, Seed / biome.MountainCompression) * biome.MountainHeight);
            short dif = (short)(prevHeight != -1 ? (short)(prevHeight - firstHeight) : 0);
            sign = (short)(dif < 0 ? 1 : -1);
            heightAdder = dif;

            //Create landscape
            for (ushort x = biome.StartX; x <= biome.EndX; x++)
            {
                //Calculate maximum height
                height = (short)(Mathf.PerlinNoise((x + Seed) / biome.MountainCompression, Seed / biome.MountainCompression) * biome.MountainHeight);
                height += (short)(startY + heightAdder);
                for (ushort y = startY; y <= height; y++)
                {
                    Terrain.CreateBlock(x, y, block);
                }

                //Change diference
                if (heightAdder != 0)
                {
                    heightAdder += (short)(sign * RandomVar.Next(0, 2));
                }
            }
            prevHeight = height;
        }
    }
    #endregion

    #region Phase 3
    private void CreateBiomes()
    {
        CreateOcean(TerrainConfiguration.Biomes[1]);
        CreateDesert(TerrainConfiguration.Biomes[2]);
        CreateSavannah(TerrainConfiguration.Biomes[3]);
        CreateMeadow(TerrainConfiguration.Biomes[4]);
        CreateForest(TerrainConfiguration.Biomes[5]);
        CreateSwamp(TerrainConfiguration.Biomes[6]);
        CreateConiferousForest(TerrainConfiguration.Biomes[7]);
    }

    //Phase 3.1
    private void CreateOcean(BiomeSO biome)
    {
        BlockSO airBlock = GameManager.Instance.ObjectsAtlass.Air;
        BlockSO waterBlock = GameManager.Instance.ObjectsAtlass.Water;

        ushort startX = (ushort)(biome.EndX - 50);
        ushort startY = TerrainConfiguration.Equator;
        ushort downHeight = 0;
        byte maxLength = 6;
        byte currentLength = 0;

        //Hole generation
        for (ushort x = startX; x > 5; x--)
        {
            //Clear dirt above ocean
            for (ushort y = startY; y <= startY + biome.MountainHeight; y++)
            {
                Terrain.CreateBlock(x, y, airBlock);
            }

            //Create hole
            for (ushort y = startY; y >= startY - downHeight; y--)
            {
                Terrain.CreateBlock(x, y, waterBlock);
                GameManager.Instance.SetChunk(x, y, biome);
            }

            byte chanceToMoveDown = (byte)RandomVar.Next(0, 6);
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
        TerrainConfiguration.DeepOceanY = (ushort)(startY - downHeight);

        //Smooth beach and hole
        ushort smoothStartY = 1;
        for (ushort x = (ushort)(startX + 1); ; x++)
        {
            if (GameManager.Instance.WorldData[x, startY + smoothStartY].CompareBlock(airBlock))
            {
                break;
            }

            for (ushort y = (ushort)(startY + smoothStartY); ; y++)
            {
                if (GameManager.Instance.WorldData[x, y].CompareBlock(airBlock))
                {
                    break;
                }
                Terrain.CreateBlock(x, y, airBlock);
            }

            byte chanceToMoveUp = (byte)RandomVar.Next(0, 3);
            if (chanceToMoveUp % 2 == 0)
            {
                smoothStartY++;
            }
        }
    }

    //Phase 3.2
    private void CreateDesert(BiomeSO biome)
    {
        BlockSO dirtBlock = GameManager.Instance.ObjectsAtlass.Dirt;
        BlockSO sandBlock = GameManager.Instance.ObjectsAtlass.Sand;

        ushort startX = biome.EndX;
        ushort startY = (ushort)(TerrainConfiguration.Equator + biome.MountainHeight);
        ushort additionalHeight = 20;

        //Replace dirt with sand (inclusive ocean biome)
        for (ushort x = startX; x > 5; x--)
        {
            for (ushort y = startY; y > TerrainConfiguration.DeepOceanY - additionalHeight; y--)
            {
                if (GameManager.Instance.WorldData[x, y].CompareBlock(dirtBlock))
                {
                    Terrain.CreateBlock(x, y, sandBlock);
                    GameManager.Instance.SetChunk(x, y, biome);
                }
            }
        }

        //Pulverize
        ushort minLength = 10;
        ushort maxLength = 21;
        ushort lengthOfPulverizing;
        ushort additionalHeightPulverize = (ushort)(RandomVar.Next(10, 21) + additionalHeight);

        //Vertical
        for (ushort y = startY; y > TerrainConfiguration.DeepOceanY - additionalHeightPulverize; y--)
        {
            lengthOfPulverizing = (ushort)RandomVar.Next(minLength, maxLength);
            for (ushort x = startX; x > startX - lengthOfPulverizing; x--)
            {
                byte chanceToPulverize = (byte)RandomVar.Next(0, 6);
                if (GameManager.Instance.WorldData[x, y].CompareBlock(sandBlock) &&
                    chanceToPulverize % 5 == 0)
                {
                    Terrain.CreateBlock(x, y, dirtBlock);
                }
            }

            for (ushort x = startX; x < startX + lengthOfPulverizing; x++)
            {
                byte chanceToPulverize = (byte)RandomVar.Next(0, 6);
                if (GameManager.Instance.WorldData[x, y].CompareBlock(dirtBlock) &&
                    chanceToPulverize % 5 == 0)
                {
                    Terrain.CreateBlock(x, y, sandBlock);
                }
            }
        }

        //Horizontal
        startY = (ushort)(TerrainConfiguration.DeepOceanY - additionalHeight);
        for (ushort x = startX; x > 5; x--)
        {
            lengthOfPulverizing = (ushort)RandomVar.Next(minLength, maxLength);
            for (ushort y = startY; y > startY - lengthOfPulverizing; y--)
            {
                byte chanceToPulverize = (byte)RandomVar.Next(0, 6);
                if (GameManager.Instance.WorldData[x, y].CompareBlock(dirtBlock) &&
                    chanceToPulverize % 5 == 0)
                {
                    Terrain.CreateBlock(x, y, sandBlock);
                }
            }

            for (ushort y = startY; y < startY + lengthOfPulverizing; y++)
            {
                byte chanceToPulverize = (byte)RandomVar.Next(0, 6);
                if (GameManager.Instance.WorldData[x, y].CompareBlock(sandBlock) &&
                    chanceToPulverize % 5 == 0)
                {
                    Terrain.CreateBlock(x, y, dirtBlock);
                }
            }
        }
    }

    //Phase 3.3
    private void CreateSavannah(BiomeSO biome)
    {
        SetBiomeIntoChunk(biome);
    }

    //Phase 3.4
    private void CreateMeadow(BiomeSO biome)
    {
        SetBiomeIntoChunk(biome);
    }

    //Phase 3.5
    private void CreateForest(BiomeSO biome)
    {
        SetBiomeIntoChunk(biome);
    }

    //Phase 3.6
    private void CreateSwamp(BiomeSO biome)
    {
        SetBiomeIntoChunk(biome);
    }

    //Phase 3.7
    private void CreateConiferousForest(BiomeSO biome)
    {
        SetBiomeIntoChunk(biome);
    }
    #endregion

    #region Phase 4
    private void CreateClusters()
    {
        foreach (ClusterSO cluster in TerrainConfiguration.Clusters)
        {
            CreateCluster(cluster, RandomVar.Next(0, 1000000));
        }
    }

    private void CreateCluster(ClusterSO cluster, int additionalSeed)
    {
        List<Thread> threads = new List<Thread>();
        byte i = 0;

        foreach (TerrainLevelSO level in TerrainConfiguration.Levels)
        {
            for (ushort y = level.StartY; y < level.EndY; y += TerrainConfiguration.ChunkSize)
            {
                threads.Add(new Thread(GenerateClusterLine));
                threads[i].Start(new Tuple<TerrainLevelSO, ClusterSO, ushort, int>(level, cluster, y, additionalSeed));
                i++;
            }
        }

        foreach (Thread thread in threads)
        {
            thread.Join();
        }
    }

    private void GenerateClusterLine(object obj)
    {
        Tuple<TerrainLevelSO, ClusterSO, ushort, int> data = (Tuple<TerrainLevelSO, ClusterSO, ushort, int>)obj;
        ClusterSO cluster = data.Item2;
        ClusterSO.ClusterData clusterData = data.Item2.GetClusterData(data.Item1);
        ushort startY = data.Item3;
        int additionalSeed = data.Item4;

        for (ushort x = 0; x < GameManager.Instance.CurrentTerrainWidth; x++)
        {
            for (ushort y = startY; y < startY + TerrainConfiguration.ChunkSize; y++)
            {
                if (!cluster.CompareBlock(GameManager.Instance.WorldData[x, y].BlockData))
                {
                    if (GenerateNoise(x, y, clusterData.Scale, clusterData.Amplitude, additionalSeed) >= clusterData.Intensity)
                    {
                        Terrain.CreateBlock(x, y, data.Item2.Block);
                    }
                }
            }
        }
    }
    #endregion

    #region Phase 5
    private void CreateCaves()
    {
        _caveMap = new float[GameManager.Instance.CurrentTerrainWidth, GameManager.Instance.CurrentTerrainHeight];
        int[,] visited = new int[GameManager.Instance.CurrentTerrainWidth, GameManager.Instance.CurrentTerrainHeight];
        List<Thread> threads = new List<Thread>();
        List<Vector2Int> caveCoords = new List<Vector2Int>();
        BlockSO airBlock = GameManager.Instance.ObjectsAtlass.Air;
        short i = 0;

        //Create noise map
        foreach (Chunk chunk in GameManager.Instance.Chunks)
        {
            threads.Add(new Thread(CreateCave));
            threads[i].Start(new Tuple<ushort, ushort>(chunk.Coords.x, chunk.Coords.y));
            i++;
        }

        //Wait until all thread done
        foreach (Thread thread in threads)
        {
            thread.Join();
        }

        //Lerp noise
        for (ushort x = 0; x < GameManager.Instance.CurrentTerrainWidth; x++)
        {
            for (ushort y = 0; y < GameManager.Instance.CurrentTerrainHeight; y++)
            {
                _caveMap[x, y] = Mathf.InverseLerp(_minNoiseHeight, _maxNoiseHeight, _caveMap[x, y]);
            }
        }

        //Create cave in conditions
        for (ushort x = 0; x < GameManager.Instance.CurrentTerrainWidth; x++)
        {
            for (ushort y = 0; y < GameManager.Instance.CurrentTerrainHeight; y++)
            {
                if (_caveMap[x, y] <= TerrainConfiguration.Intensity && visited[x, y] == 0)
                {
                    (int count, bool isNonChunk) = FloodFill(x, y, _caveMap, visited, ref caveCoords);
                    if (!isNonChunk)
                    {
                        caveCoords.Clear();
                        continue;
                    }
                    if ((count > TerrainConfiguration.MinSmallCaveSize && count < TerrainConfiguration.MaxSmallCaveSize) ||
                        (count > TerrainConfiguration.MinLargeCaveSize && count < TerrainConfiguration.MaxLargeCaveSize))
                    {
                        foreach (Vector2Int v in caveCoords)
                        {
                            Terrain.CreateBlock((ushort)v.x, (ushort)v.y, airBlock);
                        }
                    }
                    caveCoords.Clear();
                }
            }
        }
    }

    private void CreateCave(object obj)
    {
        Tuple<ushort, ushort> data = (Tuple<ushort, ushort>)obj;
        ushort startX = (ushort)(data.Item1 * TerrainConfiguration.ChunkSize);
        ushort startY = (ushort)(data.Item2 * TerrainConfiguration.ChunkSize);
        float scale = TerrainConfiguration.Scale;
        int octaves = TerrainConfiguration.Octaves;
        float persistance = TerrainConfiguration.Persistance;
        float lacunarity = TerrainConfiguration.Lacunarity;
        System.Random randomVar = new System.Random(Seed);
        Vector2[] octaveOffset = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = randomVar.Next(-100000, 100000);
            float offsetY = randomVar.Next(-100000, 100000);
            octaveOffset[i] = new Vector2(offsetX, offsetY);
        }

        for (ushort x = startX; x < startX + TerrainConfiguration.ChunkSize; x++)
        {
            for (ushort y = startY; y < startY + TerrainConfiguration.ChunkSize; y++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = x / scale * frequency + octaveOffset[i].x;
                    float sampleY = y / scale * frequency + octaveOffset[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                lock (_lockObject)
                {
                    if (noiseHeight > _maxNoiseHeight)
                    {
                        _maxNoiseHeight = noiseHeight;
                    }
                    else if (noiseHeight < _minNoiseHeight)
                    {
                        _minNoiseHeight = noiseHeight;
                    }
                }
                _caveMap[x, y] = noiseHeight;
            }
        }
    }
    #endregion

    #endregion

    #region Validation

    #endregion

    #region Helpful
    public float GenerateNoise(int x, int y, float scale, float amplitude, int additionalSeed = 0)
    {
        return Mathf.PerlinNoise((x + Seed + additionalSeed) / scale, (y + Seed + additionalSeed) / scale) * amplitude;
    }

    public float GenerateNoiseLandscape(int x, float compression, float height)
    {
        return Mathf.PerlinNoise((x + Seed) / compression, Seed / compression) * height;
    }

    private void SetBiomeIntoChunk(BiomeSO biome)
    {
        for (ushort x = biome.StartX; x < biome.EndX; x += TerrainConfiguration.ChunkSize)
        {
            GameManager.Instance.SetChunk(x, TerrainConfiguration.Equator, biome);
        }
    }

    private (int, bool) FloodFill(int startX, int startY, float[,] map, int[,] visited, ref List<Vector2Int> connected)
    {
        try
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            int regionSize = 0;
            bool isNonChunk = true;
            Queue<(int, int)> queue = new Queue<(int, int)>();
            queue.Enqueue((startX, startY));

            while (queue.Count > 0)
            {
                (int x, int y) = queue.Dequeue();

                if (x < 0 || x >= width || y < 0 || y >= height || map[x, y] >= TerrainConfiguration.Intensity || visited[x, y] == 1)
                {
                    continue;
                }

                visited[x, y] = 1;
                connected.Add(new Vector2Int(x, y));
                if (GameManager.Instance.GetChunk(x, y).Biome.Id != BiomesID.NonBiom)
                {
                    isNonChunk = false;
                }
                regionSize++;

                queue.Enqueue((x + 1, y));
                queue.Enqueue((x - 1, y));
                queue.Enqueue((x, y + 1));
                queue.Enqueue((x, y - 1));
            }

            return (regionSize , isNonChunk);
        }
        catch (Exception e)
        {
            throw e;
        }
    }
    #endregion

    #endregion
}
