using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Terrain : MonoBehaviour
{
    #region Private fields
    [Header("Tilemaps")]
    [SerializeField] private Tilemap _blocksTilemap;
    [SerializeField] private GameObject _trees;
    [SerializeField] private GameObject _pickableItems;

    #region Threads
    private Thread _blockProcessingThread;
    private Thread _randomBlockProcessingThread;
    private object _lockObject = new object();
    #endregion

    #endregion

    #region Public fields

    #endregion

    #region Properties
    public GameObject Trees
    {
        get
        {
            return _trees;
        }

        set
        {
            _trees = value;
        }
    }

    public Tilemap BlocksTilemap
    {
        get
        {
            return _blocksTilemap;
        }

        set
        {
            _blocksTilemap = value;
        }
    }

    public GameObject PickableItems
    {
        get
        {
            return _pickableItems;
        }

        set
        {
            _pickableItems = value;
        }
    }
    #endregion

    #region Methods

    #region General
    private void Awake()
    {
        //Setup tilemaps
        BlocksTilemap = transform.Find("BlocksTilemap").GetComponent<Tilemap>();
        if (BlocksTilemap == null)
        {
            throw new NullReferenceException("BlockTilemap is null");
        }
        Trees = transform.Find("Trees").gameObject;
        if (Trees == null)
        {
            throw new NullReferenceException("Trees is null");
        }
        PickableItems = transform.Find("PickableItems").gameObject;
        if (PickableItems == null)
        {
            throw new NullReferenceException("PickableItems is null");
        }
    }

    public void CreateNewWorld(ref WorldCellData[,] worldData)
    {
        try
        {
            GameManager.Instance.RandomVar = new System.Random(GameManager.Instance.Seed);

            //Start generation
            TerrainGeneration terrainGeneration = new TerrainGeneration(GameManager.Instance.Seed, ref worldData);
            terrainGeneration.StartTerrainGeneration();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            throw e;
        }
    }

    public void StartCoroutinesAndThreads()
    {
        //Start update tilemaps
        StartCoroutine(UpdateTilemaps());

        //Start update chunk activity
        //StartCoroutine(UpdateChunkActivity());

        //Start block processing
        //_blockProcessingThread = new Thread(BlockProcessing);
        //_blockProcessingThread.Start();

        //Start random block processing
        //_randomBlockProcessingThread = new Thread(RandomBlockProcessing);
        //_randomBlockProcessingThread.Start();
    }
    #endregion

    #region Update
    public IEnumerator UpdateTilemaps()
    {
        RectInt currentCameraRect = new RectInt();
        RectInt prevCameraRect = new RectInt();
        Vector3Int vector = new Vector3Int();

        int arraySizeX;
        int arraySizeY;
        TileBase[] blockTiles;
        Vector3Int[] vectors;
        int i;

        ArrayObjectPool<TileBase> blockTilesPool = new ArrayObjectPool<TileBase>();
        ArrayObjectPool<Vector3Int> vectorsPool = new ArrayObjectPool<Vector3Int>();

        prevCameraRect = GetCameraRectInt();

        while (true)
        {
            yield return null;

            i = 0;
            currentCameraRect = GetCameraRectInt();

            //Calculate array size
            arraySizeX = prevCameraRect.width + Mathf.Abs(prevCameraRect.x - currentCameraRect.x);
            arraySizeY = prevCameraRect.height + Mathf.Abs(prevCameraRect.y - currentCameraRect.y);

            blockTiles = blockTilesPool.GetArray(arraySizeX * arraySizeY);
            vectors = vectorsPool.GetArray(arraySizeX * arraySizeY);

            //Fill Tiles array with blocks to destroy
            if (Mathf.Abs(prevCameraRect.x - currentCameraRect.x) >= 1
               || Mathf.Abs(prevCameraRect.y - currentCameraRect.y) >= 1)
            {
                foreach (Vector2Int position in prevCameraRect.allPositionsWithin)
                {
                    if (!currentCameraRect.Contains(position))
                    {
                        blockTiles[i] = null;
                        vector.x = position.x;
                        vector.y = position.y;
                        vectors[i] = vector;
                        i++;
                    }
                }
                prevCameraRect = currentCameraRect;
            }

            //Fill Tiles array with drawable blocks
            foreach (Vector2Int position in currentCameraRect.allPositionsWithin)
            {
                if (IsInMapRange(position.x, position.y))
                {
                    blockTiles[i] = GameManager.Instance.WorldData[position.x, position.y].GetTile();
                    vector.x = position.x;
                    vector.y = position.y;
                    vectors[i] = vector;
                    i++;
                }
            }

            //Change Tilemap using Vector's array and Tile's array
            BlocksTilemap.SetTiles(vectors, blockTiles);
        }
    }

    public IEnumerator UpdateChunkActivity()
    {
        RectInt currentCameraRect;
        RectInt prevCameraRect = GetCameraRectInt();
        Chunk currentChunk = GameManager.Instance.GetChunk((int)prevCameraRect.center.x, (int)prevCameraRect.center.y);
        Chunk prevChunk;
        int chunkSize = GameManager.Instance.TerrainConfiguration.ChunkSize;
        int x;
        int y;
        
        while (true)
        {
            yield return null;

            currentCameraRect = GetCameraRectInt();

            for (x = currentChunk.Coords.x - 1; x <= currentChunk.Coords.x + 1; x++)
            {
                for (y = currentChunk.Coords.y - 1; y <= currentChunk.Coords.y + 1; y++)
                {
                    if (x < 0 || y < 0 || x > GameManager.Instance.Chunks.GetLength(0) || y > GameManager.Instance.Chunks.GetLength(1))
                    {
                        continue;
                    }
                    GameManager.Instance.SetChunkActivityByChunkCoords(x, y, true);
                }
            }

            if (Mathf.Abs(prevCameraRect.x - currentCameraRect.x) >= 1 ||
                Mathf.Abs(prevCameraRect.y - currentCameraRect.y) >= 1)
            {
                prevChunk = currentChunk;
                currentChunk = GameManager.Instance.GetChunk((int)currentCameraRect.center.x, (int)currentCameraRect.center.y);

                if (prevChunk.Coords.x != currentChunk.Coords.x ||
                    prevChunk.Coords.y != currentChunk.Coords.y)
                {
                    for (x = prevChunk.Coords.x - 1; x <= prevChunk.Coords.x + 1; x++)
                    {
                        for (y = prevChunk.Coords.y - 1; y <= prevChunk.Coords.y + 1; y++)
                        {
                            if (x < 0 || y < 0 || x > GameManager.Instance.Chunks.GetLength(0) || y > GameManager.Instance.Chunks.GetLength(1))
                            {
                                continue;
                            }
                            GameManager.Instance.SetChunkActivityByChunkCoords(x, y, false);
                        }
                    }
                }
            }
        }
    }

    public void BlockProcessing()
    {
        Chunk currentChunk;
        int chunkSize = GameManager.Instance.TerrainConfiguration.ChunkSize;

        int minX = 0;
        int minY = 0;
        int maxX = GameManager.Instance.CurrentTerrainWidth - 1;
        int maxY = GameManager.Instance.CurrentTerrainHeight - 1;

        WorldCellData block;
        WorldCellData bottomBlock;
        WorldCellData topBlock;
        WorldCellData leftBlock;
        WorldCellData rightBlock;

        while (!GameManager.Instance.IsGameSession)
        {

        }
        while (GameManager.Instance.IsGameSession)
        {
            for (int chunkX = 0; chunkX < GameManager.Instance.Chunks.GetLength(0); chunkX++)
            {
                for (int chunkY = 0; chunkY < GameManager.Instance.Chunks.GetLength(1); chunkY++)
                {
                    currentChunk = GameManager.Instance.Chunks[chunkX, chunkY];
                    if (currentChunk.Activity)
                    {
                        for (ushort x = (ushort)(currentChunk.Coords.x * chunkSize); x < currentChunk.Coords.x * chunkSize + chunkSize; x++)
                        {
                            for (ushort y = (ushort)(currentChunk.Coords.y * chunkSize); x < currentChunk.Coords.y * chunkSize + chunkSize; y++)
                            {
                                #region Set blocks
                                block = GameManager.Instance.WorldData[x, y];
                                if (y - 1 >= minY)
                                {
                                    bottomBlock = GameManager.Instance.WorldData[x, y - 1];
                                }
                                if (y + 1 <= maxY)
                                {
                                    topBlock = GameManager.Instance.WorldData[x, y + 1];
                                }
                                if (x - 1 >= minX)
                                {
                                    leftBlock = GameManager.Instance.WorldData[x - 1, y];
                                }
                                if (x + 1 <= maxX)
                                {
                                    rightBlock = GameManager.Instance.WorldData[x + 1, y];
                                }
                                #endregion

                                if (block.BlockType == BlockTypes.Abstract)
                                {

                                }

                                if (block.BlockType == BlockTypes.Solid)
                                {

                                }

                                if (block.BlockType == BlockTypes.Dust)
                                {

                                }

                                if (block.BlockType == BlockTypes.Liquid)
                                {

                                }

                                if (block.BlockType == BlockTypes.Plant)
                                {

                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void RandomBlockProcessing()
    {
        while (!GameManager.Instance.IsGameSession)
        {

        }
        while (GameManager.Instance.IsGameSession)
        {

        }
    }
    #endregion

    #region Helpful
    public static void CreateBlock(ushort x, ushort y, BlockSO block)
    {
        GameManager.Instance.WorldData[x, y].SetData(block);
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

    #region Valid
    public bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < GameManager.Instance.CurrentTerrainWidth && y >= 0 && y < GameManager.Instance.CurrentTerrainHeight;
    }
    #endregion

    #endregion
}
