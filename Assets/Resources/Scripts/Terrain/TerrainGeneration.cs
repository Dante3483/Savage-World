using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Color = UnityEngine.Color;

public class TerrainGeneration : MonoBehaviour
{
    #region Private fields
    private World _world;
    private ObjectData[,] _objectsData;
    #endregion

    #region Properties
    public World World
    {
        get
        {
            return _world;
        }

        set
        {
            _world = value;
        }
    }
    #endregion

    #region Methods

    #region Helpful methods

    public float GenerateNoise(int x, int y, float noiseFreq, int additionalSeed = 0)
    {
        return Mathf.PerlinNoise((x + World.Seed + additionalSeed)/ noiseFreq, (y + World.Seed + additionalSeed)/ noiseFreq);
    }

    private List<ObjectData> GetCaveSize(int startX, int startY)
    {
        List<ObjectData> blocks = new List<ObjectData>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(startX, startY));
        _objectsData[startX, startY].IsChecked = true;

        while (queue.Count > 0)
        {
            Vector2Int position = queue.Dequeue();
            blocks.Add(_objectsData[position.x, position.y]);
            for (int x = position.x - 1; x <= position.x + 1; x++)
            {
                for (int y = position.y - 1; y <= position.y + 1; y++)
                {
                    if (World.IsInMapRange(x, y) && (y == position.y || x == position.x))
                    {
                        if (!_objectsData[x, y].IsChecked && _objectsData[x, y].Type == ObjectType.Empty)
                        {
                            _objectsData[x, y].IsChecked = true;
                            queue.Enqueue(new Vector2Int(x, y));
                        }
                    }
                }
            }
        }
        return blocks;
    }

    private bool IsValidSurvivalistCave(int centerX, int centerY, int witdh, int height)
    {
        for (int dx = centerX - witdh / 2; dx < centerX + witdh / 2; dx++)
        {
            for (int dy = centerY - height / 2; dy < centerY + height / 2; dy++)
            {
                if (_objectsData[dx, dy].Type == ObjectType.Empty)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private bool IsValidLake(int startX, int startY, int witdh, int height)
    {
        for (int dx = startX; dx < startX + witdh; dx++)
        {
            for (int dy = startY - 1; dy > startY - height; dy--)
            {
                if (_objectsData[dx, dy].Type == ObjectType.Empty)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private bool IsValidTree(GameObject Tree)
    {
        Tree tree = Tree.GetComponent<Tree>();
        foreach (var block in tree.TrunkBlocks)
        {
            Vector2 point = Tree.transform.TransformPoint(new Vector2(block.x, block.y));
            Vector3Int intPosition = Vector3Int.RoundToInt(point);
            if (_objectsData[intPosition.x, intPosition.y].Type != ObjectType.Empty)
            {
                return false;
            }
        }
        foreach (var block in tree.FoliageBlocks)
        {
            Vector2 point = Tree.transform.TransformPoint(new Vector2(block.x, block.y));
            Vector3Int intPosition = Vector3Int.RoundToInt(point);
            if (_objectsData[intPosition.x, intPosition.y].Type != ObjectType.Empty)
            {
                return false;
            }
        }
        return true;
    }

    #endregion

    #region Generation

    public void Generation()
    {
        //Create new maps
        CreateNewWorldMap();

        //Terrain generation
        int currrentXLevel = 0;
        int currrentYLevel = 0;
        int chunkSize = World.TerrainConfiguration.chunkSize;
        Debug.Log("Start terrain generation");
        var watch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < World.TerrainConfiguration.horizontalChunksCount; i++)
        {
            currrentYLevel = 0;
            foreach (TerrainLevel level in World.TerrainConfiguration.levels.AsEnumerable().Reverse())
            {
                for (int j = 0; j < level.chunkCount; j++)
                {
                    Chunk newChunk = new Chunk();
                    newChunk.x = currrentXLevel;
                    newChunk.y = currrentYLevel;
                    World.Chunks[currrentXLevel / chunkSize, currrentYLevel / chunkSize] = newChunk;
                    GenerateTerrain(new Vector2Int(currrentXLevel, currrentYLevel), level);
                    currrentYLevel += chunkSize;
                }
            }
            currrentXLevel += chunkSize;
        }
        watch.Stop();
        Debug.Log($"Terrain generation complete: {watch.Elapsed.TotalSeconds}");

        //Ocean generation
        if (World.TerrainConfiguration.EnableOceanGeneration)
        {
            Debug.Log("Start ocean biome generation");
            watch.Restart();
            CreateOceanBiome();
            World.TerrainConfiguration.SetBiomesInChunks(World.Chunks);
            watch.Stop();
            Debug.Log($"Ocean biome generation complete: {watch.Elapsed.TotalSeconds}");
        }

        //Biomes generation
        if (World.TerrainConfiguration.EnableBiomGeneration)
        {
            watch.Restart();
            Debug.Log("Start biomes generation");
            Debug.Log("Start desert biome generation");
            CreateDesertBiome();
            Debug.Log("Desert biome generation complete");
            Debug.Log("Start plains biome generation");
            CreatePlainsBiome();
            Debug.Log("Plains biome generation complete");
            Debug.Log("Start meadow biome generation");
            CreateMeadowBiome();
            Debug.Log("Meadow biome generation complete");
            Debug.Log("Start forest biome generation");
            CreateForestBiome();
            Debug.Log("Forest biome generation complete");
            Debug.Log("Start swamp biome generation");
            CreateSwampBiome();
            Debug.Log("Swamp biome generation complete");
            Debug.Log("Start coniferous forest biome generation");
            CreateConiferousForestBiome();
            Debug.Log("Coniferous forest biome generation complete");
            watch.Stop();
            Debug.Log($"Biomes generation complete: {watch.Elapsed.TotalSeconds}");
        }

        //Holes generation
        if (World.TerrainConfiguration.EnableHolesGeneration)
        {
            currrentXLevel = 0;
            currrentYLevel = 0;
            Debug.Log("Start holes generation");
            watch.Restart();
            for (int i = 0; i < World.TerrainConfiguration.horizontalChunksCount; i++)
            {
                currrentYLevel = 0;
                foreach (TerrainLevel level in World.TerrainConfiguration.levels.AsEnumerable().Reverse())
                {
                    for (int j = 0; j < level.chunkCount; j++)
                    {
                        Chunk chunk = World.Chunks[currrentXLevel / chunkSize, currrentYLevel / chunkSize];
                        GenerateHoles(new Vector2Int(currrentXLevel, currrentYLevel), level.Holes);
                        currrentYLevel += chunkSize;
                    }
                }
                currrentXLevel += chunkSize;
            }
            Debug.Log($"Holes generation complete: {watch.Elapsed.TotalSeconds}");
        }

        //Clusters generation
        if (World.TerrainConfiguration.EnableClustersGenration)
        {
            Debug.Log("Start clusters generation");
            watch.Restart();
            foreach (TerrainLevel level in World.TerrainConfiguration.levels)
            {
                GenerateClusters(level);
            }
            watch.Stop();
            Debug.Log($"Clusters generation complete: {watch.Elapsed.TotalSeconds}");
        }

        //Caves generation
        if (World.TerrainConfiguration.EnableCavesGeneration)
        {
            currrentXLevel = 0;
            currrentYLevel = 0;
            Debug.Log("Start caves generation");
            watch.Restart();
            for (int i = 0; i < World.TerrainConfiguration.horizontalChunksCount; i++)
            {
                currrentYLevel = 0;
                foreach (TerrainLevel level in World.TerrainConfiguration.levels.AsEnumerable().Reverse())
                {
                    for (int j = 0; j < level.chunkCount; j++)
                    {
                        Chunk chunk = World.Chunks[currrentXLevel / chunkSize, currrentYLevel / chunkSize];
                        GenerateCaves(new Vector2Int(currrentXLevel, currrentYLevel), level, chunk);
                        currrentYLevel += chunkSize;
                    }
                }
                currrentXLevel += chunkSize;
            }
            foreach (TerrainLevel level in World.TerrainConfiguration.levels.AsEnumerable().Reverse())
            {
                DeleteSmallCaves(level);
            }
            watch.Stop();
            Debug.Log($"Caves generation complete: {watch.Elapsed.TotalSeconds}");
        }

        //Survivalist caves generation
        if (World.TerrainConfiguration.EnableSurvivalistCavesGeneration)
        {
            Debug.Log("Start survivalist caves generation");
            watch.Restart();
            foreach (TerrainLevel level in World.TerrainConfiguration.levels)
            {
                GenerateSurvivalistCave(level);
            }
            watch.Stop();
            Debug.Log($"Survivalist caves generation complete: {watch.Elapsed.TotalSeconds}");
        }

        //Ore generation
        if (World.TerrainConfiguration.EnableOreGeneration)
        {
            Debug.Log("Start ores generation");
            watch.Restart();
            GenerateOres();
            watch.Stop();
            Debug.Log($"Ores generation complete: {watch.Elapsed.TotalSeconds}");
        }

        //Smooth world
        if (World.TerrainConfiguration.EnableSmoothingWorld)
        {
            SmoothWorld();
        }

        //Lakes generation
        if (World.TerrainConfiguration.EnableLakesGeneration)
        {
            Debug.Log("Start lakes generation");
            watch.Restart();
            GenerateLakes();
            watch.Stop();
            Debug.Log($"Lakes generation complete: {watch.Elapsed.TotalSeconds}");
        }

        //Tree generation
        if (World.TerrainConfiguration.EnableTreeGeneration)
        {
            Debug.Log("Start tree genearion");
            watch.Restart();
            GenerateTrees();
            watch.Stop();
            Debug.Log($"Tree generation complete: {watch.Elapsed.TotalSeconds}");
        }

        //Grass seeding
        GrassSeeding();

        //Planting
        if (World.TerrainConfiguration.EnablePlanting)
        {
            Debug.Log("Start planting");
            watch.Restart();
            foreach (TerrainLevel level in World.TerrainConfiguration.levels)
            {
                PlantWorld(level);
            }
            watch.Stop();
            Debug.Log($"Planting complete: {watch.Elapsed.TotalSeconds}");
        }

        //Structure generation
        if (World.TerrainConfiguration.EnableStructureGeneration)
        {
            Debug.Log("Start structures genearion");
            watch.Restart();
            foreach (Structure structure in World.TerrainConfiguration.Structures)
            {
                GenerateStructure(structure);
            }
            watch.Stop();
            Debug.Log($"Structures generation complete: {watch.Elapsed.TotalSeconds}");
        }
    }

    private void GenerateTrees()
    {
        int startX = 5;
        int startY = World.TerrainConfiguration.levels.Find(x => x.Type == TerrainLevelID.Surface).startY;
        int endX = World.TerrainConfiguration.WorldWitdh - 5;
        int endY = World.TerrainConfiguration.levels.Find(x => x.Type == TerrainLevelID.Surface).endY;
        int chunkSize = World.TerrainConfiguration.chunkSize;
        GameObject treesGameObject = new GameObject("Trees");
        treesGameObject.transform.parent = transform.parent;

        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                if (World.IsInMapRange(x, y))
                {
                    Chunk currentChunk = World.Chunks[x / chunkSize, y / chunkSize];
                    List<GameObject> trees = World.ObjectAtlas.GetBiomeTreesByID(currentChunk.BiomeID);
                    if (_objectsData[x, y].Type != ObjectType.Empty &&
                        _objectsData[x, y + 1].Type == ObjectType.Empty)
                    {
                        if (trees != null && trees.Count != 0)
                        {
                            GameObject treeGameObject = trees[World.RandomVar.Next(0, trees.Count)];
                            Tree tree = treeGameObject.GetComponent<Tree>();
                            bool validPlace = true;
                            for (int dx = x; dx < x + tree.WidthToSpawn; dx++)
                            {
                                if (!tree.AllowedToSpawnOn.Exists(b => b.blockType == _objectsData[dx,y].Type && b.GetID() == _objectsData[dx, y].Id) ||
                                    _objectsData[dx, y + 1].Type != ObjectType.Empty)
                                {
                                    validPlace = false;
                                    break;
                                }
                            }
                            if (validPlace)
                            {
                                GameObject createdTree = Instantiate(treeGameObject);
                                createdTree.transform.position = new Vector3(x + tree.WidthToSpawn / 2f, y + 1);
                                createdTree.transform.parent = treesGameObject.transform;
                                if (GenerateTree(createdTree))
                                {
                                    x += World.TerrainConfiguration.MinDistanceTree;
                                }
                            }
                        }
                    }
                }       
            }
        }
    }

    private bool GenerateTree(GameObject treeGameObject)
    {
        Tree tree = treeGameObject.GetComponent<Tree>();
        bool isValid = World.IsInMapRange((int)tree.transform.position.x + tree.Width, (int)tree.transform.position.y);
        if (isValid && IsValidTree(treeGameObject))
        {
            foreach (var block in tree.TrunkBlocks)
            {
                Vector2 point = treeGameObject.transform.TransformPoint(new Vector2(block.x, block.y));
                Vector3Int intPosition = Vector3Int.RoundToInt(point);
                _objectsData[intPosition.x, intPosition.y].IsTreeTrunk = true;
            }
            foreach (var block in tree.FoliageBlocks)
            {
                Vector2 point = treeGameObject.transform.TransformPoint(new Vector2(block.x, block.y));
                Vector3Int intPosition = Vector3Int.RoundToInt(point);
                _objectsData[intPosition.x, intPosition.y].IsTreeFoliage = true;
            }
            return true;
        }
        else
        {
            Destroy(treeGameObject);
            return false;
        }
    }

    private void GenerateLakes()
    {
        #region Lake
        ObjectData currentObjectData;
        int startY;
        int startX = World.TerrainConfiguration.PlainsPosition.EndPositionX;
        for (int x = startX; x < World.TerrainConfiguration.WorldWitdh - 100; x++)
        {
            startY = World.TerrainConfiguration.Equator + 1;
            currentObjectData = _objectsData[x, startY];
            while (_objectsData[x, currentObjectData.Position.y + 1].Type != ObjectType.Empty)
            {
                currentObjectData = _objectsData[x, currentObjectData.Position.y + 1];
            }
            if (_objectsData[x, currentObjectData.Position.y - 1].Type != ObjectType.Empty)
            {
                int id = World.RandomVar.Next(0, 10);
                bool isChecked = false;
                bool isValid = false;
                int lakeWidth = World.RandomVar.Next(40,100);
                int lakeHeight = 0;
                Queue<ObjectData> queue = new Queue<ObjectData>();
                for (int dx = x; dx < x + lakeWidth; dx++)
                {
                    int randY = World.RandomVar.Next(0, 2);
                    if (dx <= x + lakeWidth / 2)
                    {
                        lakeHeight += randY;
                    }
                    else
                    {
                        if (!isChecked)
                        {
                            isValid = IsValidLake(currentObjectData.Position.x, currentObjectData.Position.y, lakeWidth, lakeHeight);
                            isChecked = true;
                        }
                        lakeHeight -= randY;
                    }
                    if (isChecked && !isValid)
                    {
                        queue.Clear();
                        break;
                    }

                    for (int dy = currentObjectData.Position.y; dy >= currentObjectData.Position.y - lakeHeight; dy--)
                    {
                        queue.Enqueue(_objectsData[dx, dy]);
                    }
                }
                if (isChecked && isValid)
                {
                    x += lakeWidth + 105;
                    if (World.RandomVar.Next(0, 101) <= World.TerrainConfiguration.ChanceToSpawnLake)
                    {
                        while (queue.Count != 0)
                        {
                            ObjectData objectData = queue.Dequeue();
                            World.CreateBlock(World.ObjectAtlas.Water, objectData.Position.x, objectData.Position.y);
                            _objectsData[objectData.Position.x, objectData.Position.y].CurrentFlowValue = 1f;
                        }
                        for (int dx = currentObjectData.Position.x; dx < currentObjectData.Position.x + lakeWidth; dx++)
                        {
                            int dy = currentObjectData.Position.y;
                            while (_objectsData[dx, dy].Type != ObjectType.Empty) 
                            {
                                World.CreateBlock(World.ObjectAtlas.Air, dx, dy);
                                dy++;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Oasis
        double Ellipse(int x, int a, int b)
        {
            return Math.Sqrt((1 - Math.Pow(x, 2) / Math.Pow(a, 2)) * Math.Pow(b, 2));
        }
        startX = World.TerrainConfiguration.DesertPosition.StartPositionX;
        int endX = World.TerrainConfiguration.DesertPosition.EndPositionX;
        startY = World.TerrainConfiguration.Equator + 1;
        for (int x = startX + 50; x < endX - 50; x++)
        {
            currentObjectData = _objectsData[x, startY];
            while (_objectsData[x, currentObjectData.Position.y + 1].Type != ObjectType.Empty)
            {
                currentObjectData = _objectsData[x, currentObjectData.Position.y + 1];
            }
            int oasisWidth = World.RandomVar.Next(20, 41);
            int oasisHeight = World.RandomVar.Next(20, 31);
            if (IsValidLake(currentObjectData.Position.x, currentObjectData.Position.y, oasisWidth, oasisHeight))
            {
                if (World.RandomVar.Next(0, 101) <= World.TerrainConfiguration.ChanceToSpawnOasis)
                {
                    for (int dx = currentObjectData.Position.x; dx < currentObjectData.Position.x + oasisWidth; dx++)
                    {
                        for (int dy = currentObjectData.Position.y - oasisHeight; dy <= currentObjectData.Position.y; dy++)
                        {
                            if (currentObjectData.Position.y - dy <= Ellipse(dx - (currentObjectData.Position.x + oasisWidth / 2), oasisWidth / 2, oasisHeight / 2))
                            {
                                World.CreateBlock(World.ObjectAtlas.Water, dx, dy);
                                _objectsData[dx, dy].CurrentFlowValue = 1f;
                            }
                        }
                    }
                    for (int dx = currentObjectData.Position.x; dx < currentObjectData.Position.x + oasisWidth; dx++)
                    {
                        int dy = currentObjectData.Position.y;
                        while (_objectsData[dx, dy].Type != ObjectType.Empty)
                        {
                            World.CreateBlock(World.ObjectAtlas.Air, dx, dy);
                            dy++;
                        }
                    }
                    SmoothMountains(currentObjectData.Position.x + 1, currentObjectData.Position.y - 1, -1);
                    SmoothMountains(currentObjectData.Position.x + oasisWidth - 1, currentObjectData.Position.y - 1, 1);
                }
                x += oasisWidth + 250;
            }
        }
        #endregion
    }

    private void GenerateStructure(Structure structure)
    {
        for (int x = 0; x < structure.StructureTemplate.texture.width; x++)
        {
            for (int y = 0; y < structure.StructureTemplate.texture.height; y++)
            {
                Color color = structure.StructureTemplate.texture.GetPixel(x, y);
                BlockSO block = structure.colorsAndBlocks.Find(b => b.ColorOnTemplate == color).BlockOnTemplate;
                World.CreateBlock(block, x, y);
            }
        }
    }

    private void PlantWorld(TerrainLevel level)
    {
        //Plant vines
        PlantSO vine = (World.ObjectAtlas.Vine as PlantSO);
        if (vine.levelID == level.Type)
        {
            for (int x = World.TerrainConfiguration.DesertPosition.EndPositionX + 1; x < World.TerrainConfiguration.WorldWitdh; x++)
            {
                for (int y = level.startY; y < level.endY; y++)
                {
                    if (World.RandomVar.Next(0, 101) <= vine.ChanceToSpawn &&
                        vine.AllowedToSpawnOn.ToList().Find(b => b.blockType == _objectsData[x, y + 1].Type && b.GetID() == _objectsData[x, y + 1].Id) &&
                        _objectsData[x, y].Type == ObjectType.Empty)
                    {
                        World.CreateBlock(vine, x, y);
                    }
                }
            }
        }

        //Biomes specified plants
        List<List<BlockSO>> biomeSpecifiedPlants = new List<List<BlockSO>>
        {
            World.ObjectAtlas.GetBiomesBlocks(ObjectType.Plant, BiomesID.Desert),
            World.ObjectAtlas.GetBiomesBlocks(ObjectType.Plant, BiomesID.Plains),
            World.ObjectAtlas.GetBiomesBlocks(ObjectType.Plant, BiomesID.Meadow),
            World.ObjectAtlas.GetBiomesBlocks(ObjectType.Plant, BiomesID.Forest),
            World.ObjectAtlas.GetBiomesBlocks(ObjectType.Plant, BiomesID.Swamp),
            World.ObjectAtlas.GetBiomesBlocks(ObjectType.Plant, BiomesID.ConiferousForest),
        };

        foreach (var biomePlants in biomeSpecifiedPlants)
        {
            foreach (PlantSO plant in biomePlants)
            {
                for (int x = 0; x < World.TerrainConfiguration.WorldWitdh; x++)
                {
                    for (int y = level.startY; y < level.endY; y++)
                    {
                        if (plant.levelID == level.Type &&
                            World.Chunks[x/World.TerrainConfiguration.chunkSize, y / World.TerrainConfiguration.chunkSize].BiomeID == plant.BiomeID &&
                            World.RandomVar.Next(0, 101) <= plant.ChanceToSpawn &&
                            plant.AllowedToSpawnOn.ToList().Find(b => b.blockType == _objectsData[x, y - 1].Type && b.GetID() == _objectsData[x, y - 1].Id) &&
                            _objectsData[x, y].Type == ObjectType.Empty)
                        {
                            World.CreateBlock(plant, x, y);
                        }
                    }
                }
            }
        }
    }

    public void CreateNewWorldMap()
    {
        //Initialize blocksData array with specified width and height
        //Width = count of horizontal chunks * chunk size
        //Height = count of chunk in specified terrain level * chunk size
        int width = 0;
        int height = 0;
        int chunkSize = World.TerrainConfiguration.chunkSize;
        width = World.TerrainConfiguration.horizontalChunksCount * World.TerrainConfiguration.chunkSize;
        foreach (TerrainLevel level in World.TerrainConfiguration.levels)
        {
            height += level.chunkCount * World.TerrainConfiguration.chunkSize;
        }
        World.Chunks = new Chunk[width / chunkSize, height / chunkSize];
    }
    
    public void GenerateTerrain(Vector2Int startPosition, TerrainLevel level)
    {
        float height;
        int chunkSize = World.TerrainConfiguration.chunkSize;

        for (int x = startPosition.x; x < startPosition.x + chunkSize; x++)
        {
            //Define max y level for surface or other terrain levels
            if (level.Type == TerrainLevelID.Surface)
            {
                height = startPosition.y + chunkSize / 2;
            }
            else
            {
                height = startPosition.y + chunkSize;
            }
            for (int y = startPosition.y; y < height; y++)
            {
                World.CreateBlock(level.defaultBlock, x, y);
            }
        }
    }
    
    public void GenerateHoles(Vector2Int startPosition, List<Hole> holes)
    {
        int chunkSize = World.TerrainConfiguration.chunkSize;
        foreach (Hole hole in holes)
        {
            for (int x = startPosition.x; x < startPosition.x + chunkSize; x++)
            {
                for (int y = startPosition.y; y < startPosition.y + chunkSize; y++)
                {
                    if (GenerateNoise(x, y, hole.freq) > hole.size)
                    {
                        World.CreateBlock(hole.fillBlock, x, y);
                        if (hole.fillBlock.blockType == ObjectType.LiquidBlock)
                        {
                            _objectsData[x, y].CurrentFlowValue = 1f;
                        }
                    }
                }
            }
        }
    }
    
    public void GenerateCaves(Vector2Int startPosition, TerrainLevel level, Chunk chunk)
    {
        if (chunk.IsAllowedToCave)
        {
            for (int x = startPosition.x; x < startPosition.x + World.TerrainConfiguration.chunkSize; x++)
            {
                for (int y = startPosition.y; y < startPosition.y + World.TerrainConfiguration.chunkSize; y++)
                {
                    if (GenerateNoise(x, y, level.caveFreq) > level.caveSize)
                    {
                        World.CreateBlock(World.ObjectAtlas.Air, x, y, World.ObjectAtlas.GetBlockByID(_objectsData[x,y].Type, _objectsData[x,y].Id));
                    }
                }
            }
        }
    }

    public void GenerateSurvivalistCave(TerrainLevel level)
    {
        if (level.Type == TerrainLevelID.Surface)
        {
            int startX = World.TerrainConfiguration.DesertPosition.EndPositionX + 51;
            while (startX + 150 <= World.TerrainConfiguration.WorldWitdh)
            {
                int xOffset = startX + 100;
                int x = World.RandomVar.Next(startX, xOffset);
                int y = World.RandomVar.Next(level.startY + 20, World.TerrainConfiguration.Equator - 10);
                int randWitdh = World.RandomVar.Next(20, 40);
                int randHeight = World.RandomVar.Next(5, 10);

                if (World.RandomVar.Next(1,101) <= World.TerrainConfiguration.chanceToSpawnSurvivalistCave && IsValidSurvivalistCave(x, y, randWitdh, randHeight))
                {
                    int direction = World.RandomVar.Next(0, 2);
                    for (int dx = x - randWitdh / 2; dx < x + randWitdh / 2; dx++)
                    {
                        for (int dy = y - randHeight / 2; dy < y + randHeight / 2; dy++)
                        {
                            //Right side
                            if (direction == 1)
                            {
                                CreateWavesTunnel(1, x + randWitdh / 2, y - randHeight / 2);
                            }
                            //Left side
                            else
                            {
                                CreateWavesTunnel(-1, x - randWitdh / 2, y - randHeight / 2);
                            }
                            //Noise bottom side
                            if (dy == y - randHeight / 2)
                            {
                                if (World.RandomVar.Next(0, 2) == 1)
                                {
                                    World.CreateBlock(World.ObjectAtlas.Air, dx, dy - 1);
                                }
                            }
                            //Noise left or right side
                            //If left side
                            if (direction == 0)
                            {
                                if (dx == x - randWitdh / 2 && World.RandomVar.Next(0, 2) == 1)
                                {
                                    World.CreateBlock(World.ObjectAtlas.Air, dx - 1, dy);
                                }
                            }
                            //If right side
                            else
                            {
                                if (dx == x + randWitdh / 2 && World.RandomVar.Next(0, 2) == 1)
                                {
                                    World.CreateBlock(World.ObjectAtlas.Air, dx + 1, dy);
                                }
                            }
                            //Noise top side
                            if (dy == y + randHeight / 2 - 1)
                            {
                                if (World.RandomVar.Next(0, 2) == 1)
                                {
                                    for (int i = 1; i <= World.RandomVar.Next(1, 4); i++)
                                    {
                                        World.CreateBlock(World.ObjectAtlas.Air, dx, dy + i);
                                    }
                                }
                            }
                            World.CreateBlock(World.ObjectAtlas.Air, dx, dy);
                        }
                    }
                }
                startX = xOffset + 50;
            }
        }
    }

    private void CreateWavesTunnel(int startDirection, int startX, int startY)
    {
        int x = startX;
        int y = startY;
        int countOfRepeat = 0;
        int moveType = 0;
        while (true)
        {
            int stepUp = 5;
            DeletePillar(x, y, stepUp);
            x += startDirection;
            if (World.RandomVar.Next(0, 2) == 1)
            {
                y++;
            }
            if (_objectsData[x, y].Type == ObjectType.Empty)
            {
                break;
            }
        }

        void DeletePillar(int x, int y, int height)
        {
            for (int i = 0; i <= height; i++)
            {
                World.CreateBlock(World.ObjectAtlas.Air, x, y + i);
            }
        }
    }

    private void DeleteSmallCaves(TerrainLevel level)
    {
        List<ObjectData> blocksToDelete = new List<ObjectData>();
        if (level.caveSize != 0)
        {
            for (int x = 0; x < World.TerrainConfiguration.WorldWitdh; x++)
            {
                for (int y = level.startY; y <= level.endY; y++)
                {
                    int startX = x;
                    int startY = y;
                    if (_objectsData[x, y].Type == ObjectType.Empty && !_objectsData[x, y].IsChecked)
                    {
                        blocksToDelete.Clear();
                        blocksToDelete = GetCaveSize(startX, startY);
                        if (blocksToDelete.Count < level.MinGeneratedCaveSize)
                        {
                            for (int i = 0; i < blocksToDelete.Count; i++)
                            {
                                int dx = blocksToDelete[i].Position.x;
                                int dy = blocksToDelete[i].Position.y;
                                BlockSO block = World.ObjectAtlas.GetBlockByID(_objectsData[dx, dy].TypeBackground, _objectsData[dx,dy].IdBackground);
                                World.CreateBlock(block, dx, dy);
                            }
                        }
                    }
                }
            }
        }
    }

    #region Biomes generation
    //30% of all chunks (if world size = 84 chunks => Round to 26)
    //Plants: Oak tree, birch tree, grass, flower, bushes;
    //Animals: Chickens, Bushes rabbits, cows, pigs, birds;
    //Enemies: ----
    public void CreateMeadowBiome()
    {
        int chunkSize = World.TerrainConfiguration.chunkSize;
        int countOfMeadowChunks = World.TerrainConfiguration.CountOfMeadowChunks;
        int startXPosition = World.TerrainConfiguration.MeadowPosition.StartPositionX;
        int startYPosition = World.TerrainConfiguration.Equator;

        for (int x = startXPosition - 1; x < startXPosition + countOfMeadowChunks * chunkSize; x++)
        {
            float height = startYPosition + Mathf.PerlinNoise(
                (x + World.Seed) * World.TerrainConfiguration.mountainFreq,
                World.Seed * World.TerrainConfiguration.mountainFreq) * World.TerrainConfiguration.heightMeadowMountainMultiplier;
            for (int y = startYPosition; y < height; y++)
            {
                World.CreateBlock(World.ObjectAtlas.Dirt, x, y);
            }
        }
    }

    //10% of all chunks (if world size = 84 chunks => Round to 9)
    public void CreateForestBiome()
    {
        int chunkSize = World.TerrainConfiguration.chunkSize;
        int countOfForestChunks = World.TerrainConfiguration.CountOfForestChunks;
        int startXPosition = World.TerrainConfiguration.ForestPosition.StartPositionX;
        int startYPosition = World.TerrainConfiguration.Equator;

        for (int x = startXPosition; x < startXPosition + countOfForestChunks * chunkSize; x++)
        {
            float height = startYPosition + Mathf.PerlinNoise(
                (x + World.Seed) * World.TerrainConfiguration.mountainFreq,
                World.Seed * World.TerrainConfiguration.mountainFreq) * World.TerrainConfiguration.heightMeadowMountainMultiplier;
            for (int y = startYPosition; y < height; y++)
            {
                World.CreateBlock(World.ObjectAtlas.Dirt, x, y);
            }
        }
    }

    //10% of all chunks (if world size = 84 chunks => Floor to 8)
    public void CreateSwampBiome()
    {
        int chunkSize = World.TerrainConfiguration.chunkSize;
        int countOfSwampChunks = World.TerrainConfiguration.CountOfSwampChunks;
        int startXPosition = World.TerrainConfiguration.SwampPosition.StartPositionX;
        int startYPosition = World.TerrainConfiguration.Equator;

        for (int x = startXPosition; x < startXPosition + countOfSwampChunks * chunkSize; x++)
        {
            float height = startYPosition + Mathf.PerlinNoise(
                (x + World.Seed) * World.TerrainConfiguration.mountainFreq,
                World.Seed * World.TerrainConfiguration.mountainFreq) * World.TerrainConfiguration.heightMeadowMountainMultiplier;
            for (int y = startYPosition; y < height; y++)
            {
                World.CreateBlock(World.ObjectAtlas.Dirt, x, y);
            }
        }
    }

    //10% of all chunks (if world size = 84 chunks => Floor to 8)
    public void CreateConiferousForestBiome()
    {
        int chunkSize = World.TerrainConfiguration.chunkSize;
        int countOfConiferousForestChunks = World.TerrainConfiguration.CountOfConiferousForestChunks;
        int startXPosition = World.TerrainConfiguration.ConiferousForestPosition.StartPositionX;
        int startYPosition = World.TerrainConfiguration.Equator;

        for (int x = startXPosition; x < startXPosition + countOfConiferousForestChunks * chunkSize - 1; x++)
        {
            float height = startYPosition + Mathf.PerlinNoise(
                (x + World.Seed) * World.TerrainConfiguration.mountainFreq,
                World.Seed * World.TerrainConfiguration.mountainFreq) * World.TerrainConfiguration.heightMeadowMountainMultiplier;
            for (int y = startYPosition; y < height; y++)
            {
                World.CreateBlock(World.ObjectAtlas.Dirt, x, y);
            }
        }
    }

    //5% of all chunks (if world size = 84 chunks => Floor to 4)
    public void CreatePlainsBiome()
    {
        int chunkSize = World.TerrainConfiguration.chunkSize;
        int countOfPlainsChunks = World.TerrainConfiguration.CountOfPlainsChunks;
        int startXPosition = World.TerrainConfiguration.PlainsPosition.StartPositionX;
        int startYPosition = World.TerrainConfiguration.Equator;

        for (int x = startXPosition; x < startXPosition + countOfPlainsChunks * chunkSize; x++)
        {
            float height = startYPosition + Mathf.PerlinNoise(
                (x + World.Seed) * World.TerrainConfiguration.mountainFreq,
                World.Seed * World.TerrainConfiguration.mountainFreq) * World.TerrainConfiguration.heightPlainsMountainMultiplier;
            for (int y = startYPosition; y < height; y++)
            {
                World.CreateBlock(World.ObjectAtlas.Dirt, x, y);
            }
        }
    }

    //25% of all chunks (if world size = 84 chunks => Round to 21)
    public void CreateDesertBiome()
    {
        int chunkSize = World.TerrainConfiguration.chunkSize;
        int countOfDesertChunks = World.TerrainConfiguration.CountOfDesertChunks;
        int startXPosition = World.TerrainConfiguration.DesertPosition.EndPositionX;
        int startYPosition = World.TerrainConfiguration.Equator;

        int randomDownSandCount = 0;
        int minSandCount = 20;
        int maxSandCount = 25;

        int chancePulverize;
        int lengthOfPulverizing = 10;

        for (int x = startXPosition; x > 5; x--)
        {
            for (int y = startYPosition; y > World.TerrainConfiguration.OceanPosition.StartPositionY; y--)
            {
                if (_objectsData[x, y].Type != ObjectType.LiquidBlock)
                {
                    World.CreateBlock(World.ObjectAtlas.Sand, x, y);
                }
            }
            randomDownSandCount = World.RandomVar.Next(minSandCount, maxSandCount);
            for (int y = World.TerrainConfiguration.OceanPosition.StartPositionY; y > World.TerrainConfiguration.OceanPosition.StartPositionY - randomDownSandCount; y--)
            {
                World.CreateBlock(World.ObjectAtlas.Sand, x, y);
            }
        }

        //Pulverizing
        for (int y = startYPosition; y > World.TerrainConfiguration.OceanPosition.StartPositionY - 20; y--)
        {
            for (int x = startXPosition; x > startXPosition - lengthOfPulverizing; x--)
            {
                chancePulverize = World.RandomVar.Next(1, 4);
                if (chancePulverize % 3 == 0)
                {
                    World.CreateBlock(World.ObjectAtlas.Dirt, x, y);
                }
            }
            for (int x = startXPosition; x < startXPosition + lengthOfPulverizing; x++)
            {
                chancePulverize = World.RandomVar.Next(1, 4);
                if (chancePulverize % 3 == 0)
                {
                    World.CreateBlock(World.ObjectAtlas.Sand, x, y);
                }
            }
        }

        //Mountain generation
        startXPosition = World.TerrainConfiguration.DesertPosition.StartPositionX;
        for (int x = startXPosition; x < startXPosition + countOfDesertChunks * chunkSize; x++)
        {
            float height = startYPosition + Mathf.PerlinNoise(
                (x + World.Seed) * World.TerrainConfiguration.mountainFreq,
                World.Seed * World.TerrainConfiguration.mountainFreq) * World.TerrainConfiguration.heightDesertMountainMultiplier;
            for (int y = startYPosition; y < height; y++)
            {
                World.CreateBlock(World.ObjectAtlas.Sand, x, y);
            }
        }
    }

    //10% of all chunks (if world size = 84 chunks => Floor to 8)
    public void CreateOceanBiome()
    {
        int chunkSize = World.TerrainConfiguration.chunkSize;
        int countOfOceanChunks = World.TerrainConfiguration.CountOfOceanChunks;
        int startXPosition = World.TerrainConfiguration.OceanPosition.EndPositionX;
        int startYPosition = World.TerrainConfiguration.Equator;

        int leftSandOffset = 20;
        int randomDownSandCount = 0;
        int minSandCount = 20;
        int maxSandCount = 25;

        int downYOffset = 0;
        int chanceToMoveDown;
        int countOfStableYPosition = 0;
        for (int x = startXPosition; x > 5; x--)
        {
            for (int y = startYPosition; y > startYPosition - downYOffset; y--)
            {
                World.CreateBlock(World.ObjectAtlas.Water, x, y);
                _objectsData[x, y].CurrentFlowValue = 1f;
                World.TerrainConfiguration.OceanPosition.StartPositionY = y;
            }
            randomDownSandCount = World.RandomVar.Next(minSandCount, maxSandCount);
            for (int y = startYPosition - downYOffset; y > startYPosition - downYOffset - randomDownSandCount; y--)
            {
                World.CreateBlock(World.ObjectAtlas.Sand, x, y);
            }
            if (leftSandOffset == 0)
            {
                chanceToMoveDown = World.RandomVar.Next(0, 6);
                if (chanceToMoveDown % 5 == 0 || countOfStableYPosition == 6)
                {
                    downYOffset++;
                    countOfStableYPosition = 0;
                }
                else
                {
                    countOfStableYPosition++;
                }
            }
            else
            {
                leftSandOffset--;
            }
        }
    }
    #endregion

    public void GenerateClusters(TerrainLevel level)
    {
        foreach (Cluster cluster in level.Clusters)
        {
            int clusterAddSeed = World.RandomVar.Next(World.Seed, World.Seed + 1000);
            for (int x = 0; x < World.TerrainConfiguration.WorldWitdh; x++)
            {
                for (int y = level.startY; y <= level.endY; y++)
                {
                    if (GenerateNoise(x, y, cluster.freq, clusterAddSeed) > cluster.size &&
                        cluster.AllowedToSpawnOn.Find(b => b.GetID() == _objectsData[x, y].Id && b.blockType == _objectsData[x, y].Type) != null)  
                    {
                        World.CreateBlock(cluster.fillBlock, x, y);
                    }
                }
            }
        }
    }

    public void GenerateOres()
    {
        foreach (Ore ore in World.TerrainConfiguration.Ores)
        {
            int oreAddSeed = World.RandomVar.Next(World.Seed, World.Seed + 1000);
            for (int x = 0; x < World.TerrainConfiguration.WorldWitdh; x++)
            {
                for (int y = ore.MinHeightToSpawn; y < ore.MaxHeightToSpawn; y++)
                {
                    if (GenerateNoise(x, y, ore.Rarity, oreAddSeed) > ore.Size && 
                        ore.AllowedToSpawnOn.Find(b => b.GetID() == _objectsData[x, y].Id && b.blockType == _objectsData[x, y].Type) != null)
                    {
                        World.CreateBlock(ore.FillBlock, x, y);
                    }
                }
            }
        }
    }

    public void SmoothWorld()
    {
        Debug.Log("Start smooth world");
        var watch = System.Diagnostics.Stopwatch.StartNew();
        //Smooth biomes mountains
        int chunkSize = World.TerrainConfiguration.chunkSize;
        int equator = World.TerrainConfiguration.Equator;
        for (int x = 0; x < World.TerrainConfiguration.horizontalChunksCount * chunkSize - chunkSize; x += chunkSize)
        {
            if (World.Chunks[x / chunkSize, equator / chunkSize].BiomeID != World.Chunks[x / chunkSize + 1, equator / chunkSize].BiomeID)
            {
                int startX = x + chunkSize - 1;
                int startY = equator;
                SmoothMountains(startX, startY, 1);
                SmoothMountains(startX, startY, -1);
            }
        }

        for (int x = 1; x < _objectsData.GetLength(0) - 1; x++)
        {
            for (int y = 0; y < _objectsData.GetLength(1); y++)
            {
                if (_objectsData[x - 1, y].Type == ObjectType.Empty &&
                    _objectsData[x + 1, y].Type == ObjectType.Empty)
                {
                    //blocks[x, y] = empty;
                    World.CreateBlock(World.ObjectAtlas.Air, x, y);
                }
            }
        }
        watch.Stop();
        Debug.Log($"Smooth world complete: {watch.Elapsed.TotalSeconds}");
    }

    private void SmoothMountains(int startX, int startY, int direction)
    {
        while (true)
        {
            int inc = 1;
            if (_objectsData[startX + direction, startY + 1].Type == ObjectType.Empty)
            {
                break;
            }
            else
            {
                if (_objectsData[startX + 1, startY + 1 + inc].Type == ObjectType.Empty)
                {
                    startX += direction;
                    startY++;
                }
                else
                {
                    while (_objectsData[startX + 1, startY + 1 + inc].Type != ObjectType.Empty)
                    {
                        World.CreateBlock(World.ObjectAtlas.Air, startX + 1, startY + 1 + inc);
                        inc++;
                    }
                }
            }
        }
    }

    private void GrassSeeding()
    {
        int startX = World.TerrainConfiguration.PlainsPosition.StartPositionX;
        int startY = World.TerrainConfiguration.levels.Find(x => x.Type == TerrainLevelID.Surface).startY;
        int endX = World.TerrainConfiguration.WorldWitdh;
        int endY = World.TerrainConfiguration.levels.Find(x => x.Type == TerrainLevelID.Surface).endY;

        Chunk currentChunk;
        BlockSO block;
        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                currentChunk = World.Chunks[x / World.TerrainConfiguration.chunkSize, y / World.TerrainConfiguration.chunkSize];
                switch (currentChunk.BiomeID)
                {
                    case BiomesID.Plains:
                        {
                            block = World.ObjectAtlas.GetBlockByID(ObjectType.SolidBlock, SolidBlocksID.PlainsGrass);
                        }
                        break;
                    case BiomesID.Meadow:
                        {
                            block = World.ObjectAtlas.GetBlockByID(ObjectType.SolidBlock, SolidBlocksID.MeadowGrass);
                        }
                        break;
                    case BiomesID.Forest:
                        {
                            block = World.ObjectAtlas.GetBlockByID(ObjectType.SolidBlock, SolidBlocksID.ForestGrass);
                        }
                        break;
                    case BiomesID.Swamp:
                        {
                            block = World.ObjectAtlas.GetBlockByID(ObjectType.SolidBlock, SolidBlocksID.SwampGrass);
                        }
                        break;
                    case BiomesID.ConiferousForest:
                        {
                            block = World.ObjectAtlas.GetBlockByID(ObjectType.SolidBlock, SolidBlocksID.ConiferousForestGrass);
                        }
                        break;
                    default:
                        {
                            block = null;
                        }
                        break;
                }
                if (block != null && 
                    _objectsData[x, y + 1].Type == ObjectType.Empty &&
                    _objectsData[x, y].Type == (block as SolidBlockSO).AllowedForConvertation.blockType &&
                    _objectsData[x, y].Id == (block as SolidBlockSO).AllowedForConvertation.GetID()
                    ) 
                {
                    World.CreateBlock(block, x, y);
                }
            }
        }
    }

    #endregion

    #endregion
}