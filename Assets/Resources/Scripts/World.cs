using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

public class World : MonoBehaviour
{
    #region Private fields
    //For generation
    [SerializeField] private TerrainConfiguration _terrainConfiguration;
    [SerializeField] private int _seed;
    private static bool _isGameLoaded;
    private GameObject _terrainGenerationObject;
    private System.Random _randomVar;
    [SerializeField] private bool _isLevelFullyCreated;

    //Threads
    private Thread _updateBlocksDataThread;
    private Thread _randomUpdateBlocksDataThread;

    //Flags
    private bool _isGameClosed;

    //Liquid (test)
    private float _maxValue = 1.0f;
    private float _minValue = 0.001f;

    public float _time = 3f;
    #endregion

    #region Public fields
    [Header("Tilemap Properties")]
    public Tilemap BlockTilemap;
    public Tilemap BlockBackgroundTilemap;
    public Tilemap LiquidBackgroundTilemap;
    public Tilemap PlantTilemap;
    public Texture2D WorldMap;

    [Header("For Physics")]
    public Tilemap SolidBlocksTilemap;
    public TileBase SolidBlocksRuleTile;

    #endregion

    #region Properties
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

    public TerrainConfiguration TerrainConfiguration
    {
        get
        {
            if (_terrainConfiguration == null)
            {
                throw new NullReferenceException("Terrain configuration is NULL");
            }
            return _terrainConfiguration;
        }

        set
        {
            _terrainConfiguration = value;
        }
    }

