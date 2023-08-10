using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public class TerrainGeneration
{
    #region Private fields
    private TerrainConfigurationSO _terrainConfiguration;
    private System.Random _randomVar;
    private System.Diagnostics.Stopwatch _watch;
    private int _seed;
    private int _currentTerrainWidth;
    private int _currentTerrainHeight;
    private byte[,] _visitedCaveMap;
    private BlockSO _dirtBlock;
    private BlockSO _airBlock;
    private BlockSO _waterBlock;
    private BlockSO _sandBlock;
    private BlockSO _stoneBlock;
    private BlockSO _stoneBG;
    private BlockSO _dirtBG;
    private BlockSO _airBG;
    private BiomeSO _nonBiom;
    private BiomeSO _ocean;
    private BiomeSO _desert;
    private BiomeSO _savannah;
    private BiomeSO _meadow;
    private BiomeSO _forest;
    private BiomeSO _swamp;
    private BiomeSO _coniferousForest;
    private TerrainLevelSO _surfaceLevel;
    private float _step;
    private float _maxNoiseHeight;
    private float _minNoiseHeight;
    private float[,] _caveMap;
    private double _totalTime;
    private object _lockObject;
    private List<Vector2Ushort> _surfaceCoords;
    private WorldCellData[,] _worldData;
    private List<Thread> _threads;
    private Terrain _terrain;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods

    #region General
    public TerrainGeneration(int seed, ref WorldCellData[,] worldData, Terrain terrain)
    {
        _terrainConfiguration = GameManager.Instance.TerrainConfiguration;
        _randomVar = GameManager.Instance.RandomVar;
        _seed = seed;
        _currentTerrainWidth = GameManager.Instance.CurrentTerrainWidth;
        _currentTerrainHeight = GameManager.Instance.CurrentTerrainHeight;
        _dirtBlock = GameManager.Instance.ObjectsAtlass.Dirt;
        _airBlock = GameManager.Instance.ObjectsAtlass.Air;
        _waterBlock = GameManager.Instance.ObjectsAtlass.Water;
        _sandBlock = GameManager.Instance.ObjectsAtlass.Sand;
        _stoneBlock = GameManager.Instance.ObjectsAtlass.Stone;
        _airBG = GameManager.Instance.ObjectsAtlass.AirBG;
        _dirtBG = GameManager.Instance.ObjectsAtlass.DirtBG;
        _stoneBG = GameManager.Instance.ObjectsAtlass.StoneBG;
        _nonBiom = _terrainConfiguration.Biomes.Find(b => b.Id == BiomesID.NonBiom);
        _ocean = _terrainConfiguration.Biomes.Find(b => b.Id == BiomesID.Ocean);
        _desert = _terrainConfiguration.Biomes.Find(b => b.Id == BiomesID.Desert);
        _savannah = _terrainConfiguration.Biomes.Find(b => b.Id == BiomesID.Savannah);
        _meadow = _terrainConfiguration.Biomes.Find(b => b.Id == BiomesID.Meadow);
        _forest = _terrainConfiguration.Biomes.Find(b => b.Id == BiomesID.Forest);
        _swamp = _terrainConfiguration.Biomes.Find(b => b.Id == BiomesID.Swamp);
        _coniferousForest = _terrainConfiguration.Biomes.Find(b => b.Id == BiomesID.ConiferousForest);
        _surfaceLevel = _terrainConfiguration.Levels.Find(l => l.Name == "Surface");
        _maxNoiseHeight = float.MinValue;
        _minNoiseHeight = float.MaxValue;
        _lockObject = new object();
        _surfaceCoords = new List<Vector2Ushort>();
        _worldData = worldData;
        _threads = new List<Thread>();
        _terrain = terrain;
    }

    public void StartTerrainGeneration()
    {
        _totalTime = 0f;
        _step = 100f / 14;

        #region Phase 1 - Flat world generation
        _watch = System.Diagnostics.Stopwatch.StartNew();

        if (_terrainConfiguration.Phase1)
        {
            //CreateFlatWorld();
            CreateFlatWorldParallel();
        }

        _watch.Stop();
        Debug.Log($"Phase 1: {_watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 1: {_watch.Elapsed.TotalSeconds}\n";
        _totalTime += _watch.Elapsed.TotalSeconds;
        GameManager.Instance.LoadingValue += _step;
        #endregion

        #region Phase 2 - Landscape generation
        _watch.Restart();

        if (_terrainConfiguration.Phase2)
        {
            CreateLandscape();
        }

        _watch.Stop();
        Debug.Log($"Phase 2: {_watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 2: {_watch.Elapsed.TotalSeconds}\n";
        _totalTime += _watch.Elapsed.TotalSeconds;
        GameManager.Instance.LoadingValue += _step;
        #endregion

        #region Phase 3 - Biomes generation
        _watch.Restart();

        if (_terrainConfiguration.Phase3)
        {
            CreateBiomes();
        }

        _watch.Stop();
        Debug.Log($"Phase 3: {_watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 3: {_watch.Elapsed.TotalSeconds}\n";
        _totalTime += _watch.Elapsed.TotalSeconds;
        GameManager.Instance.LoadingValue += _step;
        #endregion

        #region Phase 4 - Clusters generation
        _watch.Restart();

        if (_terrainConfiguration.Phase4)
        {
            //CreateClusters();
            CreateClustersParallel();
        }

        _watch.Stop();
        Debug.Log($"Phase 4: {_watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 4: {_watch.Elapsed.TotalSeconds}\n";
        _totalTime += _watch.Elapsed.TotalSeconds;
        GameManager.Instance.LoadingValue += _step;
        #endregion

        #region Phase 5 - Caves generation
        _watch.Restart();

        if (_terrainConfiguration.Phase5)
        {
            //CreateCaves();
            CreateCavesParallel();
        }

        _watch.Stop();
        Debug.Log($"Phase 5: {_watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 5: {_watch.Elapsed.TotalSeconds}\n";
        _totalTime += _watch.Elapsed.TotalSeconds;
        GameManager.Instance.LoadingValue += _step;
        #endregion

        #region Phase 6 - Special caves generation
        _watch.Restart();

        if (_terrainConfiguration.Phase6)
        {
            CreateSpecialCaves();
        }

        _watch.Stop();
        Debug.Log($"Phase 6: {_watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 6: {_watch.Elapsed.TotalSeconds}\n";
        _totalTime += _watch.Elapsed.TotalSeconds;
        GameManager.Instance.LoadingValue += _step;
        #endregion

        #region Phase 7 - Lakes generation
        _watch.Restart();

        if (_terrainConfiguration.Phase7)
        {
            CreateLakes();
        }

        _watch.Stop();
        Debug.Log($"Phase 7: {_watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 7: {_watch.Elapsed.TotalSeconds}\n";
        _totalTime += _watch.Elapsed.TotalSeconds;
        GameManager.Instance.LoadingValue += _step;
        #endregion

        #region Phase 8 - Oasises generation
        _watch.Restart();

        if (_terrainConfiguration.Phase8)
        {
            CreateOasises();
        }

        _watch.Stop();
        Debug.Log($"Phase 8: {_watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 8: {_watch.Elapsed.TotalSeconds}\n";
        _totalTime += _watch.Elapsed.TotalSeconds;
        GameManager.Instance.LoadingValue += _step;
        #endregion

        #region Phase 9 - Grass seeding
        _watch.Restart();

        if (_terrainConfiguration.Phase9)
        {
            GrassSeeding();
        }

        _watch.Stop();
        Debug.Log($"Phase 9: {_watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 9: {_watch.Elapsed.TotalSeconds}\n";
        _totalTime += _watch.Elapsed.TotalSeconds;
        GameManager.Instance.LoadingValue += _step;
        #endregion

        #region Phase 10 - Plants generation
        _watch.Restart();

        if (_terrainConfiguration.Phase10)
        {
            CreatePlants();
        }

        _watch.Stop();
        Debug.Log($"Phase 10: {_watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 10: {_watch.Elapsed.TotalSeconds}\n";
        _totalTime += _watch.Elapsed.TotalSeconds;
        GameManager.Instance.LoadingValue += _step;
        #endregion

        #region Phase 11 - Tree generation
        _watch.Restart();

        if (_terrainConfiguration.Phase11)
        {
            CreateTrees();
        }

        _watch.Stop();
        Debug.Log($"Phase 11: {_watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 11: {_watch.Elapsed.TotalSeconds}\n";
        _totalTime += _watch.Elapsed.TotalSeconds;
        GameManager.Instance.LoadingValue += _step;
        #endregion

        #region Phase 12 - Pickable items generation
        _watch.Restart();

        if (_terrainConfiguration.Phase12)
        {
            CreatePickableItems();
        }

        _watch.Stop();
        Debug.Log($"Phase 12: {_watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 12: {_watch.Elapsed.TotalSeconds}\n";
        _totalTime += _watch.Elapsed.TotalSeconds;
        GameManager.Instance.LoadingValue += _step;
        #endregion

        #region Phase 13 - Set random tiles
        _watch.Restart();

        if (_terrainConfiguration.Phase13)
        {
            SetRandomTiles();
        }

        _watch.Stop();
        Debug.Log($"Phase 13: {_watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 13: {_watch.Elapsed.TotalSeconds}\n";
        _totalTime += _watch.Elapsed.TotalSeconds;
        GameManager.Instance.LoadingValue += _step;
        #endregion

        #region Phase 14 - Pre-block processing
        _watch.Restart();

        if (_terrainConfiguration.Phase14)
        {
            PreLaunchBlocksUpdate();
        }

        _watch.Stop();
        Debug.Log($"Phase 14: {_watch.Elapsed.TotalSeconds}");
        GameManager.Instance.GeneralInfo += $"Phase 14: {_watch.Elapsed.TotalSeconds}\n";
        _totalTime += _watch.Elapsed.TotalSeconds;
        GameManager.Instance.LoadingValue += _step;
        #endregion

        Debug.Log($"Total time: {_totalTime}");
        GameManager.Instance.GeneralInfo += $"Total time: {_totalTime}\n";
        GameManager.Instance.IsGameSession = true;
    }
    #endregion

    #region Phases

    #region Phase 1
    private void CreateFlatWorld()
    {
        ushort x;
        ushort y;
        TerrainLevelSO undergroundLevel = _terrainConfiguration.Levels.Find(l => l.Name == "Underground");
        TerrainLevelSO preUndergroundLevel = _terrainConfiguration.Levels.Find(l => l.Name == "PreUnderground");

        for (x = 0; x < _currentTerrainWidth; x++)
        {
            for (y = 0; y <= _terrainConfiguration.Equator; y++)
            {
                _terrain.CreateBlock(x, y, _dirtBlock);
                if (y >= undergroundLevel.StartY && y <= undergroundLevel.EndY)
                {
                    _terrain.CreateBackground(x, y, undergroundLevel.DefaultBackground);
                }

                if (y >= preUndergroundLevel.StartY && y <= preUndergroundLevel.EndY)
                {
                    _terrain.CreateBackground(x, y, preUndergroundLevel.DefaultBackground);
                }

                if (y >= _surfaceLevel.StartY && y <= _surfaceLevel.EndY)
                {
                    _terrain.CreateBackground(x, y, _surfaceLevel.DefaultBackground);
                }
            }
            for (y = (ushort)(_terrainConfiguration.Equator + 1); y < GameManager.Instance.CurrentTerrainHeight; y++)
            {
                _terrain.CreateBlock(x, y, _airBlock);
                _terrain.CreateBackground(x, y, _airBG);
            }
        }
    }

    private void CreateFlatWorldParallel()
    {
        TerrainLevelSO undergroundLevel = _terrainConfiguration.Levels.Find(l => l.Name == "Underground");
        TerrainLevelSO preUndergroundLevel = _terrainConfiguration.Levels.Find(l => l.Name == "PreUnderground");

        Parallel.For(0, _currentTerrainWidth, (index) =>
        {
            ushort x = (ushort)index;
            ushort y;
            for (y = 0; y <= _terrainConfiguration.Equator; y++)
            {
                _terrain.CreateBlock(x, y, _dirtBlock);
                if (y >= undergroundLevel.StartY && y <= undergroundLevel.EndY)
                {
                    _terrain.CreateBackground(x, y, undergroundLevel.DefaultBackground);
                }

                if (y >= preUndergroundLevel.StartY && y <= preUndergroundLevel.EndY)
                {
                    _terrain.CreateBackground(x, y, preUndergroundLevel.DefaultBackground);
                }

                if (y >= _surfaceLevel.StartY && y <= _surfaceLevel.EndY)
                {
                    _terrain.CreateBackground(x, y, _surfaceLevel.DefaultBackground);
                }
            }
            for (y = (ushort)(_terrainConfiguration.Equator + 1); y < _currentTerrainHeight; y++)
            {
                _terrain.CreateBlock(x, y, _airBlock);
                _terrain.CreateBackground(x, y, _airBG);
            }
        });
    }
    #endregion

    #region Phase 2
    private void CreateLandscape()
    {
        ushort startY = (ushort)(_terrainConfiguration.Equator + 1);
        ushort x;
        ushort y;
        short prevHeight = -1;
        short firstHeight;
        short sign;
        short heightAdder;
        short height = 0;
        short dif;
        Vector2Ushort vector = new Vector2Ushort();

        foreach (BiomeSO biome in _terrainConfiguration.Biomes)
        {
            //Calculate difference of height between two biomes
            firstHeight = (short)(startY + Mathf.PerlinNoise((biome.StartX + _seed) / biome.MountainCompression, _seed / biome.MountainCompression) * biome.MountainHeight);
            dif = (short)(prevHeight != -1 ? (short)(prevHeight - firstHeight) : 0);
            sign = (short)(dif < 0 ? 1 : -1);
            heightAdder = dif;

            //Create landscape
            for (x = biome.StartX; x <= biome.EndX; x++)
            {
                //Calculate maximum height
                height = (short)(Mathf.PerlinNoise((x + _seed) / biome.MountainCompression, _seed / biome.MountainCompression) * biome.MountainHeight);
                height += (short)(startY + heightAdder);
                for (y = startY; y <= height; y++)
                {
                    _terrain.CreateBlock(x, y, _dirtBlock);
                }
                vector.x = x;
                vector.y = (ushort)(y - 1);
                _surfaceCoords.Add(vector);

                //Change diference
                if (heightAdder != 0)
                {
                    heightAdder += (short)(sign * _randomVar.Next(0, 2));
                }
            }
            prevHeight = height;
        }
    }
    #endregion

    #region Phase 3
    private void CreateBiomes()
    {
        CreateOcean(_ocean);
        CreateDesert(_desert);
        CreateSavannah(_savannah);
        CreateMeadow(_meadow);
        CreateForest(_forest);
        CreateSwamp(_swamp);
        CreateConiferousForest(_coniferousForest);
    }

    //Phase 3.1
    private void CreateOcean(BiomeSO biome)
    {
        ushort startX = (ushort)(biome.EndX - 50);
        ushort startY = _terrainConfiguration.Equator;
        ushort downHeight = 0;
        byte maxLength = 6;
        byte currentLength = 0;
        byte chanceToMoveDown;
        byte chanceToMoveUp;
        byte waterId = (byte)_waterBlock.GetId();
        ushort x;
        ushort y;
        //Hole generation
        for (x = startX; x > 5; x--)
        {
            //Clear dirt above ocean
            for (y = startY; y <= startY + biome.MountainHeight; y++)
            {
                _terrain.CreateBlock(x, y, _airBlock);
            }

            //Create hole
            for (y = startY; y >= startY - downHeight; y--)
            {
                _terrain.CreateBlock(x, y, _airBlock);
                _terrain.CreateLiquidBlock(x, y, waterId);
                GameManager.Instance.SetChunkBiome(x, y, biome);
            }

            chanceToMoveDown = (byte)_randomVar.Next(0, 6);
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
        _terrainConfiguration.DeepOceanY = (ushort)(startY - downHeight);

        //Smooth beach and hole
        ushort smoothStartY = 1;
        for (x = (ushort)(startX + 1); ; x++)
        {
            if (_worldData[x, startY + smoothStartY].CompareBlock(_airBlock))
            {
                break;
            }

            for (y = (ushort)(startY + smoothStartY); ; y++)
            {
                if (_worldData[x, y].CompareBlock(_airBlock))
                {
                    break;
                }
                _terrain.CreateBlock(x, y, _airBlock);
            }

            chanceToMoveUp = (byte)_randomVar.Next(0, 3);
            if (chanceToMoveUp % 2 == 0)
            {
                smoothStartY++;
            }
        }
    }

    //Phase 3.2
    private void CreateDesert(BiomeSO biome)
    {
        ushort startX = biome.EndX;
        ushort startY = (ushort)(_terrainConfiguration.Equator + biome.MountainHeight);
        ushort additionalHeight = 20;
        ushort x;
        ushort y;

        //Replace dirt with sand (inclusive ocean biome)
        for (x = startX; x > 5; x--)
        {
            for (y = startY; y > _terrainConfiguration.DeepOceanY - additionalHeight; y--)
            {
                if (_worldData[x, y].CompareBlock(_dirtBlock))
                {
                    _terrain.CreateBlock(x, y, _sandBlock);
                    GameManager.Instance.SetChunkBiome(x, y, biome);
                }
            }
        }

        //Pulverize
        ushort minLength = 10;
        ushort maxLength = 21;
        ushort lengthOfPulverizing;
        ushort additionalHeightPulverize = (ushort)(_randomVar.Next(10, 21) + additionalHeight);
        byte chanceToPulverize;

        //Vertical
        for (y = startY; y > _terrainConfiguration.DeepOceanY - additionalHeightPulverize; y--)
        {
            lengthOfPulverizing = (ushort)_randomVar.Next(minLength, maxLength);
            for (x = startX; x > startX - lengthOfPulverizing; x--)
            {
                chanceToPulverize = (byte)_randomVar.Next(0, 6);
                if (_worldData[x, y].CompareBlock(_sandBlock) &&
                    chanceToPulverize % 5 == 0)
                {
                    _terrain.CreateBlock(x, y, _dirtBlock);
                }
            }

            for (x = startX; x < startX + lengthOfPulverizing; x++)
            {
                chanceToPulverize = (byte)_randomVar.Next(0, 6);
                if (_worldData[x, y].CompareBlock(_dirtBlock) &&
                    chanceToPulverize % 5 == 0)
                {
                    _terrain.CreateBlock(x, y, _sandBlock);
                }
            }
        }

        //Horizontal
        startY = (ushort)(_terrainConfiguration.DeepOceanY - additionalHeight);
        for (x = startX; x > 5; x--)
        {
            lengthOfPulverizing = (ushort)_randomVar.Next(minLength, maxLength);
            for (y = startY; y > startY - lengthOfPulverizing; y--)
            {
                chanceToPulverize = (byte)_randomVar.Next(0, 6);
                if (_worldData[x, y].CompareBlock(_dirtBlock) &&
                    chanceToPulverize % 5 == 0)
                {
                    _terrain.CreateBlock(x, y, _sandBlock);
                }
            }

            for (y = startY; y < startY + lengthOfPulverizing; y++)
            {
                chanceToPulverize = (byte)_randomVar.Next(0, 6);
                if (_worldData[x, y].CompareBlock(_sandBlock) &&
                    chanceToPulverize % 5 == 0)
                {
                    _terrain.CreateBlock(x, y, _dirtBlock);
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
        foreach (ClusterSO cluster in _terrainConfiguration.Clusters)
        {
            CreateCluster(cluster, _randomVar.Next(0, 1000000));
        }
    }

    private void CreateCluster(ClusterSO cluster, int additionalSeed)
    {
        _threads.Clear();
        byte i = 0;
        ushort y;

        foreach (TerrainLevelSO level in _terrainConfiguration.Levels)
        {
            for (y = level.StartY; y < level.EndY; y += _terrainConfiguration.ChunkSize)
            {
                _threads.Add(new Thread(GenerateClusterLine));
                _threads[i].Start(new Tuple<TerrainLevelSO, ClusterSO, ushort, int>(level, cluster, y, additionalSeed));
                i++;
            }
        }

        foreach (Thread thread in _threads)
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
        ushort x;
        ushort y;

        for (x = 0; x < _currentTerrainWidth; x++)
        {
            for (y = startY; y < startY + _terrainConfiguration.ChunkSize; y++)
            {
                if (!cluster.CompareForbiddenBlock(_worldData[x, y].BlockData))
                {
                    if (GenerateNoise(x, y, clusterData.Scale, clusterData.Amplitude, additionalSeed) >= clusterData.Intensity)
                    {
                        _terrain.CreateBlock(x, y, data.Item2.Block);
                    }
                }
            }
        }

        data = null;
        cluster = null;
    }

    private void CreateClustersParallel()
    {
        foreach (ClusterSO cluster in _terrainConfiguration.Clusters)
        {
            CreateClusterParallel(cluster, _randomVar.Next(0, 1000000));
        }
    }

    private void CreateClusterParallel(ClusterSO cluster, int additionalSeed)
    {
        foreach (TerrainLevelSO level in _terrainConfiguration.Levels)
        {
            if (!cluster.ContainsLevel(level))
            {
                continue;
            }
            ClusterSO.ClusterData clusterData = cluster.GetClusterData(level);
            Parallel.For(0, _currentTerrainWidth, (index) =>
            {
                ushort x = (ushort)index;
                for (ushort y = level.StartY; y < level.EndY; y++)
                {
                    if (!cluster.CompareForbiddenBlock(_worldData[x, y].BlockData))
                    {
                        if (GenerateNoise(x, y, clusterData.Scale, clusterData.Amplitude, additionalSeed) >= clusterData.Intensity)
                        {
                            _terrain.CreateBlock(x, y, cluster.Block);
                        }
                    }
                }
            });
        }
    }
    #endregion

    #region Phase 5
    private void CreateCaves()
    {
        _threads.Clear();
        _caveMap = new float[_currentTerrainWidth, _currentTerrainHeight];
        _visitedCaveMap = new byte[_currentTerrainWidth, _currentTerrainHeight];
        List<Vector2Int> caveCoords = new List<Vector2Int>();
        short i = 0;
        ushort x;
        ushort y;

        //Create noise map
        for (y = 0; y < _surfaceLevel.EndY; y += _terrainConfiguration.ChunkSize)
        {
            _threads.Add(new Thread(CreateCave));
            _threads[i].Start(y);
            i++;
        }

        //Wait until all thread done
        foreach (Thread thread in _threads)
        {
            thread.Join();
        }

        //Lerp noise
        for (x = 0; x < _currentTerrainWidth; x++)
        {
            for (y = 0; y < _currentTerrainHeight; y++)
            {
                _caveMap[x, y] = Mathf.InverseLerp(_minNoiseHeight, _maxNoiseHeight, _caveMap[x, y]);
            }
        }

        int count;
        bool isNonChunk;
        //Create cave in conditions
        for (x = 0; x < _currentTerrainWidth; x++)
        {
            for (y = 0; y < _currentTerrainHeight; y++)
            {
                if (_caveMap[x, y] <= _terrainConfiguration.Intensity && _visitedCaveMap[x, y] == 0)
                {
                    (count, isNonChunk) = FloodFill(x, y, _caveMap, ref caveCoords);
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
                            _terrain.CreateBlock((ushort)v.x, (ushort)v.y, _airBlock);
                        }
                    }
                    caveCoords.Clear();
                }
            }
        }

        caveCoords = null;
    }

    private void CreateCave(object obj)
    {
        ushort startX = 0;
        ushort startY = (ushort)obj;
        ushort x;
        ushort y;
        int octaves = _terrainConfiguration.Octaves;
        int i;
        float scale = _terrainConfiguration.Scale;
        float persistance = _terrainConfiguration.Persistance;
        float lacunarity = _terrainConfiguration.Lacunarity;
        System.Random randomVar = new System.Random(_seed);
        Vector2 vector = new Vector2();
        Vector2[] octaveOffset = new Vector2[octaves];

        for (i = 0; i < octaves; i++)
        {
            vector.x = randomVar.Next(-100000, 100000);
            vector.y = randomVar.Next(-100000, 100000);
            octaveOffset[i] = vector;
        }

        float amplitude;
        float frequency;
        float noiseHeight;
        float sampleX;
        float sampleY;
        float perlinValue;

        for (x = startX; x < startX + _currentTerrainWidth; x++)
        {
            for (y = startY; y < startY + _terrainConfiguration.ChunkSize; y++)
            {
                amplitude = 1;
                frequency = 1;
                noiseHeight = 0;

                for (i = 0; i < octaves; i++)
                {
                    sampleX = x / scale * frequency + octaveOffset[i].x;
                    sampleY = y / scale * frequency + octaveOffset[i].y;

                    perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
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

        randomVar = null;
        octaveOffset = null;
    }

    private void CreateCavesParallel()
    {
        _caveMap = new float[_currentTerrainWidth, _currentTerrainHeight];
        _visitedCaveMap = new byte[_currentTerrainWidth, _currentTerrainHeight];
        List<Vector2Int> caveCoords = new List<Vector2Int>();

        bool isNonChunk;
        int count;
        int octaves = _terrainConfiguration.Octaves;
        float scale = _terrainConfiguration.Scale;
        float persistance = _terrainConfiguration.Persistance;
        float lacunarity = _terrainConfiguration.Lacunarity;
        Vector2[] octaveOffset = new Vector2[octaves];

        for (byte i = 0; i < octaves; i++)
        {
            octaveOffset[i].x = _randomVar.Next(-100000, 100000);
            octaveOffset[i].y = _randomVar.Next(-100000, 100000);
        }

        //Create noise map
        System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
        Parallel.For(0, _currentTerrainWidth, (index) =>
        {
            ushort x = (ushort)index;
            float amplitude;
            float frequency;
            float noiseHeight;
            float sampleX;
            float sampleY;
            float perlinValue;

            for (ushort y = 0; y < _currentTerrainHeight; y++)
            {
                amplitude = 1;
                frequency = 1;
                noiseHeight = 0;

                for (byte i = 0; i < octaves; i++)
                {
                    sampleX = x / scale * frequency + octaveOffset[i].x;
                    sampleY = y / scale * frequency + octaveOffset[i].y;

                    perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                _caveMap[x, y] = noiseHeight;
            }
        });
        watch.Stop();
        Debug.Log(watch.Elapsed.TotalSeconds);

        //Find max and min noise height
        watch.Restart();
        Parallel.For(0, _currentTerrainWidth, (index) =>
        {
            float localMin = float.MaxValue;
            float localMax = float.MinValue;

            for (int y = 0; y < _currentTerrainHeight; y++)
            {
                float value = _caveMap[index, y];
                localMin = Math.Min(localMin, value);
                localMax = Math.Max(localMax, value);
            }

            lock (this)
            {
                _minNoiseHeight = Math.Min(_minNoiseHeight, localMin);
                _maxNoiseHeight = Math.Max(_maxNoiseHeight, localMax);
            }
        });
        watch.Stop();
        Debug.Log(watch.Elapsed.TotalSeconds);

        //Lerp noise
        watch.Restart();
        Parallel.For(0, _currentTerrainWidth, (index) =>
        {
            ushort x = (ushort)index;
            for (ushort y = 0; y < _currentTerrainHeight; y++)
            {
                _caveMap[x, y] = Mathf.InverseLerp(_minNoiseHeight, _maxNoiseHeight, _caveMap[x, y]);
            }
        });
        watch.Stop();
        Debug.Log(watch.Elapsed.TotalSeconds);

        //Create cave in conditions
        watch.Restart();
        for (ushort x = 0; x < _currentTerrainWidth; x++)
        {
            for (ushort y = 0; y < _currentTerrainHeight; y++)
            {
                if (_caveMap[x, y] <= _terrainConfiguration.Intensity && _visitedCaveMap[x, y] == 0)
                {
                    (count, isNonChunk) = FloodFill(x, y, _caveMap, ref caveCoords);
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
                            _terrain.CreateBlock((ushort)v.x, (ushort)v.y, _airBlock);
                        }
                    }
                    caveCoords.Clear();
                }
            }
        }
        watch.Stop();
        Debug.Log(watch.Elapsed.TotalSeconds);
    }
    #endregion

    #region Phase 6
    private void CreateSpecialCaves()
    {
        ushort x;
        ushort startX;
        ushort startY;
        short tunnelDirection;
        short prevTunnelDirection = 0;
        int countOfRepeats = 0;
        int chance;

        for (x = (ushort)(_savannah.StartX + _terrainConfiguration.ChunkSize); x < _coniferousForest.StartX; x += _terrainConfiguration.ChunkSize)
        {
            chance = _randomVar.Next(0, 101);
            if (chance <= _terrainConfiguration.StarterCaveChance)
            {
                startX = (ushort)_randomVar.Next(x + 5, x + _terrainConfiguration.ChunkSize - _terrainConfiguration.MaxStarterCaveLength - 5);
                startY = (ushort)_randomVar.Next(_surfaceLevel.StartY + _terrainConfiguration.ChunkSize + 5, _terrainConfiguration.Equator - _terrainConfiguration.MaxStarterCaveHeight - 10);
                tunnelDirection = (short)(_randomVar.Next(0, 2) == 0 ? -1 : 1);
                tunnelDirection = (short)(countOfRepeats == 2 ? tunnelDirection - (tunnelDirection * 2) : tunnelDirection);

                if (CreateStarterCave(startX, startY, tunnelDirection))
                {
                    x += _terrainConfiguration.ChunkSize;
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
        int length = _randomVar.Next(_terrainConfiguration.MinStarterCaveLength, _terrainConfiguration.MaxStarterCaveLength);
        int height = _randomVar.Next(_terrainConfiguration.MinStarterCaveHeight, _terrainConfiguration.MaxStarterCaveHeight);

        //Define list of coords and air block
        List<Vector2Ushort> coords = new List<Vector2Ushort>();
        List<Vector2Ushort> stoneCoords = new List<Vector2Ushort>();
        List<Vector2Ushort> backgroundCoords = new List<Vector2Ushort>();
        Vector2Ushort vector = new Vector2Ushort();

        //Create rectangle
        ushort y;
        ushort x;
        for (x = startX; x <= startX + length; x++)
        {
            for (y = startY; y <= startY + height; y++)
            {
                if (!_worldData[x, y].CompareBlock(_airBlock))
                {
                    vector.x = x;
                    vector.y = y;
                    coords.Add(vector);
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
            if (_randomVar.Next(0, 2) == 1 || countOfRepeats == 2)
            {
                vector.x = x;
                vector.y = y;
                coords.Add(vector);
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
            if (_randomVar.Next(0, 2) == 1 || countOfRepeats == 2)
            {
                vector.x = x;
                vector.y = (ushort)(y - 1);
                stoneCoords.Add(vector);

                vector.x = x;
                vector.y = y;
                coords.Add(vector);
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
            if (_randomVar.Next(0, 2) == 1 || countOfRepeats == 2)
            {
                vector.x = x;
                vector.y = y;
                coords.Add(vector);
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
            if (_randomVar.Next(0, 2) == 1 || countOfRepeats == 2)
            {
                vector.x = x;
                vector.y = y;
                coords.Add(vector);
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
            CreateTunnel(tunnelDirection, startX, startY, ref coords, ref stoneCoords, ref backgroundCoords);
        }
        else
        {
            CreateTunnel(tunnelDirection, (ushort)(startX + length), startY, ref coords, ref stoneCoords, ref backgroundCoords);
        }


        //Fill terrain with air blocks
        foreach (Vector2Ushort coord in coords)
        {
            _terrain.CreateBlock(coord.x, coord.y, _airBlock);
        }

        //Fill terrain with stone blocks
        foreach (Vector2Ushort coord in stoneCoords)
        {
            _terrain.CreateBlock(coord.x, coord.y, _stoneBlock);
        }
        
        foreach (Vector2Ushort coord in backgroundCoords)
        {
            _terrain.CreateBackground(coord.x, coord.y, _dirtBG);
        }

        coords = null;
        stoneCoords = null;

        return true;
    }

    private void CreateTunnel(short direction, ushort startX, ushort startY, 
        ref List<Vector2Ushort> coords, ref List<Vector2Ushort> stoneCoords, ref List<Vector2Ushort> backgroundCoords)
    {
        short x = (short)startX;
        short y = (short)startY;
        int stepUp = 5;
        int stepUpBackground = 5;
        bool decreaseStep = false;
        int i;
        Vector2Ushort vector = new Vector2Ushort();

        while (true)
        {
            for (i = 0; i <= stepUp; i++)
            {
                vector.x = (ushort)x;
                vector.y = (ushort)(y + i);
                coords.Add(vector);
            }

            for (i = 0; i <= stepUpBackground; i++)
            {
                vector.x = (ushort)x;
                vector.y = (ushort)(y + i);
                backgroundCoords.Add(vector);
                if (_worldData[x, y + i + 2].CompareBlock(_airBlock))
                {
                    decreaseStep = true;
                }
            }

            if (_randomVar.Next(0, 2) == 1)
            {
                vector.x = (ushort)x;
                vector.y = (ushort)(y - 1);
                stoneCoords.Add(vector);
            }

            x += direction;
            if (_randomVar.Next(0, 2) == 1)
            {
                y++;
                if (decreaseStep)
                {
                    stepUpBackground -= 2;
                }
            }
            if (_worldData[x, y].CompareBlock(_airBlock))
            {
                break;
            }
        }
    }
    #endregion

    #region Phase 7
    private void CreateLakes()
    {
        ushort startX = (ushort)(_savannah.StartX + _terrainConfiguration.ChunkSize);
        ushort endX = (ushort)(_coniferousForest.EndX - _terrainConfiguration.ChunkSize);
        bool isChance = false;
        int i;
        int j;

        for (i = startX; i < endX; i += _terrainConfiguration.ChunkSize)
        {
            if (_randomVar.Next(0, 101) <= _terrainConfiguration.LakeChance || isChance)
            {
                for (j = i; j < i + _terrainConfiguration.ChunkSize; j++)
                {
                    isChance = true;
                    if (CreateLake(_surfaceCoords[j].x, _surfaceCoords[j].y))
                    {
                        isChance = false;
                        i += _terrainConfiguration.ChunkSize * _terrainConfiguration.LakeDistanceInChunks;
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
        Vector2Ushort vector = new Vector2Ushort();
        ushort lakeLength = (ushort)_randomVar.Next(_terrainConfiguration.MinLakeLength, _terrainConfiguration.MaxLakeLength);
        ushort lakeHeight = (ushort)_randomVar.Next(_terrainConfiguration.MinLakeHeight, _terrainConfiguration.MaxLakeHeight);
        ushort currentlakeHeight = 0;
        ushort startAdder = 0;
        ushort endAdder = 0;
        ushort i;
        ushort j;
        byte waterId = (byte)_waterBlock.GetId();


        //Fill list with potential blocks
        while (currentlakeHeight != lakeHeight)
        {
            i = startAdder;
            if (i > lakeLength - endAdder)
            {
                break;
            }
            for (; i < lakeLength - endAdder; i++)
            {
                vector.x = (ushort)(startX + i);
                vector.y = (ushort)(startY - currentlakeHeight);
                coords.Add(vector);
            }
            startAdder += (ushort)_randomVar.Next(1, 3);
            endAdder += (ushort)_randomVar.Next(1, 3);
            currentlakeHeight++;
        }

        //Verify lake conditions
        foreach (Vector2Ushort coord in coords)
        {
            if (_worldData[coord.x - 1, coord.y].CompareBlock(_airBlock) ||
                _worldData[coord.x + 1, coord.y].CompareBlock(_airBlock))
            {
                return false;
            }
            for (i = 0; i < 3; i++)
            {
                if (_worldData[coord.x, coord.y - i].CompareBlock(_airBlock))
                {
                    return false;
                }
            }
        }

        //Fill list to empty blocks
        for (i = 0; i < lakeLength; i++)
        {
            for (j = 1; ; j++)
            {
                if (_worldData[coords[i].x, coords[i].y + j].CompareBlock(_airBlock))
                {
                    break;
                }
                vector.x = coords[i].x;
                vector.y = (ushort)(coords[i].y + j);
                emptyCoords.Add(vector);
            }
        }

        //Smooth
        ushort smoothStartY = 1;
        ushort x;
        ushort y;
        byte chanceToMoveUp;

        for (x = (ushort)(startX + lakeLength); ; x++)
        {
            if (_worldData[x, startY + smoothStartY].CompareBlock(_airBlock))
            {
                break;
            }

            for (y = (ushort)(startY + smoothStartY); ; y++)
            {
                if (_worldData[x, y].CompareBlock(_airBlock))
                {
                    break;
                }
                vector.x = x;
                vector.y = y;
                emptyCoords.Add(vector);
            }

            chanceToMoveUp = (byte)_randomVar.Next(0, 3);
            if (chanceToMoveUp % 2 == 0)
            {
                smoothStartY++;
            }
        }

        //Create lake
        foreach (Vector2Ushort coord in coords)
        {
            _terrain.CreateBlock(coord.x, coord.y, _airBlock);
            _terrain.CreateLiquidBlock(coord.x, coord.y, waterId);
        }

        //Create air
        foreach (Vector2Ushort coord in emptyCoords)
        {
            _terrain.CreateBlock(coord.x, coord.y, _airBlock);
        }

        coords = null;
        emptyCoords = null;

        return true;
    }
    #endregion

    #region Phase 8
    private void CreateOasises()
    {
        ushort startX = (ushort)(_desert.StartX + _terrainConfiguration.ChunkSize);
        ushort endX = (ushort)(_desert.EndX - _terrainConfiguration.ChunkSize);
        ushort i;
        ushort j;
        bool isChance = false;

        for (i = startX; i < endX; i += _terrainConfiguration.ChunkSize)
        {
            if (_randomVar.Next(0, 101) <= _terrainConfiguration.OasisChance || isChance)
            {
                for (j = i; j < i + _terrainConfiguration.ChunkSize; j++)
                {
                    isChance = true;
                    if (CreateOasis(_surfaceCoords[j].x, _surfaceCoords[j].y))
                    {
                        isChance = false;
                        i += (ushort)(_terrainConfiguration.ChunkSize * _terrainConfiguration.OasisDistanceInChunks);
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
        Vector2Ushort vector = new Vector2Ushort();
        ushort oasisLength = (ushort)_randomVar.Next(_terrainConfiguration.MinOasisLength, _terrainConfiguration.MaxOasisLength);
        ushort oasisHeight = (ushort)_randomVar.Next(_terrainConfiguration.MinOasisHeight, _terrainConfiguration.MaxOasisHeight);
        ushort x;
        ushort y;
        byte waterId = (byte)_waterBlock.GetId();

        //Fill list with potential blocks
        for (x = startX; x < startX + oasisLength; x++)
        {
            for (y = startY; y > startY - oasisHeight; y--)
            {
                if (startY - y <= Ellipse(x - (startX + oasisLength / 2), oasisLength / 2, oasisHeight / 2))
                {
                    vector.x = x;
                    vector.y = y;
                    coords.Add(vector);
                }
            }
        }

        //Verify lake conditions
        ushort i;
        ushort j;

        foreach (Vector2Ushort coord in coords)
        {
            if (_worldData[coord.x - 1, coord.y].CompareBlock(_airBlock) ||
                _worldData[coord.x + 1, coord.y].CompareBlock(_airBlock))
            {
                return false;
            }
            for (i = 0; i < 3; i++)
            {
                if (_worldData[coord.x, coord.y - i].CompareBlock(_airBlock))
                {
                    return false;
                }
            }
        }

        //Fill list to empty blocks
        for (x = startX; x < startX + oasisLength; x++)
        {
            for (j = 1; ; j++)
            {
                if (_worldData[x, startY + j].CompareBlock(_airBlock))
                {
                    break;
                }
                vector.x = x;
                vector.y = (ushort)(startY + j);
                emptyCoords.Add(vector);
            }
        }

        //Smooth
        ushort smoothStartY = 1;
        byte chanceToMoveUp;

        for (x = (ushort)(startX + oasisLength); ; x++)
        {
            if (_worldData[x, startY + smoothStartY].CompareBlock(_airBlock))
            {
                break;
            }

            for (y = (ushort)(startY + smoothStartY); ; y++)
            {
                if (_worldData[x, y].CompareBlock(_airBlock))
                {
                    break;
                }
                vector.x = x;
                vector.y = y;
                emptyCoords.Add(vector);
            }

            chanceToMoveUp = (byte)_randomVar.Next(0, 3);
            if (chanceToMoveUp % 2 == 0)
            {
                smoothStartY++;
            }
        }

        //Create lake
        foreach (Vector2Ushort coord in coords)
        {
            _terrain.CreateBlock(coord.x, coord.y, _airBlock);
            _terrain.CreateLiquidBlock(coord.x, coord.y, waterId);
        }

        //Create air
        foreach (Vector2Ushort coord in emptyCoords)
        {
            _terrain.CreateBlock(coord.x, coord.y, _airBlock);
        }

        coords = null;
        emptyCoords = null;

        return true;
    }
    #endregion

    #region Phase 9
    private void GrassSeeding()
    {
        _surfaceCoords.Clear();
        BiomesID currentBiomeId;
        Vector2Ushort vector = new Vector2Ushort();
        ushort x;
        ushort y;


        for (x = 0; x < _currentTerrainWidth; x++)
        {
            for (y = _terrainConfiguration.Equator; y < _surfaceLevel.EndY; y++)
            {
                currentBiomeId = GameManager.Instance.GetChunk(x, y).Biome.Id;
                if (!_worldData[x, y + 1].IsEmpty())
                {
                    continue;
                }
                if (_worldData[x, y + 1].IsLiquid())
                {
                    continue;
                }
                vector.x = x;
                vector.y = y;
                _surfaceCoords.Add(vector);
                if (_worldData[x, y].CompareBlock(_dirtBlock))
                {
                    _terrain.CreateBlock(x, y, GameManager.Instance.ObjectsAtlass.GetGrassByBiome(currentBiomeId));
                }
            }
        }
    }
    #endregion

    #region Phase 10
    private void CreatePlants()
    {
        //Create surface plants
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
        byte chance;
        ushort startX;
        ushort endX;
        ushort x;
        ushort y;

        foreach (List<PlantSO> plants in allPlants)
        {
            foreach (PlantSO plant in plants)
            {
                if (plant.BiomeId == BiomesID.NonBiom)
                {
                    startX = 0;
                    endX = (ushort)(_currentTerrainWidth - 1);
                }
                else
                {
                    currentBiome = _terrainConfiguration.Biomes.Find(b => b.Id == plant.BiomeId);
                    startX = currentBiome.StartX;
                    endX = currentBiome.EndX;
                }

                for (x = startX; x <= endX; x++)
                {
                    for (y = _surfaceLevel.StartY; y < _surfaceLevel.EndY; y++)
                    {
                        chance = (byte)_randomVar.Next(0, 101);
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
                                _terrain.CreateBlock(x, (ushort)(y + 1), plant);
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
                                _terrain.CreateBlock(x, (ushort)(y - 1), plant);
                            }
                        }
                    }
                }
            }
        }

        allPlants = null;
    }
    #endregion

    #region Phase 11
    private void CreateTrees()
    {
        //Create surface trees
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
        byte chance;
        ushort startX;
        ushort endX;
        ushort x;
        ushort y;
        ushort i;
        bool isValidPlace;
        GameObject treesSection = GameManager.Instance.Terrain.Trees;

        foreach (var trees in allTrees)
        {
            foreach (Tree tree in trees.Value)
            {
                if (trees.Key == BiomesID.NonBiom)
                {
                    startX = 0;
                    endX = (ushort)(_currentTerrainWidth - 1);
                }
                else
                {
                    currentBiome = _terrainConfiguration.Biomes.Find(b => b.Id == trees.Key);
                    startX = currentBiome.StartX;
                    endX = currentBiome.EndX;
                }

                for (x = startX; x <= endX - tree.Width; x++)
                {
                    for (y = _terrainConfiguration.Equator; y < _surfaceLevel.EndY; y++)
                    {
                        isValidPlace = true;
                        for (i = 0; i < tree.WidthToSpawn; i++)
                        {
                            if (!tree.AllowedToSpawnOn.Contains(_worldData[x + i, y].BlockData))
                            {
                                isValidPlace = false;
                                break;
                            }
                            if (!_worldData[x + i, y + 1].IsEmptyForTree())
                            {
                                isValidPlace = false;
                                break;
                            }
                            if (_worldData[x + i, y + 1].IsLiquid())
                            {
                                isValidPlace = false;
                                break;
                            }
                        }
                        if (isValidPlace)
                        {
                            chance = (byte)_randomVar.Next(0, 101);
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

        allTrees = null;
        coords = null;
    }

    private bool CreateTree(ushort x, ushort y, Tree tree, ref List<Vector3> coords)
    {
        int blockX;
        int blockY;
        foreach (Vector3 vector in tree.TrunkBlocks)
        {
            blockX = x + (int)(vector.x - tree.Start.x);
            blockY = y;
            if (!_worldData[blockX, blockY].IsEmptyForTree())
            {
                return false;
            }
            if (_worldData[blockX, blockY].IsLiquid())
            {
                return false;
            }
        }
        foreach (Vector3 vector in tree.TreeBlocks)
        {
            blockX = x + (int)(vector.x - tree.Start.x);
            blockY = y;
            if (!_worldData[blockX, blockY].IsEmptyForTree())
            {
                return false;
            }
            if (_worldData[blockX, blockY].IsLiquid())
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
        Dictionary<BiomesID, List<PickableItem>> allPickableItems = null;
        List<Vector3> coords = new List<Vector3>();
        Vector3 vector = new Vector3();
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
        byte chance;
        ushort startX;
        ushort endX;
        ushort x;
        ushort y;
        GameObject pickableItemsSection = GameManager.Instance.Terrain.PickableItems;

        foreach (var pickableItems in allPickableItems)
        {
            foreach (PickableItem pickableItem in pickableItems.Value)
            {
                if (pickableItems.Key == BiomesID.NonBiom)
                {
                    startX = 0;
                    endX = (ushort)(_currentTerrainWidth - 1);
                }
                else
                {
                    currentBiome = _terrainConfiguration.Biomes.Find(b => b.Id == pickableItems.Key);
                    startX = currentBiome.StartX;
                    endX = currentBiome.EndX;
                }

                for (x = startX; x <= endX; x++)
                {
                    for (y = _terrainConfiguration.Equator; y < _surfaceLevel.EndY; y++)
                    {
                        if (!_worldData[x, y].IsSolid())
                        {
                            continue;
                        }
                        if (!_worldData[x, y + 1].IsEmpty())
                        {
                            continue;
                        }
                        chance = (byte)_randomVar.Next(0, 101);
                        if (chance <= pickableItem.ChanceToSpawn)
                        {
                            vector.x = x;
                            vector.y = y + 1;
                            coords.Add(vector);
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

        allPickableItems = null;
        coords = null;
    }
    #endregion

    #region Phase 13
    private void SetRandomTiles()
    {
        for (int x = 0; x < _currentTerrainWidth; x++)
        {
            for (int y = 0; y < _currentTerrainHeight; y++)
            {
                _worldData[x, y].SetRandomBlockTile(_randomVar);
                _worldData[x, y].SetRandomBackgroundTile(_randomVar);
            }
        }
    }
    #endregion

    #region Phase 14
    private void PreLaunchBlocksUpdate()
    {
        try
        {
            Parallel.For(5, _currentTerrainWidth - 5, (index) =>
            {
                for (int y = 5; y < _currentTerrainHeight - 5; y++)
                {
                    if (_worldData[index, y].IsDust() && _worldData[index, y - 1].IsEmpty())
                    {
                        lock (_lockObject)
                        {
                            _terrain.NeedToUpdate.Add(new Vector2Ushort(index, y));
                        }
                    }
                }
            });

            while (_terrain.NeedToUpdate.Count != 0)
            {
                _terrain.UpdateWorldData();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion

    #endregion

    #region Validation

    #endregion

    #region Helpful
    public float GenerateNoise(int x, int y, float scale, float amplitude, int additionalSeed = 0)
    {
        return Mathf.PerlinNoise((x + _seed + additionalSeed) / scale, (y + _seed + additionalSeed) / scale) * amplitude;
    }

    public float GenerateNoiseLandscape(int x, float compression, float height)
    {
        return Mathf.PerlinNoise((x + _seed) / compression, _seed / compression) * height;
    }

    private void SetBiomeIntoChunk(BiomeSO biome)
    {
        for (ushort x = biome.StartX; x < biome.EndX; x += _terrainConfiguration.ChunkSize)
        {
            GameManager.Instance.SetChunkBiome(x, _terrainConfiguration.Equator, biome);
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

            queue = null;

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