using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
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
    private List<Vector2Ushort> _surfaceCoords;
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
        float step = 100f / 12;

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

        #region Phase 6 - Special caves generation
        watch.Restart();

        CreateSpecialCaves();

        watch.Stop();
        Debug.Log($"Phase 6: {watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 6: {watch.Elapsed.TotalSeconds}\n";
        totalTime += watch.Elapsed.TotalSeconds;
        GameManager.Instance.LoadingValue += step;
        #endregion

        #region Phase 7 - Lakes generation
        watch.Restart();

        CreateLakes();

        watch.Stop();
        Debug.Log($"Phase 7: {watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 7: {watch.Elapsed.TotalSeconds}\n";
        totalTime += watch.Elapsed.TotalSeconds;
        GameManager.Instance.LoadingValue += step;
        #endregion

        #region Phase 8 - Oasises generation
        watch.Restart();

        CreateOasises();

        watch.Stop();
        Debug.Log($"Phase 8: {watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 8: {watch.Elapsed.TotalSeconds}\n";
        totalTime += watch.Elapsed.TotalSeconds;
        GameManager.Instance.LoadingValue += step;
        #endregion

        #region Phase 9 - Grass seeding
        watch.Restart();

        GrassSeeding();

        watch.Stop();
        Debug.Log($"Phase 9: {watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 9: {watch.Elapsed.TotalSeconds}\n";
        totalTime += watch.Elapsed.TotalSeconds;
        GameManager.Instance.LoadingValue += step;
        #endregion

        #region Phase 10 - Plants generation
        watch.Restart();

        CreatePlants();

        watch.Stop();
        Debug.Log($"Phase 10: {watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 10: {watch.Elapsed.TotalSeconds}\n";
        totalTime += watch.Elapsed.TotalSeconds;
        GameManager.Instance.LoadingValue += step;
        #endregion

        #region Phase 11 - Tree generation
        watch.Restart();

        CreateTrees();

        watch.Stop();
        Debug.Log($"Phase 11: {watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 11: {watch.Elapsed.TotalSeconds}\n";
        totalTime += watch.Elapsed.TotalSeconds;
        GameManager.Instance.LoadingValue += step;
        #endregion

        #region Phase 12 - Pickable items generation
        watch.Restart();

        CreatePickableItems();

        watch.Stop();
        Debug.Log($"Phase 12: {watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 12: {watch.Elapsed.TotalSeconds}\n";
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
        _surfaceCoords = new List<Vector2Ushort>();

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
                ushort y;
                for (y = startY; y <= height; y++)
                {
                    Terrain.CreateBlock(x, y, block);
                }
                _surfaceCoords.Add(new Vector2Ushort(x, (ushort)(y - 1)));

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
        CreateOcean(TerrainConfiguration.Biomes.Find(b => b.Id == BiomesID.Ocean));
        CreateDesert(TerrainConfiguration.Biomes.Find(b => b.Id == BiomesID.Desert));
        CreateSavannah(TerrainConfiguration.Biomes.Find(b => b.Id == BiomesID.Savannah));
        CreateMeadow(TerrainConfiguration.Biomes.Find(b => b.Id == BiomesID.Meadow));
        CreateForest(TerrainConfiguration.Biomes.Find(b => b.Id == BiomesID.Forest));
        CreateSwamp(TerrainConfiguration.Biomes.Find(b => b.Id == BiomesID.Swamp));
        CreateConiferousForest(TerrainConfiguration.Biomes.Find(b => b.Id == BiomesID.ConiferousForest));
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

    #region Phase 6
    private void CreateSpecialCaves()
    {
        BiomeSO savannah = TerrainConfiguration.Biomes.Find(b => b.Id == BiomesID.Savannah);
        BiomeSO coniferousForest = TerrainConfiguration.Biomes.Find(b => b.Id == BiomesID.ConiferousForest);
        TerrainLevelSO surfaceLevel = TerrainConfiguration.Levels.Find(l => l.Name == "Surface");
        short tunnelDirection = 0;
        short prevTunnelDirection = 0;
        int countOfRepeats = 0;

        for (ushort x = (ushort)(savannah.StartX + TerrainConfiguration.ChunkSize); x < coniferousForest.StartX; x += TerrainConfiguration.ChunkSize)
        {
            int chance = RandomVar.Next(0, 101);
            if (chance <= TerrainConfiguration.StarterCaveChance)
            {
                ushort startX = (ushort)RandomVar.Next(x + 5, x + TerrainConfiguration.ChunkSize - TerrainConfiguration.MaxStarterCaveLength - 5);
                ushort startY = (ushort)RandomVar.Next(surfaceLevel.StartY + TerrainConfiguration.ChunkSize + 5, TerrainConfiguration.Equator - TerrainConfiguration.MaxStarterCaveHeight - 10);
                tunnelDirection = (short)(RandomVar.Next(0, 2) == 0 ? -1 : 1);
                tunnelDirection = (short)(countOfRepeats == 2 ? tunnelDirection - (tunnelDirection * 2) : tunnelDirection);

                if (CreateStarterCave(startX, startY, tunnelDirection))
                {
                    x += TerrainConfiguration.ChunkSize;
                }
                if (tunnelDirection == prevTunnelDirection)
                {
                    countOfRepeats++;
                }
                else
                {
                    prevTunnelDirection = tunnelDirection;
                    countOfRepeats = 0;
                }
            }
        }
    }

    private bool CreateStarterCave(ushort startX, ushort startY, short tunnelDirection)
    {
        //Define length and height
        int length = RandomVar.Next(TerrainConfiguration.MinStarterCaveLength, TerrainConfiguration.MaxStarterCaveLength);
        int height = RandomVar.Next(TerrainConfiguration.MinStarterCaveHeight, TerrainConfiguration.MaxStarterCaveHeight);

        //Define list of coords and air block
        List<Vector2Ushort> coords = new List<Vector2Ushort>();
        List<Vector2Ushort> stoneCoords = new List<Vector2Ushort>();
        BlockSO airBlock = GameManager.Instance.ObjectsAtlass.Air;
        BlockSO stoneBlock = GameManager.Instance.ObjectsAtlass.Stone;

        //Create rectangle
        ushort y;
        ushort x;
        for (x = startX; x <= startX + length; x++)
        {
            for (y = startY; y <= startY + height; y++)
            {
                if (!GameManager.Instance.WorldData[x, y].CompareBlock(airBlock))
                {
                    coords.Add(new Vector2Ushort(x, y));
                }
                else
                {
                    return false;
                }
            }
        }

        int countOfRepeats = 0;
        //Add noise to the top
        y = (ushort)(startY + height + 1);
        for (x = startX; x <= startX + length; x++)
        {
            if (RandomVar.Next(0, 2) == 1 || countOfRepeats == 2)
            {
                coords.Add(new Vector2Ushort(x, y));
                countOfRepeats = 0;
            }
            else
            {
                countOfRepeats++;
            }
        }

        //Add noise to the bottom
        y = (ushort)(startY - 1);
        countOfRepeats = 0;
        for (x = startX; x <= startX + length; x++)
        {
            if (RandomVar.Next(0, 2) == 1 || countOfRepeats == 2)
            {
                stoneCoords.Add(new Vector2Ushort(x, (ushort)(y - 1)));
                coords.Add(new Vector2Ushort(x, y));
                countOfRepeats = 0;
            }
            else
            {
                stoneCoords.Add(new Vector2Ushort(x, (ushort)(y)));
                countOfRepeats++;
            }
        }

        //Add noise to the left
        countOfRepeats = 0;
        x = (ushort)(startX + length + 1);
        for (y = startY; y <= startY + height; y++)
        {
            if (RandomVar.Next(0, 2) == 1 || countOfRepeats == 2)
            {
                coords.Add(new Vector2Ushort(x, y));
                countOfRepeats = 0;
            }
            else
            {
                countOfRepeats++;
            }
        }

        //Add noise to the right
        x = (ushort)(startX - 1);
        for (y = startY; y <= startY + height; y++)
        {
            if (RandomVar.Next(0, 2) == 1 || countOfRepeats == 2)
            {
                coords.Add(new Vector2Ushort(x, y));
                countOfRepeats = 0;
            }
            else
            {
                countOfRepeats++;
            }
        }

        //Create tunnel
        if (tunnelDirection == -1)
        {
            CreateTunnel(tunnelDirection, startX, startY, ref coords, ref stoneCoords);
        }
        else
        {
            CreateTunnel(tunnelDirection, (ushort)(startX + length), startY, ref coords, ref stoneCoords);
        }


        //Fill terrain with air blocks
        foreach (Vector2Ushort coord in coords)
        {
            Terrain.CreateBlock(coord.x, coord.y, airBlock);
        }

        //Fill terrain with stone blocks
        foreach (Vector2Ushort coord in stoneCoords)
        {
            Terrain.CreateBlock(coord.x, coord.y, stoneBlock);
        }

        return true;
    }

    private void CreateTunnel(short direction, ushort startX, ushort startY, ref List<Vector2Ushort> coords, ref List<Vector2Ushort> stoneCoords)
    {
        short x = (short)startX;
        short y = (short)startY;
        int stepUp = 5;
        BlockSO airBlock = GameManager.Instance.ObjectsAtlass.Air;

        while (true)
        {
            for (int i = 0; i <= stepUp; i++)
            {
                coords.Add(new Vector2Ushort((ushort)x, (ushort)(y + i)));
            }

            if (RandomVar.Next(0, 2) == 1)
            {
                stoneCoords.Add(new Vector2Ushort((ushort)x, (ushort)(y - 1)));
            }

            x += direction;
            if (RandomVar.Next(0, 2) == 1)
            {
                y++;
            }
            if (GameManager.Instance.WorldData[x, y].CompareBlock(airBlock))
            {
                break;
            }
        }
    }
    #endregion

    #region Phase 7
    private void CreateLakes()
    {
        ushort startX = (ushort)(TerrainConfiguration.Biomes.Find(b => b.Id == BiomesID.Savannah).StartX + TerrainConfiguration.ChunkSize);
        ushort endX = (ushort)(TerrainConfiguration.Biomes.Find(b => b.Id == BiomesID.ConiferousForest).EndX - TerrainConfiguration.ChunkSize);
        bool isChance = false;

        for (int i = startX; i < endX; i += TerrainConfiguration.ChunkSize)
        {
            if (RandomVar.Next(0, 101) <= TerrainConfiguration.LakeChance || isChance)
            {
                for (int j = i; j < i + TerrainConfiguration.ChunkSize; j++)
                {
                    isChance = true;
                    if (CreateLake(_surfaceCoords[j].x, _surfaceCoords[j].y))
                    {
                        isChance = false;
                        i += TerrainConfiguration.ChunkSize * TerrainConfiguration.LakeDistanceInChunks;
                        break;
                    }
                }
            }
        }
    }

    private bool CreateLake(ushort startX, ushort startY)
    {
        List<Vector2Ushort> coords = new List<Vector2Ushort>();
        List<Vector2Ushort> emptyCoords = new List<Vector2Ushort>();
        BlockSO airBlock = GameManager.Instance.ObjectsAtlass.Air;
        BlockSO waterBlock = GameManager.Instance.ObjectsAtlass.Water;
        ushort lakeLength = (ushort)RandomVar.Next(TerrainConfiguration.MinLakeLength, TerrainConfiguration.MaxLakeLength);
        ushort lakeHeight = (ushort)RandomVar.Next(TerrainConfiguration.MinLakeHeight, TerrainConfiguration.MaxLakeHeight);
        ushort currentlakeHeight = 0;
        ushort startAdder = 0;
        ushort endAdder = 0;

        //Fill list with potential blocks
        while (currentlakeHeight != lakeHeight)
        {
            ushort i = startAdder;
            if (i > lakeLength - endAdder)
            {
                break;
            }
            for (; i < lakeLength - endAdder; i++)
            {
                coords.Add(new Vector2Ushort((ushort)(startX + i), (ushort)(startY - currentlakeHeight)));
            }
            startAdder += (ushort)RandomVar.Next(1, 3);
            endAdder += (ushort)RandomVar.Next(1, 3);
            currentlakeHeight++;
        }

        //Verify lake conditions
        foreach (Vector2Ushort coord in coords)
        {
            if (GameManager.Instance.WorldData[coord.x - 1, coord.y].CompareBlock(airBlock) ||
                GameManager.Instance.WorldData[coord.x + 1, coord.y].CompareBlock(airBlock))
            {
                return false;
            }
            for (int i = 0; i < 3; i++)
            {
                if (GameManager.Instance.WorldData[coord.x, coord.y - i].CompareBlock(airBlock))
                {
                    return false;
                }
            }
        }

        //Fill list to empty blocks
        for (int i = 0; i < lakeLength; i++)
        {
            for (int j = 1; ; j++)
            {
                if (GameManager.Instance.WorldData[coords[i].x, coords[i].y + j].CompareBlock(airBlock))
                {
                    break;
                }
                emptyCoords.Add(new Vector2Ushort(coords[i].x, (ushort)(coords[i].y + j)));
            }
        }

        //Smooth
        ushort smoothStartY = 1;
        for (ushort x = (ushort)(startX + lakeLength); ; x++)
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
                emptyCoords.Add(new Vector2Ushort(x, y));
            }

            byte chanceToMoveUp = (byte)RandomVar.Next(0, 3);
            if (chanceToMoveUp % 2 == 0)
            {
                smoothStartY++;
            }
        }

        //Create lake
        foreach (Vector2Ushort coord in coords)
        {
            Terrain.CreateBlock(coord.x, coord.y, waterBlock);
        }

        //Create air
        foreach (Vector2Ushort coord in emptyCoords)
        {
            Terrain.CreateBlock(coord.x, coord.y, airBlock);
        }

        return true;
    }
    #endregion

    #region Phase 8
    private void CreateOasises()
    {
        ushort startX = (ushort)(TerrainConfiguration.Biomes.Find(b => b.Id == BiomesID.Desert).StartX + TerrainConfiguration.ChunkSize);
        ushort endX = (ushort)(TerrainConfiguration.Biomes.Find(b => b.Id == BiomesID.Desert).EndX - TerrainConfiguration.ChunkSize);
        bool isChance = false;

        for (int i = startX; i < endX; i += TerrainConfiguration.ChunkSize)
        {
            if (RandomVar.Next(0, 101) <= TerrainConfiguration.OasisChance || isChance)
            {
                for (int j = i; j < i + TerrainConfiguration.ChunkSize; j++)
                {
                    isChance = true;
                    if (CreateOasis(_surfaceCoords[j].x, _surfaceCoords[j].y))
                    {
                        isChance = false;
                        i += TerrainConfiguration.ChunkSize * TerrainConfiguration.OasisDistanceInChunks;
                        break;
                    }
                }
            }
        }
    }

    private bool CreateOasis(ushort startX, ushort startY)
    {
        double Ellipse(int x, int a, int b)
        {
            return Math.Sqrt((1 - Math.Pow(x, 2) / Math.Pow(a, 2)) * Math.Pow(b, 2));
        }

        List<Vector2Ushort> coords = new List<Vector2Ushort>();
        List<Vector2Ushort> emptyCoords = new List<Vector2Ushort>();
        BlockSO airBlock = GameManager.Instance.ObjectsAtlass.Air;
        BlockSO waterBlock = GameManager.Instance.ObjectsAtlass.Water;
        int oasisLength = RandomVar.Next(TerrainConfiguration.MinOasisLength, TerrainConfiguration.MaxOasisLength);
        int oasisHeight = RandomVar.Next(TerrainConfiguration.MinOasisHeight, TerrainConfiguration.MaxOasisHeight);

        //Fill list with potential blocks
        for (int x = startX; x < startX + oasisLength; x++)
        {
            for (int y = startY; y > startY - oasisHeight; y--)
            {
                if (startY - y <= Ellipse(x - (startX + oasisLength / 2), oasisLength / 2, oasisHeight / 2))
                {
                    coords.Add(new Vector2Ushort((ushort)x, (ushort)y));
                }
            }
        }

        //Verify lake conditions
        foreach (Vector2Ushort coord in coords)
        {
            if (GameManager.Instance.WorldData[coord.x - 1, coord.y].CompareBlock(airBlock) ||
                GameManager.Instance.WorldData[coord.x + 1, coord.y].CompareBlock(airBlock))
            {
                return false;
            }
            for (int i = 0; i < 3; i++)
            {
                if (GameManager.Instance.WorldData[coord.x, coord.y - i].CompareBlock(airBlock))
                {
                    return false;
                }
            }
        }

        //Fill list to empty blocks
        for (int x = startX; x < startX + oasisLength; x++)
        {
            for (int j = 1; ; j++)
            {
                if (GameManager.Instance.WorldData[x, startY + j].CompareBlock(airBlock))
                {
                    break;
                }
                emptyCoords.Add(new Vector2Ushort((ushort)x, (ushort)(startY + j)));
            }
        }

        //Smooth
        ushort smoothStartY = 1;
        for (ushort x = (ushort)(startX + oasisLength); ; x++)
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
                emptyCoords.Add(new Vector2Ushort(x, y));
            }

            byte chanceToMoveUp = (byte)RandomVar.Next(0, 3);
            if (chanceToMoveUp % 2 == 0)
            {
                smoothStartY++;
            }
        }

        //Create lake
        foreach (Vector2Ushort coord in coords)
        {
            Terrain.CreateBlock(coord.x, coord.y, waterBlock);
        }

        //Create air
        foreach (Vector2Ushort coord in emptyCoords)
        {
            Terrain.CreateBlock(coord.x, coord.y, airBlock);
        }

        return true;
    }
    #endregion

    #region Phase 9
    private void GrassSeeding()
    {
        TerrainLevelSO surface = TerrainConfiguration.Levels.Find(l => l.Name == "Surface");
        BlockSO dirtBlock = GameManager.Instance.ObjectsAtlass.Dirt;
        BlockSO airBlock = GameManager.Instance.ObjectsAtlass.Air;
        BiomesID currentBiomeId;
        _surfaceCoords.Clear();

        for (int x = 0; x < GameManager.Instance.CurrentTerrainWidth; x++)
        {
            for (int y = TerrainConfiguration.Equator; y < surface.EndY; y++)
            {
                currentBiomeId = GameManager.Instance.GetChunk(x, y).Biome.Id;
                if (GameManager.Instance.WorldData[x, y + 1].CompareBlock(airBlock))
                {
                    _surfaceCoords.Add(new Vector2Ushort((ushort)x, (ushort)y));
                    if (GameManager.Instance.WorldData[x, y].CompareBlock(dirtBlock))
                    {
                        Terrain.CreateBlock((ushort)x, (ushort)y, GameManager.Instance.ObjectsAtlass.GetGrassByBiome(currentBiomeId));
                    }
                }
            }
        }
    }
    #endregion

    #region Phase 10
    private void CreatePlants()
    {
        //Create surface plants
        TerrainLevelSO surface = TerrainConfiguration.Levels.Find(l => l.Name == "Surface");
        BlockSO airBlock = GameManager.Instance.ObjectsAtlass.Air;
        List<List<PlantSO>> allPlants = new List<List<PlantSO>>()
        {
            GameManager.Instance.ObjectsAtlass.GetAllBiomePlants(BiomesID.NonBiom),
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
        ushort startX;
        ushort endX;

        foreach (List<PlantSO> plants in allPlants)
        {
            foreach (PlantSO plant in plants)
            {
                if (plant.BiomeId == BiomesID.NonBiom)
                {
                    startX = 0;
                    endX = (ushort)(GameManager.Instance.CurrentTerrainWidth - 1);
                }
                else
                {
                    currentBiome = TerrainConfiguration.Biomes.Find(b => b.Id == plant.BiomeId);
                    startX = currentBiome.StartX;
                    endX = currentBiome.EndX;
                }

                for (ushort x = startX; x <= endX; x++)
                {
                    for (ushort y = surface.StartY; y < surface.EndY; y++)
                    {
                        chance = RandomVar.Next(0, 101);
                        if (plant.AllowedToSpawnOn.Contains(GameManager.Instance.WorldData[x, y].BlockData) && chance <= plant.ChanceToSpawn)
                        {
                            if (plant.IsBottomSpawn && GameManager.Instance.WorldData[x, y + 1].CompareBlock(airBlock))
                            {
                                Terrain.CreateBlock(x, (ushort)(y + 1), plant);
                            }
                            else if (plant.IsTopSpawn && GameManager.Instance.WorldData[x, y - 1].CompareBlock(airBlock))
                            {
                                Terrain.CreateBlock(x, (ushort)(y - 1), plant);
                            }
                        }
                    }
                }
            }
        }
    }
    #endregion

    #region Phase 11
    private void CreateTrees()
    {
        //Create surface trees
        TerrainLevelSO surface = TerrainConfiguration.Levels.Find(l => l.Name == "Surface");
        BlockSO airBlock = GameManager.Instance.ObjectsAtlass.Air;
        Dictionary<BiomesID, List<Tree>> allTrees = null;
        List<Vector3> coords = new List<Vector3>();
        ThreadsManager.Instance.AddAction(() =>
        {
            allTrees = new Dictionary<BiomesID, List<Tree>>()
            {
                //{ BiomesID.NonBiom, GameManager.Instance.ObjectsAtlass.GetAllBiomeTrees(BiomesID.NonBiom) },
                //{ BiomesID.Ocean, GameManager.Instance.ObjectsAtlass.GetAllBiomeTrees(BiomesID.Ocean) },
                { BiomesID.Desert, GameManager.Instance.ObjectsAtlass.GetAllBiomeTrees(BiomesID.Desert) },
                { BiomesID.Savannah, GameManager.Instance.ObjectsAtlass.GetAllBiomeTrees(BiomesID.Savannah) },
                { BiomesID.Meadow, GameManager.Instance.ObjectsAtlass.GetAllBiomeTrees(BiomesID.Meadow) },
                { BiomesID.Forest, GameManager.Instance.ObjectsAtlass.GetAllBiomeTrees(BiomesID.Forest) },
                { BiomesID.Swamp, GameManager.Instance.ObjectsAtlass.GetAllBiomeTrees(BiomesID.Swamp) },
                { BiomesID.ConiferousForest, GameManager.Instance.ObjectsAtlass.GetAllBiomeTrees(BiomesID.ConiferousForest) },
            };
        });
        BiomeSO currentBiome;
        int chance;
        ushort startX;
        ushort endX;
        GameObject treesSection = GameManager.Instance.Terrain.Trees;

        foreach (var trees in allTrees)
        {
            foreach (Tree tree in trees.Value)
            {
                if (trees.Key == BiomesID.NonBiom)
                {
                    startX = 0;
                    endX = (ushort)(GameManager.Instance.CurrentTerrainWidth - 1);
                }
                else
                {
                    currentBiome = TerrainConfiguration.Biomes.Find(b => b.Id == trees.Key);
                    startX = currentBiome.StartX;
                    endX = currentBiome.EndX;
                }

                for (ushort x = startX; x <= endX - tree.Width; x++)
                {
                    for (ushort y = surface.StartY; y < surface.EndY; y++)
                    {
                        bool isValidPlace = true;
                        for (int i = 0; i < tree.WidthToSpawn; i++)
                        {
                            if (!tree.AllowedToSpawnOn.Contains(GameManager.Instance.WorldData[x + i, y].BlockData))
                            {
                                isValidPlace = false;
                                break;
                            }
                            if (!GameManager.Instance.WorldData[x, y + 1].IsEmptyWithPlant())
                            {
                                isValidPlace = false;
                                break;
                            }
                        }
                        if (isValidPlace)
                        {
                            chance = RandomVar.Next(0, 101);
                            if (chance <= tree.ChanceToSpawn)
                            {
                                if (CreateTree(x, (ushort)(y + 1), tree, ref coords))
                                {
                                    x += (ushort)(tree.Width + tree.DistanceEachOthers);
                                }
                            }
                        }
                    }
                }
                ThreadsManager.Instance.AddAction(() =>
                {
                    foreach (Vector3 coord in coords)
                    {
                        GameObject treeGameObject = GameObject.Instantiate(tree.gameObject, coord, Quaternion.identity, treesSection.transform);
                        treeGameObject.name = tree.gameObject.name;
                    }
                });
                coords.Clear();
            }
        }
    }

    private bool CreateTree(ushort x, ushort y, Tree tree, ref List<Vector3> coords)
    {
        foreach (Vector3 vector in tree.TrunkBlocks)
        {
            if (!GameManager.Instance.WorldData[x + (int)(vector.x - tree.Start.x), y].IsEmptyWithPlant())
            {
                return false;
            }
        }
        foreach (Vector3 vector in tree.TreeBlocks)
        {
            if (!GameManager.Instance.WorldData[x + (int)(vector.x - tree.Start.x), y].IsEmptyWithPlant())
            {
                return false;
            }
        }
        coords.Add(new Vector3(x - tree.Start.x + tree.Offset.x, y));
        return true;
    }
    #endregion

    #region Phase 12
    private void CreatePickableItems()
    {
        //Create surface pickable items
        TerrainLevelSO surface = TerrainConfiguration.Levels.Find(l => l.Name == "Surface");
        BlockSO airBlock = GameManager.Instance.ObjectsAtlass.Air;
        Dictionary<BiomesID, List<PickableItem>> allPickableItems = null;
        List<Vector3> coords = new List<Vector3>();
        ThreadsManager.Instance.AddAction(() =>
        {
            allPickableItems = new Dictionary<BiomesID, List<PickableItem>>()
            {
                //{ BiomesID.NonBiom, GameManager.Instance.ObjectsAtlass.GetAllBiomePickableItems(BiomesID.NonBiom) },
                //{ BiomesID.Ocean, GameManager.Instance.ObjectsAtlass.GetAllBiomePickableItems(BiomesID.Ocean) },
                { BiomesID.Desert, GameManager.Instance.ObjectsAtlass.GetAllBiomePickableItems(BiomesID.Desert) },
                { BiomesID.Savannah, GameManager.Instance.ObjectsAtlass.GetAllBiomePickableItems(BiomesID.Savannah) },
                { BiomesID.Meadow, GameManager.Instance.ObjectsAtlass.GetAllBiomePickableItems(BiomesID.Meadow) },
                { BiomesID.Forest, GameManager.Instance.ObjectsAtlass.GetAllBiomePickableItems(BiomesID.Forest) },
                { BiomesID.Swamp, GameManager.Instance.ObjectsAtlass.GetAllBiomePickableItems(BiomesID.Swamp) },
                { BiomesID.ConiferousForest, GameManager.Instance.ObjectsAtlass.GetAllBiomePickableItems(BiomesID.ConiferousForest) },
            };
        });
        BiomeSO currentBiome;
        int chance;
        ushort startX;
        ushort endX;
        GameObject pickableItemsSection = GameManager.Instance.Terrain.PickableItems;

        foreach (var pickableItems in allPickableItems)
        {
            foreach (PickableItem pickableItem in pickableItems.Value)
            {
                if (pickableItems.Key == BiomesID.NonBiom)
                {
                    startX = 0;
                    endX = (ushort)(GameManager.Instance.CurrentTerrainWidth - 1);
                }
                else
                {
                    currentBiome = TerrainConfiguration.Biomes.Find(b => b.Id == pickableItems.Key);
                    startX = currentBiome.StartX;
                    endX = currentBiome.EndX;
                }

                for (ushort x = startX; x <= endX; x++)
                {
                    for (ushort y = surface.StartY; y < surface.EndY; y++)
                    {
                        if (GameManager.Instance.WorldData[x, y].IsSolid() &&
                            GameManager.Instance.WorldData[x, y + 1].IsEmpty())
                        {
                            chance = RandomVar.Next(0, 101);
                            if (chance <= pickableItem.ChanceToSpawn)
                            {
                                coords.Add(new Vector3(x, y + 1));
                            }
                        }
                    }
                }
                ThreadsManager.Instance.AddAction(() =>
                {
                    foreach (Vector3 coord in coords)
                    {
                        GameObject pickableItemGameObject = GameObject.Instantiate(pickableItem.gameObject, coord, Quaternion.identity, pickableItemsSection.transform);
                        pickableItemGameObject.name = pickableItem.gameObject.name;
                    }
                });
                coords.Clear();
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

            return (regionSize, isNonChunk);
        }
        catch (Exception e)
        {
            throw e;
        }
    }
    #endregion

    #endregion
}