    public bool IsGameClosed
    {
        get
        {
            return _isGameClosed;
        }

        set
        {
            _isGameClosed = value;
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

    public TerrainGeneration TerrainGeneration
    {
        get
        {
            return _terrainGenerationObject.GetComponent<TerrainGeneration>();
        }
    }

    public bool IsLevelFullyCreated
    {
        get
        {
            return _isLevelFullyCreated;
        }

        set
        {
            _isLevelFullyCreated = value;
        }
    }
    #endregion

    #region Methods

    #region General methods
    private void OnApplicationQuit()
    {
        _updateBlocksDataThread.Abort();
        _randomUpdateBlocksDataThread.Abort();
        IsGameClosed = true;
        Destroy(WorldMap);
    }

    private void Awake()
    {
        _terrainGenerationObject = gameObject.transform.Find("TerrainGeneration").gameObject;
        IsLevelFullyCreated = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            FillMap();
        }
    }

    public void CreateNewWorld()
    {
        try
        {
            Seed = new System.Random().Next(-10000, 10000);
            RandomVar = new System.Random(Seed.GetHashCode());
            TerrainGeneration.Generation();
            LoadCoroutinsAndThreads();
        }
        catch (Exception c)
        {
            Debug.LogError(c);
            File.WriteAllText(Application.dataPath + "/Output.txt", c.ToString());
        }
    }

    public void LoadWorld()
    {
        try
        {
            GameManager.Instance.SaveLoadManager.LoadAllData();
            LoadCoroutinsAndThreads();
        }
        catch (Exception c)
        {
            Debug.LogError(c);
            File.WriteAllText(Application.dataPath + "/Output.txt", c.ToString());
        }
    }

    private void LoadCoroutinsAndThreads()
    {
        StartCoroutine(UpdateScreen());
        _updateBlocksDataThread = new Thread(UpdateBlocksData);
        _updateBlocksDataThread.Start();

        _randomUpdateBlocksDataThread = new Thread(RandomUpdateBlocksData);
        _randomUpdateBlocksDataThread.Start();
    }

    public void FillMap()
    {
        WorldMap = new Texture2D(TerrainConfiguration.WorldWitdh, TerrainConfiguration.WorldHeight);
        for (int x = 0; x < TerrainConfiguration.WorldWitdh; x++)
        {
            for (int y = 0; y < TerrainConfiguration.WorldHeight; y++)
            {
                if (GameManager.Instance.ObjectsData[x, y].Type == ObjectType.Empty)
                {
                    if (GameManager.Instance.ObjectsData[x, y].IsTreeTrunk)
                    {
                        Color trunkColor = new Color(100f / 255f, 60f / 255f, 0);
                        WorldMap.SetPixel(x, y, trunkColor);
                        continue;
                    }
                    Color backgroundColor = new Color(GameManager.Instance.ObjectsData[x, y].BackgroundColorOnMap.r - 0.2f, GameManager.Instance.ObjectsData[x, y].BackgroundColorOnMap.g - 0.2f, GameManager.Instance.ObjectsData[x, y].BackgroundColorOnMap.b - 0.2f);
                    WorldMap.SetPixel(x, y, backgroundColor);
                    continue;
                }
                WorldMap.SetPixel(x, y, GameManager.Instance.ObjectsData[x, y].ColorOnMap);
            }
        }
        WorldMap.Apply();
        byte[] bytes = WorldMap.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/SavedScreen.png", bytes);
    }
    #endregion

    #region Helpful methods

    #region General methods
    public void CreateBlock(BlockSO block, int x, int y, BlockSO backgroundBlock = null)
    {
        TileBase tile = block.tiles.Count != 0 ? block.tiles[RandomVar.Next(0, block.tiles.Count)] : null;
        GameManager.Instance.ObjectsData[x, y].Id = block.GetID();
        GameManager.Instance.ObjectsData[x, y].CurrentTile = tile;
        GameManager.Instance.ObjectsData[x, y].Type = block.blockType;
        GameManager.Instance.ObjectsData[x, y].ColorOnMap = block.colorOnMap;
        GameManager.Instance.ObjectsData[x, y].CurrentLiquidBackgroundTile = null;
        GameManager.Instance.ObjectsData[x, y].Durability = block.Durability;
        if (backgroundBlock != null)
        {
            tile = block.tiles.Count != 0 ? backgroundBlock.tiles[RandomVar.Next(0, block.tiles.Count)] : null;
            GameManager.Instance.ObjectsData[x, y].IdBackground = backgroundBlock.GetID();
            GameManager.Instance.ObjectsData[x, y].CurrentBackgroundTile = backgroundBlock.tiles[RandomVar.Next(0, backgroundBlock.tiles.Count)]; ;
            GameManager.Instance.ObjectsData[x, y].TypeBackground = backgroundBlock.blockType;
            GameManager.Instance.ObjectsData[x, y].BackgroundColorOnMap = backgroundBlock.colorOnMap;
        }

        if (block.blockType == ObjectType.Plant)
        {
            GameManager.Instance.ObjectsData[x, y].ChanceToGrow = (block as PlantSO).ChanceToGrow;
        }
    }

    public bool DecreaseDurability(Vector2Int position, float durrabilityDecrease)
    {
        GameManager.Instance.ObjectsData[position.x, position.y].Durability -= durrabilityDecrease;

        if (GameManager.Instance.ObjectsData[position.x, position.y].Durability <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public ObjectData GetBlockByPosition(Vector3Int intPosition)
    {
        return GameManager.Instance.ObjectsData[intPosition.x, intPosition.y];
    }

    private RectInt GetCameraRectInt()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        float cameraHeight = Camera.main.orthographicSize * 2 + 5;
        float cameraWidth = cameraHeight * Camera.main.aspect + 5;
        Vector3 cameraSize = new Vector3(cameraWidth, cameraHeight, 0);
        RectInt cameraBounds = new RectInt(
            Vector2Int.FloorToInt(cameraPosition) - Vector2Int.FloorToInt(cameraSize / 2),
            Vector2Int.FloorToInt(cameraSize)
        );
        return cameraBounds;
    }
    #endregion

    #region Liquid block methods
    private void PaintLiquid(ObjectData block)
    {
        LiquidBlockSO liquidBlock = GameManager.Instance.WorldObjectAtlas.GetBlockByID(ObjectType.LiquidBlock, block.Id) as LiquidBlockSO;
        block.CurrentTile = GetTextureByFlowValue(block.CurrentFlowValue, liquidBlock.liquidFlowValueTiles);
        if (block.IsPsevdoFull || block.IsAboveNotEmpty)
        {
            block.CurrentTile = liquidBlock.liquidFlowValueTiles[10];
        }
    }

    private TileBase GetTextureByFlowValue(float flowValue, TileBase[] tiles)
    {
        if (flowValue > 0f && flowValue <= 0.1f)
        {
            return tiles[0];
        }
        if (flowValue > 0.1f && flowValue <= 0.2f)
        {
            return tiles[1];
        }
        if (flowValue > 0.2f && flowValue <= 0.3f)
        {
            return tiles[2];
        }
        if (flowValue > 0.3f && flowValue <= 0.4f)
        {
            return tiles[3];
        }
        if (flowValue > 0.4f && flowValue <= 0.5f)
        {
            return tiles[4];
        }
        if (flowValue > 0.5f && flowValue <= 0.6f)
        {
            return tiles[5];
        }
        if (flowValue > 0.6f && flowValue <= 0.7f)
        {
            return tiles[6];
        }
        if (flowValue > 0.7f && flowValue <= 0.8f)
        {
            return tiles[7];
        }
        if (flowValue > 0.8f && flowValue <= 0.9f)
        {
            return tiles[8];
        }
        if (flowValue > 0.9f && flowValue <= 1f)
        {
            return tiles[9];
        }
        return tiles[10];
    }
    #endregion

    #region Chunk methods
    public Chunk GetChunk(int x, int y)
    {
        if (IsInMapRange(x, y))
        {
            return GameManager.Instance.Chunks[x / TerrainConfiguration.chunkSize, y / TerrainConfiguration.chunkSize];
        }
        return null;
    }

    private void SetActiveChunks3x3(Chunk chunk)
    {
        if (TerrainConfiguration.horizontalChunksCount >= 3)
        {
            int chunkSize = TerrainConfiguration.chunkSize;
            for (int x = chunk.x / chunkSize - 1; x <= chunk.x / chunkSize + 1; x++)
            {
                for (int y = chunk.y / chunkSize - 1; y <= chunk.y / chunkSize + 1; y++)
                {
                    if ((x > 0 && y > 0) && (x < TerrainConfiguration.horizontalChunksCount && y < TerrainConfiguration.VerticalChunksCount))
                    {
                        GameManager.Instance.Chunks[x, y].isActive = true;
                    }
                }
            }
        }
    }

    private void SetInactiveChunks3x3(Chunk chunk)
    {
        if (TerrainConfiguration.horizontalChunksCount >= 3)
        {
            int chunkSize = TerrainConfiguration.chunkSize;
            for (int x = chunk.x / chunkSize - 1; x <= chunk.x / chunkSize + 1; x++)
            {
                for (int y = chunk.y / chunkSize - 1; y <= chunk.y / chunkSize + 1; y++)
                {
                    if ((x > 0 && y > 0) && (x < TerrainConfiguration.horizontalChunksCount && y < TerrainConfiguration.VerticalChunksCount))
                    {
                        GameManager.Instance.Chunks[x, y].isActive = false;
                    }
                }
            }
        }
    }
    #endregion

    #endregion

    #region Update methods
    public IEnumerator UpdateScreen()
    {
        while (!GameManager.Instance.IsPlayerCreated)
        {
            yield return null;
        }
        yield return null;
        RectInt prevCameraRect = GetCameraRectInt();
        Chunk currentChunk = GetChunk((int)prevCameraRect.center.x, (int)prevCameraRect.center.y);
        while (true)
        {
            yield return null;
            SetActiveChunks3x3(currentChunk);

            RectInt currentCameraRect = GetCameraRectInt();
            List<TileBase> blockTiles = new List<TileBase>();
            List<TileBase> liquidBackgroundTiles = new List<TileBase>();
            List<TileBase> blockBackgroundTiles = new List<TileBase>();
            List<TileBase> plantTiles = new List<TileBase>();
            List<TileBase> solidBlocksTiles = new List<TileBase>();

            List<Vector3Int> vectors = new List<Vector3Int>();

            //Fill Tiles array with blocks to destroy
            if (Mathf.Abs(prevCameraRect.x - currentCameraRect.x) >= 1
                || Mathf.Abs(prevCameraRect.y - currentCameraRect.y) >= 1)
            {
                foreach (Vector2Int position in prevCameraRect.allPositionsWithin)
                {
                    if (!currentCameraRect.Contains(position))
                    {
                        blockTiles.Add(null);
                        liquidBackgroundTiles.Add(null);
                        blockBackgroundTiles.Add(null);
                        plantTiles.Add(null);
                        solidBlocksTiles.Add(null);

                        vectors.Add(new Vector3Int(position.x, position.y));
                    }
                }
                Chunk newCurrentChunk = GetChunk((int)currentCameraRect.center.x, (int)currentCameraRect.center.y);
                if (newCurrentChunk != null && newCurrentChunk != currentChunk)
                {
                    SetInactiveChunks3x3(currentChunk);
                    currentChunk = newCurrentChunk;
                }
                prevCameraRect = currentCameraRect;
            }

            //Fill Tiles array with drawable blocks
            foreach (Vector2Int position in currentCameraRect.allPositionsWithin)
            {
                ObjectData block = GameManager.Instance.ObjectsData[position.x, position.y];
                if (IsInMapRange(position.x, position.y))
                {
                    TileBase blockTile = block.CurrentTile;
                    TileBase plantTile = null;
                    TileBase liquidBackgroundTile = block.CurrentLiquidBackgroundTile;
                    TileBase backgroundTile = block.CurrentBackgroundTile;
                    if (block.Type == ObjectType.Plant)
                    {
                        blockTile = null;
                        plantTile = block.CurrentTile;
                    }
                    if (block.Type == ObjectType.LiquidBlock)
                    {
                        PaintLiquid(block);
                    }
                    blockTiles.Add(blockTile);
                    plantTiles.Add(plantTile);
                    liquidBackgroundTiles.Add(liquidBackgroundTile);
                    blockBackgroundTiles.Add(backgroundTile);

                    if (GameManager.Instance.ObjectsData[position.x, position.y].Type != ObjectType.Empty &&
                        GameManager.Instance.ObjectsData[position.x, position.y].Type != ObjectType.LiquidBlock &&
                        GameManager.Instance.ObjectsData[position.x, position.y].Type != ObjectType.Plant &&
                        GameManager.Instance.ObjectsData[position.x, position.y].Type != ObjectType.PickableItem)
                    {
                        solidBlocksTiles.Add(SolidBlocksRuleTile);
                    }
                    else
                    {
                        solidBlocksTiles.Add(null);
                    }

                    vectors.Add(new Vector3Int(position.x, position.y));
                }
            }

            //Change Tilemap using Vector's array and Tile's array
            BlockTilemap.SetTiles(vectors.ToArray(), blockTiles.ToArray());
            LiquidBackgroundTilemap.SetTiles(vectors.ToArray(), liquidBackgroundTiles.ToArray());
            BlockBackgroundTilemap.SetTiles(vectors.ToArray(), blockBackgroundTiles.ToArray());
            PlantTilemap.SetTiles(vectors.ToArray(), plantTiles.ToArray());
            SolidBlocksTilemap.SetTiles(vectors.ToArray(), solidBlocksTiles.ToArray());
            if (!IsLevelFullyCreated)
            {
                IsLevelFullyCreated = true;
            }
        }
    }

    public void UpdateBlocksData()
    {
        int chunkSize = TerrainConfiguration.chunkSize;
        while (!IsGameClosed)
        {
            for (int chunkX = 0; chunkX < GameManager.Instance.Chunks.GetLength(0); chunkX++)
            {
                for (int chunkY = 0; chunkY < GameManager.Instance.Chunks.GetLength(1); chunkY++)
                {
                    if (GameManager.Instance.Chunks[chunkX, chunkY].isActive)
                    {
                        for (int x = GameManager.Instance.Chunks[chunkX, chunkY].x; x < GameManager.Instance.Chunks[chunkX, chunkY].x + chunkSize; x++)
                        {
                            for (int y = GameManager.Instance.Chunks[chunkX, chunkY].y; y < GameManager.Instance.Chunks[chunkX, chunkY].y + chunkSize; y++)
                            {
                                ObjectData block = GameManager.Instance.ObjectsData[x, y];
                                ObjectData downBlockData = null;
                                ObjectData leftBlockData = null;
                                ObjectData rightBlockData = null;
                                ObjectData topBlockData = null;

                                if (y - 1 != -1)
                                {
                                    downBlockData = GameManager.Instance.ObjectsData[x, y - 1];
                                }
                                if (x - 1 != -1)
                                {
                                    leftBlockData = GameManager.Instance.ObjectsData[x - 1, y];
                                }
                                if (x + 1 != GameManager.Instance.ObjectsData.GetLength(0))
                                {
                                    rightBlockData = GameManager.Instance.ObjectsData[x + 1, y];
                                }
                                if (y + 1 != GameManager.Instance.ObjectsData.GetLength(1))
                                {
                                    topBlockData = GameManager.Instance.ObjectsData[x, y + 1];
                                }

                                if (block.IsPsevdoFull)
                                {
                                    if (topBlockData.Type == ObjectType.Empty)
                                    {
                                        block.CountPsevdoFull++;
                                    }
                                    if (topBlockData.Type == ObjectType.LiquidBlock)
                                    {
                                        block.CountPsevdoFull = 0;
                                    }
                                    if (block.CountPsevdoFull >= 5)
                                    {
                                        block.IsPsevdoFull = false;
                                    }
                                }

                                if (block.Type == ObjectType.SolidBlock)
                                {
                                    #region Set backgrounds
                                    //If left block is liquid
                                    if (leftBlockData != null &&
                                        leftBlockData.Type == ObjectType.LiquidBlock)
                                    {
                                        block.CurrentLiquidBackgroundTile = leftBlockData.CurrentTile;
                                    }

                                    //If right block is liquid
                                    if (rightBlockData != null &&
                                        rightBlockData.Type == ObjectType.LiquidBlock)
                                    {
                                        block.CurrentLiquidBackgroundTile = rightBlockData.CurrentTile;
                                    }

                                    //If top block is liquid
                                    if (topBlockData != null &&
                                        topBlockData.Type == ObjectType.LiquidBlock)
                                    {
                                        block.CurrentLiquidBackgroundTile = GameManager.Instance.WorldObjectAtlas.Water.liquidFlowValueTiles[10];
                                    }

                                    //If left, right and top block is not liquid
                                    if (leftBlockData != null && leftBlockData.Type != ObjectType.LiquidBlock &&
                                        rightBlockData != null && rightBlockData.Type != ObjectType.LiquidBlock &&
                                        topBlockData != null && topBlockData.Type != ObjectType.LiquidBlock)
                                    {
                                        block.CurrentLiquidBackgroundTile = null;
                                    }
                                    #endregion
                                }

                                if (block.Type == ObjectType.DustBlock)
                                {
                                    #region Set backgrounds
                                    //If left block is liquid
                                    if (leftBlockData != null &&
                                        leftBlockData.Type == ObjectType.LiquidBlock)
                                    {
                                        block.CurrentLiquidBackgroundTile = leftBlockData.CurrentTile;
                                    }

                                    //If right block is liquid
                                    if (rightBlockData != null &&
                                        rightBlockData.Type == ObjectType.LiquidBlock)
                                    {
                                        block.CurrentLiquidBackgroundTile = rightBlockData.CurrentTile;
                                    }

                                    //If top block is liquid
                                    if (topBlockData != null &&
                                        topBlockData.Type == ObjectType.LiquidBlock)
                                    {
                                        block.CurrentLiquidBackgroundTile = GameManager.Instance.WorldObjectAtlas.Water.liquidFlowValueTiles[10];
                                    }

                                    //If left, right and top block is not liquid
                                    if (leftBlockData != null && leftBlockData.Type != ObjectType.LiquidBlock &&
                                        rightBlockData != null && rightBlockData.Type != ObjectType.LiquidBlock &&
                                        topBlockData != null && topBlockData.Type != ObjectType.LiquidBlock)
                                    {
                                        block.CurrentLiquidBackgroundTile = null;
                                    }
                                    #endregion
                                }

                                if (block.Type == ObjectType.LiquidBlock)
                                {
                                    ObjectData liquidObjectData = block;

                                    #region Skip by conditions
                                    if ((liquidObjectData.CreationTime - System.DateTime.Now).Duration().TotalMilliseconds < _time)
                                    {
                                        continue;
                                    }
                                    #endregion


                                    float startValue = liquidObjectData.CurrentFlowValue;
                                    float remainingValue = startValue;
                                    float flow;

                                    #region Move to the bottom side
                                    if (downBlockData != null && (downBlockData.Type == ObjectType.Empty ||
                                        (downBlockData.Type == ObjectType.LiquidBlock && downBlockData.CurrentFlowValue != 1f)))
                                    {
                                        // Determine rate of flow
                                        float downFlow = downBlockData.Type == ObjectType.LiquidBlock ? downBlockData.CurrentFlowValue : 0f;
                                        flow = liquidObjectData.CurrentFlowValue;

                                        // Constrain flow

                                        if (flow + downFlow > _maxValue)
                                        {
                                            flow = 1f - downFlow;
                                        }

                                        // Update temp values
                                        if (flow != 0)
                                        {
                                            remainingValue -= flow;
                                            liquidObjectData.CurrentFlowValue -= flow;
                                            liquidObjectData.CreationTime = DateTime.Now;

                                            //ObjectData newBlock;
                                            if (downBlockData.Type != ObjectType.LiquidBlock)
                                            {
                                                downBlockData.Type = ObjectType.LiquidBlock;
                                                downBlockData.Id = liquidObjectData.Id;
                                            }
                                            downBlockData.CurrentFlowValue += flow;
                                            downBlockData.IsPsevdoFull = true;
                                        }
                                    }

                                    if (remainingValue < _minValue && !liquidObjectData.IsPsevdoFull)
                                    {
                                        CreateBlock(GameManager.Instance.WorldObjectAtlas.GetBlockByID(ObjectType.Empty, 0), x, y);
                                        GameManager.Instance.ObjectsData[x, y].CurrentFlowValue = 0f;
                                        continue;
                                    }
                                    #endregion

                                    #region Move to the left side
                                    if (leftBlockData != null && (leftBlockData.Type == ObjectType.Empty || leftBlockData.Type == ObjectType.LiquidBlock))
                                    {

                                        // Calculate flow rate
                                        float leftFlow = leftBlockData.Type == ObjectType.LiquidBlock ? leftBlockData.CurrentFlowValue : 0f;
                                        flow = (remainingValue - leftFlow) / 4f;

                                        // constrain flow
                                        flow = Mathf.Max(flow, 0);
                                        if (flow > remainingValue)
                                        {
                                            flow = remainingValue;
                                        }

                                        // Adjust temp values
                                        if (flow != 0)
                                        {
                                            remainingValue -= flow;
                                            liquidObjectData.CurrentFlowValue -= flow;
                                            liquidObjectData.CreationTime = DateTime.Now;

                                            if (leftBlockData.Type != ObjectType.LiquidBlock)
                                            {
                                                leftBlockData.Type = ObjectType.LiquidBlock;
                                                leftBlockData.Id = liquidObjectData.Id;
                                            }
                                            leftBlockData.CurrentFlowValue += flow;
                                        }
                                    }

                                    if (remainingValue < _minValue && !liquidObjectData.IsPsevdoFull)
                                    {
                                        CreateBlock(GameManager.Instance.WorldObjectAtlas.Air, x, y);
                                        GameManager.Instance.ObjectsData[x, y].CurrentFlowValue = 0f;
                                        continue;
                                    }
                                    #endregion

                                    #region Move to the right side
                                    if (rightBlockData != null && (rightBlockData.Type == ObjectType.Empty || rightBlockData.Type == ObjectType.LiquidBlock))
                                    {
                                        // calc flow rate
                                        float rightFlow = rightBlockData.Type == ObjectType.LiquidBlock ? rightBlockData.CurrentFlowValue : 0f;
                                        flow = (remainingValue - rightFlow) / 3f;

                                        // constrain flow
                                        flow = Mathf.Max(flow, 0);
                                        if (flow > remainingValue)
                                        {
                                            flow = remainingValue;
                                        }

                                        // Adjust temp values
                                        if (flow != 0)
                                        {
                                            remainingValue -= flow;
                                            liquidObjectData.CurrentFlowValue -= flow;
                                            liquidObjectData.CreationTime = DateTime.Now;

                                            if (rightBlockData.Type != ObjectType.LiquidBlock)
                                            {
                                                rightBlockData.Type = ObjectType.LiquidBlock;
                                                rightBlockData.Id = liquidObjectData.Id;
                                            }
                                            rightBlockData.CurrentFlowValue += flow;
                                        }
                                    }

                                    if (remainingValue < _minValue && !liquidObjectData.IsPsevdoFull)
                                    {
                                        CreateBlock(GameManager.Instance.WorldObjectAtlas.Air, x, y);
                                        GameManager.Instance.ObjectsData[x, y].CurrentFlowValue = 0f;
                                        continue;
                                    }
                                    #endregion

                                    if (topBlockData != null && topBlockData.Type != ObjectType.Empty && Math.Round(liquidObjectData.CurrentFlowValue, 2) >= 1f)
                                    {
                                        liquidObjectData.IsAboveNotEmpty = true;
                                    }
                                    else
                                    {
                                        liquidObjectData.IsAboveNotEmpty = false;
                                    }

                                }

                                if (block.Type == ObjectType.Plant)
                                {
                                    if (block.Id == (int)PlantsID.Vine && topBlockData.Type == ObjectType.Empty)
                                    {
                                        CreateBlock(GameManager.Instance.WorldObjectAtlas.Air, block.Position.x, block.Position.y);
                                        continue;
                                    }
                                    if (downBlockData.Type == ObjectType.Empty)
                                    {
                                        CreateBlock(GameManager.Instance.WorldObjectAtlas.Air, block.Position.x, block.Position.y);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void RandomUpdateBlocksData()
    {
        while (!IsGameClosed)
        {
            int x = RandomVar.Next(0, TerrainConfiguration.WorldWitdh);
            int y = RandomVar.Next(0, TerrainConfiguration.WorldHeight);

            //Update vine
            if (GameManager.Instance.ObjectsData[x, y].Type == ObjectType.Plant && 
                GameManager.Instance.ObjectsData[x, y].Id == (int)PlantsID.Vine &&
                RandomVar.Next(0, 101) <= GameManager.Instance.ObjectsData[x, y].ChanceToGrow && 
                GameManager.Instance.ObjectsData[x, y - 1].Type == ObjectType.Empty)
            {
                CreateBlock(GameManager.Instance.WorldObjectAtlas.Vine, x, y - 1);
            }
        }
    }
    #endregion

    #region Valid methods
    public bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < TerrainConfiguration.WorldWitdh && y >= 0 && y < TerrainConfiguration.WorldHeight;
    }

    public bool IsValidPlaceToCreateBlock(Vector3Int intPosition)
    {
        return GameManager.Instance.ObjectsData[intPosition.x, intPosition.y].Type == ObjectType.Empty &&
            !GameManager.Instance.ObjectsData[intPosition.x, intPosition.y].IsTreeFoliage &&
            !GameManager.Instance.ObjectsData[intPosition.x, intPosition.y].IsTreeTrunk;
    }

    public bool IsValidPlaceToBreakBlock(Vector3Int intPosition)
    {
        bool result;
        bool isNotEmpty = GameManager.Instance.ObjectsData[intPosition.x, intPosition.y].Type != ObjectType.Empty;
        bool isNotPickableObject = GameManager.Instance.ObjectsData[intPosition.x, intPosition.y].Type != ObjectType.PickableItem;
        if (GameManager.Instance.ObjectsData[intPosition.x, intPosition.y].Type != ObjectType.Plant)
        {
            bool isUnderNotTreeTrunk = !GameManager.Instance.ObjectsData[intPosition.x, intPosition.y + 1].IsTreeTrunk;
            result = isNotEmpty && isNotPickableObject && isUnderNotTreeTrunk;
        }
        else
        {
            result = isNotEmpty && isNotPickableObject;
        }
        return result;
    }

    public bool IsAdjacentBlockSolid(Vector3 position, Vector2Int direction)
    {
        Vector3Int intPosition = BlockTilemap.WorldToCell(position);
        intPosition += new Vector3Int(direction.x, direction.y);
        return GameManager.Instance.ObjectsData[intPosition.x, intPosition.y].Type != ObjectType.Empty &&
            GameManager.Instance.ObjectsData[intPosition.x, intPosition.y].Type != ObjectType.Plant &&
            GameManager.Instance.ObjectsData[intPosition.x, intPosition.y].Type != ObjectType.LiquidBlock &&
            GameManager.Instance.ObjectsData[intPosition.x, intPosition.y].Type != ObjectType.PickableItem;
    }
    #endregion

    #endregion
}
