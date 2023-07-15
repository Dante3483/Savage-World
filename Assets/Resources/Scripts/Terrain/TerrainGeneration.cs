using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class TerrainGeneration
{
    #region Private fields
    private TerrainConfigurationSO _terrainConfiguration;
    private int _seed;
    private System.Random _randomVar;
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
        #region Phase 1 - Flat world generation
        var watch = System.Diagnostics.Stopwatch.StartNew();

        CreateFlatWorld();

        watch.Stop();
        Debug.Log($"Phase 1: {watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 1: {watch.Elapsed.TotalSeconds}\n";
        totalTime += watch.Elapsed.TotalSeconds;
        #endregion

        #region Phase 2 - Landscape generation
        watch.Restart();

        CreateLandscape();

        watch.Stop();
        Debug.Log($"Phase 2: {watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 2: {watch.Elapsed.TotalSeconds}\n";
        totalTime += watch.Elapsed.TotalSeconds;
        #endregion

        #region Phase 3 - Biomes generation
        watch.Restart();

        CreateBiomes();

        watch.Stop();
        Debug.Log($"Phase 3: {watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 3: {watch.Elapsed.TotalSeconds}\n";
        totalTime += watch.Elapsed.TotalSeconds;
        #endregion

        #region Phase 4 - Clusters generation
        watch.Restart();

        CreateClusters();

        watch.Stop();
        Debug.Log($"Phase 4: {watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 4: {watch.Elapsed.TotalSeconds}\n";
        totalTime += watch.Elapsed.TotalSeconds;
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
                Chunk chunk = GameManager.Instance.GetChunk(x, y);
                if (chunk.Biome.Id == BiomesID.NonBiom)
                {
                    chunk.Biome = biome;
                }
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
                    Chunk chunk = GameManager.Instance.GetChunk(x, y);
                    if (chunk.Biome.Id == BiomesID.NonBiom)
                    {
                        chunk.Biome = biome;
                    }
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
            CreateCluster(cluster);
        }
    }

    private void CreateCluster(ClusterSO cluster)
    {
        List<Thread> threads = new List<Thread>();
        byte i = 0;

        foreach (TerrainLevelSO level in TerrainConfiguration.Levels)
        {
            for (ushort y = level.StartY; y < level.EndY; y += TerrainConfiguration.ChunkSize)
            {
                threads.Add(new Thread(GenerateLine));
                threads[i].Start(new Tuple<TerrainLevelSO, ClusterSO, ushort>(level, cluster, y));
                i++;
            }
        }

        foreach (Thread thread in threads)
        {
            thread.Join();
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
            Chunk chunk = GameManager.Instance.GetChunk(x, TerrainConfiguration.Equator);
            if (chunk.Biome.Id == BiomesID.NonBiom)
            {
                chunk.Biome = biome;
            }
        }
    }

    private void GenerateLine(object obj)
    {
        Tuple<TerrainLevelSO, ClusterSO, ushort> data = (Tuple<TerrainLevelSO, ClusterSO, ushort>)obj;
        ClusterSO cluster = data.Item2;
        ClusterSO.ClusterData clusterData = data.Item2.GetClusterData(data.Item1);
        ushort startY = data.Item3;

        for (ushort x = 0; x < GameManager.Instance.CurrentTerrainWidth; x++)
        {
            for (ushort y = startY; y < startY + TerrainConfiguration.ChunkSize; y++)
            {
                if (!cluster.CompareBlock(GameManager.Instance.WorldData[x, y].BlockData))
                {
                    if (GenerateNoise(x, y, clusterData.Scale, clusterData.Amplitude) <= clusterData.Intensity)
                    {
                        Terrain.CreateBlock(x, y, data.Item2.Block);
                    }
                }
            }
        }
    }
    #endregion

    #endregion
}
