using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Color = UnityEngine.Color;

public class TerrainGeneration : MonoBehaviour
{
    #region Private fields
    private World _world;
    #endregion

    #region Properties

    #endregion

    #region Methods

    private void Awake()
    {
        _world = GameManager.Instance.World;
    }

    #region Helpful methods

    public float GenerateNoise(int x, int y, float noiseFreq, int additionalSeed = 0)
    {
        return Mathf.PerlinNoise((x + _world.Seed + additionalSeed)/ noiseFreq, (y + _world.Seed + additionalSeed)/ noiseFreq);
    }

    private List<ObjectData> GetCaveSize(int startX, int startY)
    {
        List<ObjectData> blocks = new List<ObjectData>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(startX, startY));
        GameManager.Instance.ObjectsData[startX, startY].IsChecked = true;

        while (queue.Count > 0)
        {
            Vector2Int position = queue.Dequeue();
            blocks.Add(GameManager.Instance.ObjectsData[position.x, position.y]);
            for (int x = position.x - 1; x <= position.x + 1; x++)
            {
                for (int y = position.y - 1; y <= position.y + 1; y++)
                {
                    if (_world.IsInMapRange(x, y) && (y == position.y || x == position.x))
                    {
                        if (!GameManager.Instance.ObjectsData[x, y].IsChecked && GameManager.Instance.ObjectsData[x, y].Type == ObjectType.Empty)
                        {
                            GameManager.Instance.ObjectsData[x, y].IsChecked = true;
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
                if (GameManager.Instance.ObjectsData[dx, dy].Type == ObjectType.Empty)
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
                if (GameManager.Instance.ObjectsData[dx, dy].Type == ObjectType.Empty)
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
            if (GameManager.Instance.ObjectsData[intPosition.x, intPosition.y].Type != ObjectType.Empty)
            {
                return false;
            }
        }
        foreach (var block in tree.FoliageBlocks)
        {
            Vector2 point = Tree.transform.TransformPoint(new Vector2(block.x, block.y));
            Vector3Int intPosition = Vector3Int.RoundToInt(point);
            if (GameManager.Instance.ObjectsData[intPosition.x, intPosition.y].Type != ObjectType.Empty)
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
        #region Terrain generation
        _world = GameManager.Instance.World;
        int currrentXLevel = 0;
        int currrentYLevel = 0;
        int chunkSize = _world.TerrainConfiguration.chunkSize;
        Debug.Log("Start terrain generation");
        var watch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < _world.TerrainConfiguration.horizontalChunksCount; i++)
        {
            currrentYLevel = 0;
            foreach (TerrainLevel level in _world.TerrainConfiguration.levels.AsEnumerable().Reverse())
            {
                for (int j = 0; j < level.chunkCount; j++)
                {
                    GenerateTerrain(new Vector2Int(currrentXLevel, currrentYLevel), level);
                    currrentYLevel += chunkSize;
                }
            }
            currrentXLevel += chunkSize;
        }
        watch.Stop();
        Debug.Log($"Terrain generation complete: {watch.Elapsed.TotalSeconds}");
        #endregion

        #region Ocean generation
        if (_world.TerrainConfiguration.EnableOceanGeneration)
        {
            Debug.Log("Start ocean biome generation");
            watch.Restart();
            CreateOceanBiome();
            _world.TerrainConfiguration.SetBiomesInChunks(GameManager.Instance.Chunks);
            watch.Stop();
            Debug.Log($"Ocean biome generation complete: {watch.Elapsed.TotalSeconds}");
        }
        #endregion

        #region Biomes generation
        if (_world.TerrainConfiguration.EnableBiomGeneration)
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
        #endregion

        #region Holes generation
        if (_world.TerrainConfiguration.EnableHolesGeneration)
        {
            currrentXLevel = 0;
            currrentYLevel = 0;
            Debug.Log("Start holes generation");
            watch.Restart();
            for (int i = 0; i < _world.TerrainConfiguration.horizontalChunksCount; i++)
            {
                currrentYLevel = 0;
                foreach (TerrainLevel level in _world.TerrainConfiguration.levels.AsEnumerable().Reverse())
                {
                    for (int j = 0; j < level.chunkCount; j++)
                    {
                        GenerateHoles(new Vector2Int(currrentXLevel, currrentYLevel), level.Holes);
                        currrentYLevel += chunkSize;
                    }
                }
                currrentXLevel += chunkSize;
            }
            Debug.Log($"Holes generation complete: {watch.Elapsed.TotalSeconds}");
        }
        #endregion

        #region Clusters generation
        if (_world.TerrainConfiguration.EnableClustersGenration)
        {
            Debug.Log("Start clusters generation");
            watch.Restart();
            foreach (TerrainLevel level in _world.TerrainConfiguration.levels)
            {
                GenerateClusters(level);
            }
            watch.Stop();
            Debug.Log($"Clusters generation complete: {watch.Elapsed.TotalSeconds}");
        }
        #endregion

        #region Caves generation
        if (_world.TerrainConfiguration.EnableCavesGeneration)
        {
            currrentXLevel = 0;
            currrentYLevel = 0;
            Debug.Log("Start caves generation");
            watch.Restart();
            for (int i = 0; i < _world.TerrainConfiguration.horizontalChunksCount; i++)
            {
                currrentYLevel = 0;
                foreach (TerrainLevel level in _world.TerrainConfiguration.levels.AsEnumerable().Reverse())
                {
                    for (int j = 0; j < level.chunkCount; j++)
                    {
                        Chunk chunk = GameManager.Instance.Chunks[currrentXLevel / chunkSize, currrentYLevel / chunkSize];
                        GenerateCaves(new Vector2Int(currrentXLevel, currrentYLevel), level, chunk);
                        currrentYLevel += chunkSize;
                    }
                }
                currrentXLevel += chunkSize;
            }
            foreach (TerrainLevel level in _world.TerrainConfiguration.levels.AsEnumerable().Reverse())
            {
                DeleteSmallCaves(level);
            }
            watch.Stop();
            Debug.Log($"Caves generation complete: {watch.Elapsed.TotalSeconds}");
        }
        #endregion

        #region Survivalist caves generation
        if (_world.TerrainConfiguration.EnableSurvivalistCavesGeneration)
        {
            Debug.Log("Start survivalist caves generation");
            watch.Restart();
            foreach (TerrainLevel level in _world.TerrainConfiguration.levels)
            {
                GenerateSurvivalistCave(level);
            }
            watch.Stop();
            Debug.Log($"Survivalist caves generation complete: {watch.Elapsed.TotalSeconds}");
        }
        #endregion

        #region Ore generation
        if (_world.TerrainConfiguration.EnableOreGeneration)
        {
            Debug.Log("Start ores generation");
            watch.Restart();
            GenerateOres();
            watch.Stop();
            Debug.Log($"Ores generation complete: {watch.Elapsed.TotalSeconds}");
        }
        #endregion

        #region Smooth world
        if (_world.TerrainConfiguration.EnableSmoothingWorld)
        {
            SmoothWorld();
        }
        #endregion

        #region Lakes generation
        if (_world.TerrainConfiguration.EnableLakesGeneration)
        {
            Debug.Log("Start lakes generation");
            watch.Restart();
            GenerateLakes();
            watch.Stop();
            Debug.Log($"Lakes generation complete: {watch.Elapsed.TotalSeconds}");
        }
        #endregion

        #region Tree generation
        if (_world.TerrainConfiguration.EnableTreeGeneration)
        {
            Debug.Log("Start tree genearion");
            watch.Restart();
            GenerateTrees();
            watch.Stop();
            Debug.Log($"Tree generation complete: {watch.Elapsed.TotalSeconds}");
        }
        #endregion

        #region Grass seeding
        GrassSeeding();
        #endregion

        #region Planting
        if (_world.TerrainConfiguration.EnablePlanting)
        {
            Debug.Log("Start planting");
            watch.Restart();
            foreach (TerrainLevel level in _world.TerrainConfiguration.levels)
            {
                PlantWorld(level);
            }
            watch.Stop();
            Debug.Log($"Planting complete: {watch.Elapsed.TotalSeconds}");
        }
        #endregion

        #region Structure generation
        if (_world.TerrainConfiguration.EnableStructureGeneration)
        {
            Debug.Log("Start structures genearion");
            watch.Restart();
            foreach (Structure structure in _world.TerrainConfiguration.Structures)
            {
                GenerateStructure(structure);
            }
            watch.Stop();
            Debug.Log($"Structures generation complete: {watch.Elapsed.TotalSeconds}");
        }
        #endregion
    }

    private void GenerateTrees()
    {
        int startX = 5;
        int startY = _world.TerrainConfiguration.levels.Find(x => x.Type == TerrainLevelID.Surface).startY;
        int endX = _world.TerrainConfiguration.WorldWitdh - 5;
        int endY = _world.TerrainConfiguration.levels.Find(x => x.Type == TerrainLevelID.Surface).endY;
        int chunkSize = _world.TerrainConfiguration.chunkSize;
        GameObject treesGameObject = GameManager.Instance.TreesSection;

        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                if (_world.IsInMapRange(x, y))
                {
                    Chunk currentChunk = GameManager.Instance.Chunks[x / chunkSize, y / chunkSize];
                    List<GameObject> trees = GameManager.Instance.WorldObjectAtlas.GetBiomeTreesByID(currentChunk.BiomeID);
                    if (GameManager.Instance.ObjectsData[x, y].Type != ObjectType.Empty &&
                        GameManager.Instance.ObjectsData[x, y + 1].Type == ObjectType.Empty)
                    {
                        if (trees != null && trees.Count != 0)
                        {
                            GameObject treeGameObject = trees[_world.RandomVar.Next(0, trees.Count)];
                            Tree tree = treeGameObject.GetComponent<Tree>();
                            bool validPlace = true;
                            for (int dx = x; dx < x + tree.WidthToSpawn; dx++)
                            {
                                if (!tree.AllowedToSpawnOn.Exists(b => b.blockType == GameManager.Instance.ObjectsData[dx,y].Type && b.GetID() == GameManager.Instance.ObjectsData[dx, y].Id) ||
                                    GameManager.Instance.ObjectsData[dx, y + 1].Type != ObjectType.Empty)
                                {
                                    validPlace = false;
                                    break;
                                }
                            }
                            if (validPlace)
                            {
                                GameObject createdTree = Instantiate(treeGameObject, new Vector3(x + tree.WidthToSpawn / 2f, y + 1), Quaternion.identity, treesGameObject.transform);
                                if (GenerateTree(createdTree))
                                {
                                    x += _world.TerrainConfiguration.MinDistanceTree;
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
        bool isValid = _world.IsInMapRange((int)tree.transform.position.x + tree.Width, (int)tree.transform.position.y);
        if (isValid && IsValidTree(treeGameObject))
        {
            foreach (var block in tree.TrunkBlocks)
            {
                Vector2 point = treeGameObject.transform.TransformPoint(new Vector2(block.x, block.y));
                Vector3Int intPosition = Vector3Int.RoundToInt(point);
                GameManager.Instance.ObjectsData[intPosition.x, intPosition.y].IsTreeTrunk = true;
            }
            foreach (var block in tree.FoliageBlocks)
            {
                Vector2 point = treeGameObject.transform.TransformPoint(new Vector2(block.x, block.y));
                Vector3Int intPosition = Vector3Int.RoundToInt(point);
                GameManager.Instance.ObjectsData[intPosition.x, intPosition.y].IsTreeFoliage = true;
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
        int startX = _world.TerrainConfiguration.PlainsPosition.EndPositionX;
        for (int x = startX; x < _world.TerrainConfiguration.WorldWitdh - 100; x++)
        {
            startY = _world.TerrainConfiguration.Equator + 1;
            currentObjectData = GameManager.Instance.ObjectsData[x, startY];
            while (GameManager.Instance.ObjectsData[x, currentObjectData.Position.y + 1].Type != ObjectType.Empty)
            {
                currentObjectData = GameManager.Instance.ObjectsData[x, currentObjectData.Position.y + 1];
            }
            if (GameManager.Instance.ObjectsData[x, currentObjectData.Position.y - 1].Type != ObjectType.Empty)
            {
                int id = _world.RandomVar.Next(0, 10);
                bool isChecked = false;
                bool isValid = false;
                int lakeWidth = _world.RandomVar.Next(40,100);
                int lakeHeight = 0;
                Queue<ObjectData> queue = new Queue<ObjectData>();
                for (int dx = x; dx < x + lakeWidth; dx++)
                {
                    int randY = _world.RandomVar.Next(0, 2);
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
                        queue.Enqueue(GameManager.Instance.ObjectsData[dx, dy]);
                    }
                }
                if (isChecked && isValid)
                {
                    x += lakeWidth + 105;
                    if (_world.RandomVar.Next(0, 101) <= _world.TerrainConfiguration.ChanceToSpawnLake)
                    {
                        while (queue.Count != 0)
                        {
                            ObjectData objectData = queue.Dequeue();
                            _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.Water, objectData.Position.x, objectData.Position.y);
                            GameManager.Instance.ObjectsData[objectData.Position.x, objectData.Position.y].CurrentFlowValue = 1f;
                        }
                        for (int dx = currentObjectData.Position.x; dx < currentObjectData.Position.x + lakeWidth; dx++)
                        {
                            int dy = currentObjectData.Position.y;
                            while (GameManager.Instance.ObjectsData[dx, dy].Type != ObjectType.Empty) 
                            {
                                _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.Air, dx, dy);
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
        startX = _world.TerrainConfiguration.DesertPosition.StartPositionX;
        int endX = _world.TerrainConfiguration.DesertPosition.EndPositionX;
        startY = _world.TerrainConfiguration.Equator + 1;
        for (int x = startX + 50; x < endX - 50; x++)
        {
            currentObjectData = GameManager.Instance.ObjectsData[x, startY];
            while (GameManager.Instance.ObjectsData[x, currentObjectData.Position.y + 1].Type != ObjectType.Empty)
            {
                currentObjectData = GameManager.Instance.ObjectsData[x, currentObjectData.Position.y + 1];
            }
            int oasisWidth = _world.RandomVar.Next(20, 41);
            int oasisHeight = _world.RandomVar.Next(20, 31);
            if (IsValidLake(currentObjectData.Position.x, currentObjectData.Position.y, oasisWidth, oasisHeight))
            {
                if (_world.RandomVar.Next(0, 101) <= _world.TerrainConfiguration.ChanceToSpawnOasis)
                {
                    for (int dx = currentObjectData.Position.x; dx < currentObjectData.Position.x + oasisWidth; dx++)
                    {
                        for (int dy = currentObjectData.Position.y - oasisHeight; dy <= currentObjectData.Position.y; dy++)
                        {
                            if (currentObjectData.Position.y - dy <= Ellipse(dx - (currentObjectData.Position.x + oasisWidth / 2), oasisWidth / 2, oasisHeight / 2))
                            {
                                _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.Water, dx, dy);
                                GameManager.Instance.ObjectsData[dx, dy].CurrentFlowValue = 1f;
                            }
                        }
                    }
                    for (int dx = currentObjectData.Position.x; dx < currentObjectData.Position.x + oasisWidth; dx++)
                    {
                        int dy = currentObjectData.Position.y;
                        while (GameManager.Instance.ObjectsData[dx, dy].Type != ObjectType.Empty)
                        {
                            _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.Air, dx, dy);
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
                _world.CreateBlock(block, x, y);
            }
        }
    }

    private void PlantWorld(TerrainLevel level)
    {
        //Plant vines
        PlantSO vine = (GameManager.Instance.WorldObjectAtlas.Vine as PlantSO);
        if (vine.levelID == level.Type)
        {
            for (int x = _world.TerrainConfiguration.DesertPosition.EndPositionX + 1; x < _world.TerrainConfiguration.WorldWitdh; x++)
            {
                for (int y = level.startY; y < level.endY; y++)
                {
                    if (_world.RandomVar.Next(0, 101) <= vine.ChanceToSpawn &&
                        vine.AllowedToSpawnOn.ToList().Find(b => b.blockType == GameManager.Instance.ObjectsData[x, y + 1].Type && b.GetID() == GameManager.Instance.ObjectsData[x, y + 1].Id) &&
                        GameManager.Instance.ObjectsData[x, y].Type == ObjectType.Empty)
                    {
                        _world.CreateBlock(vine, x, y);
                    }
                }
            }
        }

        //Biomes specified plants
        List<List<BlockSO>> biomeSpecifiedPlants = new List<List<BlockSO>>
        {
            GameManager.Instance.WorldObjectAtlas.GetBiomesBlocks(ObjectType.Plant, BiomesID.Desert),
            GameManager.Instance.WorldObjectAtlas.GetBiomesBlocks(ObjectType.Plant, BiomesID.Plains),
            GameManager.Instance.WorldObjectAtlas.GetBiomesBlocks(ObjectType.Plant, BiomesID.Meadow),
            GameManager.Instance.WorldObjectAtlas.GetBiomesBlocks(ObjectType.Plant, BiomesID.Forest),
            GameManager.Instance.WorldObjectAtlas.GetBiomesBlocks(ObjectType.Plant, BiomesID.Swamp),
            GameManager.Instance.WorldObjectAtlas.GetBiomesBlocks(ObjectType.Plant, BiomesID.ConiferousForest),
        };

        foreach (var biomePlants in biomeSpecifiedPlants)
        {
            foreach (PlantSO plant in biomePlants)
            {
                for (int x = 0; x < _world.TerrainConfiguration.WorldWitdh; x++)
                {
                    for (int y = level.startY; y < level.endY; y++)
                    {
                        if (plant.levelID == level.Type &&
                            GameManager.Instance.Chunks[x/_world.TerrainConfiguration.chunkSize, y / _world.TerrainConfiguration.chunkSize].BiomeID == plant.BiomeID &&
                            _world.RandomVar.Next(0, 101) <= plant.ChanceToSpawn &&
                            plant.AllowedToSpawnOn.ToList().Find(b => b.blockType == GameManager.Instance.ObjectsData[x, y - 1].Type && b.GetID() == GameManager.Instance.ObjectsData[x, y - 1].Id) &&
                            GameManager.Instance.ObjectsData[x, y].Type == ObjectType.Empty)
                        {
                            _world.CreateBlock(plant, x, y);
                        }
                    }
                }
            }
        }
    }
    
    public void GenerateTerrain(Vector2Int startPosition, TerrainLevel level)
    {
        float height;
        int chunkSize = _world.TerrainConfiguration.chunkSize;

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
                _world.CreateBlock(level.defaultBlock, x, y);
            }
        }
    }
    
    public void GenerateHoles(Vector2Int startPosition, List<Hole> holes)
    {
        int chunkSize = _world.TerrainConfiguration.chunkSize;
        foreach (Hole hole in holes)
        {
            for (int x = startPosition.x; x < startPosition.x + chunkSize; x++)
            {
                for (int y = startPosition.y; y < startPosition.y + chunkSize; y++)
                {
                    if (GenerateNoise(x, y, hole.freq) > hole.size)
                    {
                        _world.CreateBlock(hole.fillBlock, x, y);
                        if (hole.fillBlock.blockType == ObjectType.LiquidBlock)
                        {
                            GameManager.Instance.ObjectsData[x, y].CurrentFlowValue = 1f;
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
            for (int x = startPosition.x; x < startPosition.x + _world.TerrainConfiguration.chunkSize; x++)
            {
                for (int y = startPosition.y; y < startPosition.y + _world.TerrainConfiguration.chunkSize; y++)
                {
                    if (GenerateNoise(x, y, level.caveFreq) > level.caveSize)
                    {
                        _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.Air, x, y, GameManager.Instance.WorldObjectAtlas.GetBlockByID(GameManager.Instance.ObjectsData[x,y].Type, GameManager.Instance.ObjectsData[x,y].Id));
                    }
                }
            }
        }
    }

    public void GenerateSurvivalistCave(TerrainLevel level)
    {
        if (level.Type == TerrainLevelID.Surface)
        {
            int startX = _world.TerrainConfiguration.DesertPosition.EndPositionX + 51;
            while (startX + 150 <= _world.TerrainConfiguration.WorldWitdh)
            {
                int xOffset = startX + 100;
                int x = _world.RandomVar.Next(startX, xOffset);
                int y = _world.RandomVar.Next(level.startY + 20, _world.TerrainConfiguration.Equator - 10);
                int randWitdh = _world.RandomVar.Next(20, 40);
                int randHeight = _world.RandomVar.Next(5, 10);

                if (_world.RandomVar.Next(1,101) <= _world.TerrainConfiguration.chanceToSpawnSurvivalistCave && IsValidSurvivalistCave(x, y, randWitdh, randHeight))
                {
                    int direction = _world.RandomVar.Next(0, 2);
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
                                if (_world.RandomVar.Next(0, 2) == 1)
                                {
                                    _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.Air, dx, dy - 1);
                                }
                            }
                            //Noise left or right side
                            //If left side
                            if (direction == 0)
                            {
                                if (dx == x - randWitdh / 2 && _world.RandomVar.Next(0, 2) == 1)
                                {
                                    _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.Air, dx - 1, dy);
                                }
                            }
                            //If right side
                            else
                            {
                                if (dx == x + randWitdh / 2 && _world.RandomVar.Next(0, 2) == 1)
                                {
                                    _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.Air, dx + 1, dy);
                                }
                            }
                            //Noise top side
                            if (dy == y + randHeight / 2 - 1)
                            {
                                if (_world.RandomVar.Next(0, 2) == 1)
                                {
                                    for (int i = 1; i <= _world.RandomVar.Next(1, 4); i++)
                                    {
                                        _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.Air, dx, dy + i);
                                    }
                                }
                            }
                            _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.Air, dx, dy);
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
            if (_world.RandomVar.Next(0, 2) == 1)
            {
                y++;
            }
            if (GameManager.Instance.ObjectsData[x, y].Type == ObjectType.Empty)
            {
                break;
            }
        }

        void DeletePillar(int x, int y, int height)
        {
            for (int i = 0; i <= height; i++)
            {
                _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.Air, x, y + i);
            }
        }
    }

    private void DeleteSmallCaves(TerrainLevel level)
    {
        List<ObjectData> blocksToDelete = new List<ObjectData>();
        if (level.caveSize != 0)
        {
            for (int x = 0; x < _world.TerrainConfiguration.WorldWitdh; x++)
            {
                for (int y = level.startY; y <= level.endY; y++)
                {
                    int startX = x;
                    int startY = y;
                    if (GameManager.Instance.ObjectsData[x, y].Type == ObjectType.Empty && !GameManager.Instance.ObjectsData[x, y].IsChecked)
                    {
                        blocksToDelete.Clear();
                        blocksToDelete = GetCaveSize(startX, startY);
                        if (blocksToDelete.Count < level.MinGeneratedCaveSize)
                        {
                            for (int i = 0; i < blocksToDelete.Count; i++)
                            {
                                int dx = blocksToDelete[i].Position.x;
                                int dy = blocksToDelete[i].Position.y;
                                BlockSO block = GameManager.Instance.WorldObjectAtlas.GetBlockByID(GameManager.Instance.ObjectsData[dx, dy].TypeBackground, GameManager.Instance.ObjectsData[dx,dy].IdBackground);
                                _world.CreateBlock(block, dx, dy);
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
        int chunkSize = _world.TerrainConfiguration.chunkSize;
        int countOfMeadowChunks = _world.TerrainConfiguration.CountOfMeadowChunks;
        int startXPosition = _world.TerrainConfiguration.MeadowPosition.StartPositionX;
        int startYPosition = _world.TerrainConfiguration.Equator;

        for (int x = startXPosition - 1; x < startXPosition + countOfMeadowChunks * chunkSize; x++)
        {
            float height = startYPosition + Mathf.PerlinNoise(
                (x + _world.Seed) * _world.TerrainConfiguration.mountainFreq,
                _world.Seed * _world.TerrainConfiguration.mountainFreq) * _world.TerrainConfiguration.heightMeadowMountainMultiplier;
            for (int y = startYPosition; y < height; y++)
            {
                _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.Dirt, x, y);
            }
        }
    }

    //10% of all chunks (if world size = 84 chunks => Round to 9)
    public void CreateForestBiome()
    {
        int chunkSize = _world.TerrainConfiguration.chunkSize;
        int countOfForestChunks = _world.TerrainConfiguration.CountOfForestChunks;
        int startXPosition = _world.TerrainConfiguration.ForestPosition.StartPositionX;
        int startYPosition = _world.TerrainConfiguration.Equator;

        for (int x = startXPosition; x < startXPosition + countOfForestChunks * chunkSize; x++)
        {
            float height = startYPosition + Mathf.PerlinNoise(
                (x + _world.Seed) * _world.TerrainConfiguration.mountainFreq,
                _world.Seed * _world.TerrainConfiguration.mountainFreq) * _world.TerrainConfiguration.heightMeadowMountainMultiplier;
            for (int y = startYPosition; y < height; y++)
            {
                _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.Dirt, x, y);
            }
        }
    }

    //10% of all chunks (if world size = 84 chunks => Floor to 8)
    public void CreateSwampBiome()
    {
        int chunkSize = _world.TerrainConfiguration.chunkSize;
        int countOfSwampChunks = _world.TerrainConfiguration.CountOfSwampChunks;
        int startXPosition = _world.TerrainConfiguration.SwampPosition.StartPositionX;
        int startYPosition = _world.TerrainConfiguration.Equator;

        for (int x = startXPosition; x < startXPosition + countOfSwampChunks * chunkSize; x++)
        {
            float height = startYPosition + Mathf.PerlinNoise(
                (x + _world.Seed) * _world.TerrainConfiguration.mountainFreq,
                _world.Seed * _world.TerrainConfiguration.mountainFreq) * _world.TerrainConfiguration.heightMeadowMountainMultiplier;
            for (int y = startYPosition; y < height; y++)
            {
                _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.Dirt, x, y);
            }
        }
    }

    //10% of all chunks (if world size = 84 chunks => Floor to 8)
    public void CreateConiferousForestBiome()
    {
        int chunkSize = _world.TerrainConfiguration.chunkSize;
        int countOfConiferousForestChunks = _world.TerrainConfiguration.CountOfConiferousForestChunks;
        int startXPosition = _world.TerrainConfiguration.ConiferousForestPosition.StartPositionX;
        int startYPosition = _world.TerrainConfiguration.Equator;

        for (int x = startXPosition; x < startXPosition + countOfConiferousForestChunks * chunkSize - 1; x++)
        {
            float height = startYPosition + Mathf.PerlinNoise(
                (x + _world.Seed) * _world.TerrainConfiguration.mountainFreq,
                _world.Seed * _world.TerrainConfiguration.mountainFreq) * _world.TerrainConfiguration.heightMeadowMountainMultiplier;
            for (int y = startYPosition; y < height; y++)
            {
                _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.Dirt, x, y);
            }
        }
    }

    //5% of all chunks (if world size = 84 chunks => Floor to 4)
    public void CreatePlainsBiome()
    {
        int chunkSize = _world.TerrainConfiguration.chunkSize;
        int countOfPlainsChunks = _world.TerrainConfiguration.CountOfPlainsChunks;
        int startXPosition = _world.TerrainConfiguration.PlainsPosition.StartPositionX;
        int startYPosition = _world.TerrainConfiguration.Equator;

        for (int x = startXPosition; x < startXPosition + countOfPlainsChunks * chunkSize; x++)
        {
            float height = startYPosition + Mathf.PerlinNoise(
                (x + _world.Seed) * _world.TerrainConfiguration.mountainFreq,
                _world.Seed * _world.TerrainConfiguration.mountainFreq) * _world.TerrainConfiguration.heightPlainsMountainMultiplier;
            for (int y = startYPosition; y < height; y++)
            {
                _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.Dirt, x, y);
            }
        }
    }

    //25% of all chunks (if world size = 84 chunks => Round to 21)
    public void CreateDesertBiome()
    {
        int chunkSize = _world.TerrainConfiguration.chunkSize;
        int countOfDesertChunks = _world.TerrainConfiguration.CountOfDesertChunks;
        int startXPosition = _world.TerrainConfiguration.DesertPosition.EndPositionX;
        int startYPosition = _world.TerrainConfiguration.Equator;

        int randomDownSandCount = 0;
        int minSandCount = 20;
        int maxSandCount = 25;

        int chancePulverize;
        int lengthOfPulverizing = 10;

        for (int x = startXPosition; x > 5; x--)
        {
            for (int y = startYPosition; y > _world.TerrainConfiguration.OceanPosition.StartPositionY; y--)
            {
                if (GameManager.Instance.ObjectsData[x, y].Type != ObjectType.LiquidBlock)
                {
                    _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.Sand, x, y);
                }
            }
            randomDownSandCount = _world.RandomVar.Next(minSandCount, maxSandCount);
            for (int y = _world.TerrainConfiguration.OceanPosition.StartPositionY; y > _world.TerrainConfiguration.OceanPosition.StartPositionY - randomDownSandCount; y--)
            {
                _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.Sand, x, y);
            }
        }

        //Pulverizing
        for (int y = startYPosition; y > _world.TerrainConfiguration.OceanPosition.StartPositionY - 20; y--)
        {
            for (int x = startXPosition; x > startXPosition - lengthOfPulverizing; x--)
            {
                chancePulverize = _world.RandomVar.Next(1, 4);
                if (chancePulverize % 3 == 0)
                {
                    _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.Dirt, x, y);
                }
            }
            for (int x = startXPosition; x < startXPosition + lengthOfPulverizing; x++)
            {
                chancePulverize = _world.RandomVar.Next(1, 4);
                if (chancePulverize % 3 == 0)
                {
                    _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.Sand, x, y);
                }
            }
        }

        //Mountain generation
        startXPosition = _world.TerrainConfiguration.DesertPosition.StartPositionX;
        for (int x = startXPosition; x < startXPosition + countOfDesertChunks * chunkSize; x++)
        {
            float height = startYPosition + Mathf.PerlinNoise(
                (x + _world.Seed) * _world.TerrainConfiguration.mountainFreq,
                _world.Seed * _world.TerrainConfiguration.mountainFreq) * _world.TerrainConfiguration.heightDesertMountainMultiplier;
            for (int y = startYPosition; y < height; y++)
            {
                _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.Sand, x, y);
            }
        }
    }

    //10% of all chunks (if world size = 84 chunks => Floor to 8)
    public void CreateOceanBiome()
    {
        int chunkSize = _world.TerrainConfiguration.chunkSize;
        int countOfOceanChunks = _world.TerrainConfiguration.CountOfOceanChunks;
        int startXPosition = _world.TerrainConfiguration.OceanPosition.EndPositionX;
        int startYPosition = _world.TerrainConfiguration.Equator;

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
                _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.Water, x, y);
                GameManager.Instance.ObjectsData[x, y].CurrentFlowValue = 1f;
                _world.TerrainConfiguration.OceanPosition.StartPositionY = y;
            }
            randomDownSandCount = _world.RandomVar.Next(minSandCount, maxSandCount);
            for (int y = startYPosition - downYOffset; y > startYPosition - downYOffset - randomDownSandCount; y--)
            {
                _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.Sand, x, y);
            }
            if (leftSandOffset == 0)
            {
                chanceToMoveDown = _world.RandomVar.Next(0, 6);
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
            int clusterAddSeed = _world.RandomVar.Next(_world.Seed, _world.Seed + 1000);
            for (int x = 0; x < _world.TerrainConfiguration.WorldWitdh; x++)
            {
                for (int y = level.startY; y <= level.endY; y++)
                {
                    if (GenerateNoise(x, y, cluster.freq, clusterAddSeed) > cluster.size &&
                        cluster.AllowedToSpawnOn.Find(b => b.GetID() == GameManager.Instance.ObjectsData[x, y].Id && b.blockType == GameManager.Instance.ObjectsData[x, y].Type) != null)  
                    {
                        _world.CreateBlock(cluster.fillBlock, x, y);
                    }
                }
            }
        }
    }

    public void GenerateOres()
    {
        foreach (Ore ore in _world.TerrainConfiguration.Ores)
        {
            int oreAddSeed = _world.RandomVar.Next(_world.Seed, _world.Seed + 1000);
            for (int x = 0; x < _world.TerrainConfiguration.WorldWitdh; x++)
            {
                for (int y = ore.MinHeightToSpawn; y < ore.MaxHeightToSpawn; y++)
                {
                    if (GenerateNoise(x, y, ore.Rarity, oreAddSeed) > ore.Size && 
                        ore.AllowedToSpawnOn.Find(b => b.GetID() == GameManager.Instance.ObjectsData[x, y].Id && b.blockType == GameManager.Instance.ObjectsData[x, y].Type) != null)
                    {
                        _world.CreateBlock(ore.FillBlock, x, y);
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
        int chunkSize = _world.TerrainConfiguration.chunkSize;
        int equator = _world.TerrainConfiguration.Equator;
        for (int x = 0; x < _world.TerrainConfiguration.horizontalChunksCount * chunkSize - chunkSize; x += chunkSize)
        {
            if (GameManager.Instance.Chunks[x / chunkSize, equator / chunkSize].BiomeID != GameManager.Instance.Chunks[x / chunkSize + 1, equator / chunkSize].BiomeID)
            {
                int startX = x + chunkSize - 1;
                int startY = equator;
                SmoothMountains(startX, startY, 1);
                SmoothMountains(startX, startY, -1);
            }
        }

        for (int x = 1; x < GameManager.Instance.ObjectsData.GetLength(0) - 1; x++)
        {
            for (int y = 0; y < GameManager.Instance.ObjectsData.GetLength(1); y++)
            {
                if (GameManager.Instance.ObjectsData[x - 1, y].Type == ObjectType.Empty &&
                    GameManager.Instance.ObjectsData[x + 1, y].Type == ObjectType.Empty)
                {
                    //blocks[x, y] = empty;
                    _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.Air, x, y);
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
            if (GameManager.Instance.ObjectsData[startX + direction, startY + 1].Type == ObjectType.Empty)
            {
                break;
            }
            else
            {
                if (GameManager.Instance.ObjectsData[startX + 1, startY + 1 + inc].Type == ObjectType.Empty)
                {
                    startX += direction;
                    startY++;
                }
                else
                {
                    while (GameManager.Instance.ObjectsData[startX + 1, startY + 1 + inc].Type != ObjectType.Empty)
                    {
                        _world.CreateBlock(GameManager.Instance.WorldObjectAtlas.Air, startX + 1, startY + 1 + inc);
                        inc++;
                    }
                }
            }
        }
    }

    private void GrassSeeding()
    {
        int startX = _world.TerrainConfiguration.PlainsPosition.StartPositionX;
        int startY = _world.TerrainConfiguration.levels.Find(x => x.Type == TerrainLevelID.Surface).startY;
        int endX = _world.TerrainConfiguration.WorldWitdh;
        int endY = _world.TerrainConfiguration.levels.Find(x => x.Type == TerrainLevelID.Surface).endY;

        Chunk currentChunk;
        BlockSO block;
        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                currentChunk = GameManager.Instance.Chunks[x / _world.TerrainConfiguration.chunkSize, y / _world.TerrainConfiguration.chunkSize];
                switch (currentChunk.BiomeID)
                {
                    case BiomesID.Plains:
                        {
                            block = GameManager.Instance.WorldObjectAtlas.GetBlockByID(ObjectType.SolidBlock, SolidBlocksID.PlainsGrass);
                        }
                        break;
                    case BiomesID.Meadow:
                        {
                            block = GameManager.Instance.WorldObjectAtlas.GetBlockByID(ObjectType.SolidBlock, SolidBlocksID.MeadowGrass);
                        }
                        break;
                    case BiomesID.Forest:
                        {
                            block = GameManager.Instance.WorldObjectAtlas.GetBlockByID(ObjectType.SolidBlock, SolidBlocksID.ForestGrass);
                        }
                        break;
                    case BiomesID.Swamp:
                        {
                            block = GameManager.Instance.WorldObjectAtlas.GetBlockByID(ObjectType.SolidBlock, SolidBlocksID.SwampGrass);
                        }
                        break;
                    case BiomesID.ConiferousForest:
                        {
                            block = GameManager.Instance.WorldObjectAtlas.GetBlockByID(ObjectType.SolidBlock, SolidBlocksID.ConiferousForestGrass);
                        }
                        break;
                    default:
                        {
                            block = null;
                        }
                        break;
                }
                if (block != null && 
                    GameManager.Instance.ObjectsData[x, y + 1].Type == ObjectType.Empty &&
                    GameManager.Instance.ObjectsData[x, y].Type == (block as SolidBlockSO).AllowedForConvertation.blockType &&
                    GameManager.Instance.ObjectsData[x, y].Id == (block as SolidBlockSO).AllowedForConvertation.GetID()
                    ) 
                {
                    _world.CreateBlock(block, x, y);
                }
            }
        }
    }
    #endregion

    #endregion
}