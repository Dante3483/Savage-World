using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Terrain : MonoBehaviour
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private int _renderWidth;
    [SerializeField] private int _renderHeight;
    [SerializeField] private Camera _camera;

    [Header("Tilemaps")]
    [SerializeField] private TileBase _solidTileBase;
    [SerializeField] private Tilemap _blocksTilemap;
    [SerializeField] private Tilemap _liquidTilemap;
    [SerializeField] private Tilemap _backgroundTilemap;
    [SerializeField] private Tilemap _solidTilemap;

    [Header("Sections")]
    [SerializeField] private GameObject _trees;
    [SerializeField] private GameObject _pickableItems;

    private WorldCellData[,] _worldData;

    #region World data update
    private HashSet<Vector2Ushort> _needToUpdate;
    #endregion

    #region World data render
    private bool _firstRender;
    private RectInt _currentCameraRect;
    private RectInt _prevCameraRect;
    private Vector3Int[] _tilesCoords;
    private TileBase[] _blockTiles;
    private TileBase[] _liquidTiles;
    private TileBase[] _backgroundTiles;
    private TileBase[] _solidTiles;
    private ArrayObjectPool<TileBase> _blockTilesPool;
    private ArrayObjectPool<TileBase> _liquidTilesPool;
    private ArrayObjectPool<TileBase> _backgroundTilesPool;
    private ArrayObjectPool<TileBase> _solidTilesPool;
    private ArrayObjectPool<Vector3Int> _tilesCoordsPool;
    private List<List<TileBase>> _allLiquidTiles;
    #endregion

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

    public Tilemap LiquidTilemap
    {
        get
        {
            return _liquidTilemap;
        }

        set
        {
            _liquidTilemap = value;
        }
    }

    public Tilemap BackgroundTilemap
    {
        get
        {
            return _backgroundTilemap;
        }

        set
        {
            _backgroundTilemap = value;
        }
    }

    public HashSet<Vector2Ushort> NeedToUpdate
    {
        get
        {
            return _needToUpdate;
        }

        set
        {
            _needToUpdate = value;
        }
    }

    public Tilemap SolidTilemap
    {
        get
        {
            return _solidTilemap;
        }

        set
        {
            _solidTilemap = value;
        }
    }
    #endregion

    #region Methods

    #region General
    private void Awake()
    {
        #region Setup tilemaps
        BlocksTilemap = transform.Find("BlocksTilemap").GetComponent<Tilemap>();
        if (BlocksTilemap == null)
        {
            throw new NullReferenceException("BlockTilemap is null");
        }

        LiquidTilemap = transform.Find("LiquidTilemap").GetComponent<Tilemap>();
        if (LiquidTilemap == null)
        {
            throw new NullReferenceException("LiquidTilemap is null");
        }

        BackgroundTilemap = transform.Find("BackgroundTilemap").GetComponent<Tilemap>();
        if (BackgroundTilemap == null)
        {
            throw new NullReferenceException("BackgroundTilemap is null");
        }

        SolidTilemap = transform.Find("SolidTilemap").GetComponent<Tilemap>();
        if (SolidTilemap == null)
        {
            throw new NullReferenceException("SolidTilemap is null");
        }
        #endregion

        #region Setup sections
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
        #endregion

        #region Initialization
        NeedToUpdate = new HashSet<Vector2Ushort>();
        _camera = Camera.main;
        _currentCameraRect = new RectInt();
        _prevCameraRect = new RectInt();
        _tilesCoords = new Vector3Int[_renderWidth * _renderHeight];
        _blockTiles = new TileBase[_renderWidth * _renderHeight];
        _liquidTiles = new TileBase[_renderWidth * _renderHeight];
        _backgroundTiles = new TileBase[_renderWidth * _renderHeight];
        _solidTiles = new TileBase[_renderWidth * _renderHeight];
        _blockTilesPool = new ArrayObjectPool<TileBase>();
        _liquidTilesPool = new ArrayObjectPool<TileBase>();
        _backgroundTilesPool = new ArrayObjectPool<TileBase>();
        _solidTilesPool = new ArrayObjectPool<TileBase>();
        _tilesCoordsPool = new ArrayObjectPool<Vector3Int>();
        _firstRender = true;
        #endregion
    }

    private void Start()
    {
        _allLiquidTiles = new List<List<TileBase>>()
        {
            GameManager.Instance.ObjectsAtlass.Water.Tiles,
        };
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameSession)
        {
            RenderWorldData();
            UpdateWorldData();
        }
    }

    public void CreateNewWorld(ref WorldCellData[,] worldData)
    {
        try
        {
            _worldData = worldData;
            GameManager.Instance.RandomVar = new System.Random(GameManager.Instance.Seed);

            //Start generation
            TerrainGeneration terrainGeneration = new TerrainGeneration(GameManager.Instance.Seed, ref worldData, this);
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
        //StartCoroutine(UpdateTilemaps());

        //Start update chunk activity
        //StartCoroutine(UpdateChunkActivity());

        //Start block processing
        //_blockProcessingThread = new Thread(BlockProcessing);
        //_blockProcessingThread.Start();
        //_blockProcessingThread = new Thread(BlockProcessingThreaded);
        //_blockProcessingThread.Start();

        //Start random block processing
        _randomBlockProcessingThread = new Thread(RandomUpdateWorldData);
        _randomBlockProcessingThread.Start();
    }
    #endregion

    #region Update

    #region Old
    public IEnumerator UpdateTilemaps()
    {
        RectInt currentCameraRect = new RectInt();
        RectInt prevCameraRect = new RectInt();
        Vector3Int vector = new Vector3Int();
        Vector3Int[] vectors;

        WorldCellData block;

        TileBase[] blockTiles;
        TileBase[] liquidTiles;
        TileBase[] backgroundTiles;

        List<List<TileBase>> allLiquidTiles = new List<List<TileBase>>()
        {
            GameManager.Instance.ObjectsAtlass.Water.Tiles,
        };

        int arraySizeX;
        int arraySizeY;
        int i;

        byte liquidIndex;

        ArrayObjectPool<TileBase> blockTilesPool = new ArrayObjectPool<TileBase>();
        ArrayObjectPool<TileBase> liquidTilesPool = new ArrayObjectPool<TileBase>();
        ArrayObjectPool<TileBase> backgroundTilesPool = new ArrayObjectPool<TileBase>();
        ArrayObjectPool<Vector3Int> vectorsPool = new ArrayObjectPool<Vector3Int>();

        prevCameraRect = GetCameraRectInt();

        while (true)
        {
            yield return new WaitForFixedUpdate();

            i = 0;
            currentCameraRect = GetCameraRectInt();

            //Calculate array size
            arraySizeX = prevCameraRect.width + Mathf.Abs(prevCameraRect.x - currentCameraRect.x);
            arraySizeY = prevCameraRect.height + Mathf.Abs(prevCameraRect.y - currentCameraRect.y);

            blockTiles = blockTilesPool.GetArray(arraySizeX * arraySizeY);
            liquidTiles = liquidTilesPool.GetArray(arraySizeX * arraySizeY);
            backgroundTiles = backgroundTilesPool.GetArray(arraySizeX * arraySizeY);
            vectors = vectorsPool.GetArray(arraySizeX * arraySizeY);

            //Fill Tiles array with blocks to destroy
            if (Mathf.Abs(prevCameraRect.x - currentCameraRect.x) >= 1
               || Mathf.Abs(prevCameraRect.y - currentCameraRect.y) >= 1)
            {
                foreach (Vector2Int position in prevCameraRect.allPositionsWithin)
                {
                    if (!currentCameraRect.Contains(position))
                    {
                        vector.x = position.x;
                        vector.y = position.y;
                        vectors[i] = vector;
                        blockTiles[i] = null;
                        liquidTiles[i] = null;
                        backgroundTiles[i] = null;
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
                    block = GameManager.Instance.WorldData[position.x, position.y];

                    vector.x = position.x;
                    vector.y = position.y;
                    vectors[i] = vector;

                    //Block
                    blockTiles[i] = GameManager.Instance.WorldData[position.x, position.y].GetBlockTile();

                    //Background
                    backgroundTiles[i] = GameManager.Instance.WorldData[position.x, position.y].GetBackgroundTile();

                    //Liquid
                    liquidTiles[i] = null;
                    if (block.IsEmptyForLiquid() && block.IsLiquid())
                    {
                        liquidIndex = (byte)(block.FlowValue == 100f || block.IsFlowsDown ? 9 : block.FlowValue / 10);
                        liquidTiles[i] = allLiquidTiles[block.LiquidId][liquidIndex];
                    }

                    i++;
                }
            }

            //Change Tilemap using Vector's array and Tile's array
            BlocksTilemap.SetTiles(vectors, blockTiles);
            LiquidTilemap.SetTiles(vectors, liquidTiles);
            BackgroundTilemap.SetTiles(vectors, backgroundTiles);
        }
    }

    public IEnumerator UpdateChunkActivity()
    {
        RectInt currentCameraRect = new RectInt();
        RectInt prevCameraRect = new RectInt();

        prevCameraRect = GetCameraRectInt();
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
        int horizontalChunkCount = GameManager.Instance.Chunks.GetLength(0);
        int verticalChunkCount = GameManager.Instance.Chunks.GetLength(1);
        //System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

        int minX = 5;
        int minY = 5;
        int maxX = GameManager.Instance.CurrentTerrainWidth - 5;
        int maxY = GameManager.Instance.CurrentTerrainHeight - 5;
        int chunkX;
        int chunkY;
        int startX;
        int startY;
        int endX;
        int endY;
        int x;
        int y;

        ref WorldCellData block = ref GameManager.Instance.GetWorldCellDataRef(0,0);
        ref WorldCellData bottomBlock = ref GameManager.Instance.GetWorldCellDataRef(0, 0);
        ref WorldCellData topBlock = ref GameManager.Instance.GetWorldCellDataRef(0, 0);
        ref WorldCellData leftBlock = ref GameManager.Instance.GetWorldCellDataRef(0, 0);
        ref WorldCellData leftBottomBlock = ref GameManager.Instance.GetWorldCellDataRef(0, 0);
        ref WorldCellData rightBlock = ref GameManager.Instance.GetWorldCellDataRef(0, 0);
        ref WorldCellData rightBottomBlock = ref GameManager.Instance.GetWorldCellDataRef(0, 0);

        float minFlowValue = 0.01f;
        float maxFlowValue = 100f;
        float startFlowValue;
        float remainingFlowValue;
        float flow;
        float bottomFlow;
        float leftFlow;
        float rightFlow;

        //double currentTime = 0;
        //double prevTime = 0;

        bool needToFlow;

        BlockSO airBlock = GameManager.Instance.ObjectsAtlass.Air;
        PlantSO plant;

        while (!GameManager.Instance.IsGameSession)
        {

        }
        while (GameManager.Instance.IsGameSession)
        {
            //watch.Restart();

            for (chunkX = 0; chunkX < horizontalChunkCount; chunkX++)
            {
                for (chunkY = 0; chunkY < verticalChunkCount; chunkY++)
                {
                    currentChunk = GameManager.Instance.Chunks[chunkX, chunkY];
                    if (currentChunk.Activity)
                    {
                        startX = currentChunk.Coords.x * chunkSize;
                        startY = currentChunk.Coords.y * chunkSize;
                        endX = currentChunk.Coords.x * chunkSize + chunkSize;
                        endY = currentChunk.Coords.y * chunkSize + chunkSize;

                        for (x = startX; x < endX; x++)
                        {
                            for (y = startY; y < endY; y++)
                            {
                                if (x < minX || y < minY || x > maxX || y > maxY)
                                {
                                    continue;
                                }

                                #region Set blocks
                                block = ref GameManager.Instance.GetWorldCellDataRef(x, y);
                                bottomBlock = ref GameManager.Instance.GetWorldCellDataRef(x, y - 1);
                                topBlock = ref GameManager.Instance.GetWorldCellDataRef(x, y + 1);
                                leftBlock = ref GameManager.Instance.GetWorldCellDataRef(x - 1, y);
                                rightBlock = ref GameManager.Instance.GetWorldCellDataRef(x + 1, y);
                                #endregion

                                if (block.BlockType == BlockTypes.Abstract)
                                {

                                }

                                if (block.BlockType == BlockTypes.Solid)
                                {

                                }

                                if (block.BlockType == BlockTypes.Dust)
                                {
                                    if (block.CurrentActionTime < block.GetDustActionTime())
                                    {
                                        block.CurrentActionTime++;
                                    }
                                    else
                                    {
                                        block.CurrentActionTime = 0;
                                        if (bottomBlock.IsEmpty())
                                        {
                                            CreateBlock((ushort)x, (ushort)(y - 1), block.BlockData);
                                            CreateBlock((ushort)x, (ushort)y, airBlock);
                                        }
                                    }
                                }

                                if (block.IsLiquid() && block.IsEmptyForLiquid())
                                {
                                    if (block.CurrentActionTime < block.GetLiquidActionTime())
                                    {
                                        block.CurrentActionTime++;
                                    }
                                    else
                                    {
                                        block.CurrentActionTime = 0;
                                        startFlowValue = block.FlowValue;
                                        remainingFlowValue = startFlowValue;
                                        needToFlow = true;

                                        #region Move to the bottom side
                                        if (bottomBlock.IsEmptyForLiquid() || (bottomBlock.IsLiquid() && bottomBlock.FlowValue != 100))
                                        {
                                            //Determine rate of flow
                                            bottomFlow = (bottomBlock.IsLiquid() ? bottomBlock.FlowValue : 0);
                                            flow = block.FlowValue;

                                            //Constrain flow
                                            if (flow + bottomFlow > maxFlowValue)
                                            {
                                                flow = (100 - bottomFlow);
                                            }

                                            //Update flow values
                                            if (flow != 0)
                                            {
                                                remainingFlowValue -= flow;
                                                block.FlowValue -= flow;

                                                //liquidObjectData.CreationTime = DateTime.Now;

                                                //If bottom block is not liquid
                                                if (!bottomBlock.IsLiquid())
                                                {
                                                    CreateLiquidBlock((ushort)x, (ushort)(y - 1), block.LiquidId);
                                                    bottomBlock.FlowValue = 0;
                                                }
                                                bottomBlock.FlowValue += flow;
                                                bottomBlock.IsFlowsDown = true;
                                            }
                                        }

                                        if (remainingFlowValue < minFlowValue)
                                        {
                                            block.FlowValue = 0f;
                                            block.LiquidId = 255;
                                            needToFlow = false;
                                        }
                                        #endregion

                                        #region Move to the left side
                                        if ((leftBlock.IsEmptyForLiquid() || leftBlock.IsLiquid()) && needToFlow)
                                        {
                                            //Determine rate of flow
                                            leftFlow = (leftBlock.IsLiquid() ? leftBlock.FlowValue : 0);
                                            flow = (leftFlow > remainingFlowValue ? 0 : (remainingFlowValue - leftFlow) / 4f);

                                            //Constrain flow
                                            if (flow + leftFlow > maxFlowValue)
                                            {
                                                flow = (100 - leftFlow);
                                                //flow = remainingFlowValue;
                                            }

                                            //Update flow values
                                            if (flow != 0)
                                            {
                                                remainingFlowValue -= flow;
                                                block.FlowValue -= flow;

                                                //liquidObjectData.CreationTime = DateTime.Now;

                                                //If left block is not liquid
                                                if (!leftBlock.IsLiquid())
                                                {
                                                    CreateLiquidBlock((ushort)(x - 1), (ushort)y, block.LiquidId);
                                                    leftBlock.FlowValue = 0;
                                                }
                                                leftBlock.FlowValue += flow;
                                                //downBlockData.IsPsevdoFull = true;
                                            }
                                        }

                                        if (remainingFlowValue < minFlowValue)
                                        {
                                            block.FlowValue = 0f;
                                            block.LiquidId = 255;
                                            needToFlow = false;
                                        }
                                        #endregion

                                        #region Move to the right side
                                        if ((rightBlock.IsEmptyForLiquid() || rightBlock.IsLiquid()) && needToFlow)
                                        {
                                            //Determine rate of flow
                                            rightFlow = (rightBlock.IsLiquid() ? rightBlock.FlowValue : 0);
                                            flow = (rightFlow > remainingFlowValue ? 0 : (remainingFlowValue - rightFlow) / 3f);

                                            //Constrain flow
                                            if (flow + rightFlow > maxFlowValue)
                                            {
                                                flow = (100 - rightFlow);
                                            }

                                            //Update flow values
                                            if (flow != 0)
                                            {
                                                remainingFlowValue -= flow;
                                                block.FlowValue -= flow;

                                                //liquidObjectData.CreationTime = DateTime.Now;

                                                //If right block is not liquid
                                                if (!rightBlock.IsLiquid())
                                                {
                                                    CreateLiquidBlock((ushort)(x + 1), (ushort)y, block.LiquidId);
                                                    rightBlock.FlowValue = 0;
                                                }
                                                rightBlock.FlowValue += flow;
                                                //downBlockData.IsPsevdoFull = true;
                                            }
                                        }

                                        if (remainingFlowValue < minFlowValue)
                                        {
                                            block.FlowValue = 0f;
                                            block.LiquidId = 255;
                                        }
                                        #endregion

                                        #region Top side processing
                                        if (topBlock.IsLiquid() && block.FlowValue != 100f)
                                        {
                                            block.IsFlowsDown = true;
                                        }
                                        else
                                        {
                                            block.IsFlowsDown = false;
                                        }
                                        #endregion
                                    }
                                }

                                if (block.BlockType == BlockTypes.Plant)
                                {
                                    plant = block.BlockData as PlantSO;

                                    if (block.IsLiquid())
                                    {
                                        CreateBlock((ushort)x, (ushort)y, airBlock);
                                    }

                                    if (plant.IsTopBlockSolid && topBlock.IsEmpty())
                                    {
                                        CreateBlock((ushort)x, (ushort)y, airBlock);
                                    }

                                    if (plant.IsBottomBlockSolid && bottomBlock.IsEmpty())
                                    {
                                        CreateBlock((ushort)x, (ushort)y, airBlock);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //watch.Stop();
            //currentTime = watch.Elapsed.TotalSeconds;
            //GameManager.Instance.ProcessingSpeedInfo = "";
            //GameManager.Instance.ProcessingSpeedInfo += $"Block processing time: {currentTime}\n";
            //GameManager.Instance.ProcessingSpeedInfo += $"Difference: {prevTime - currentTime}";
            //prevTime = currentTime;

        }
    }
    #endregion

    #region Current
    List<Vector2Ushort> vectors = new List<Vector2Ushort>();
    public void UpdateWorldData()
    {
        vectors.Clear();
        vectors.AddRange(NeedToUpdate);
        NeedToUpdate.Clear();

        GameManager.Instance.OtherInfo = $"{vectors.Count} blocks need to update";

        BlockSO airBlock = GameManager.Instance.ObjectsAtlass.Air;
        PlantSO plant;

        float minFlowValue = 0.1f;
        float maxFlowValue = 100f;
        float minForCounter = 0.01f;
        float startFlowValue;
        float remainingFlowValue;
        float flow;
        float bottomFlow;
        float leftFlow;
        float rightFlow;
        bool needToFlow;
        bool needToStop = false;

        foreach (Vector2Ushort coord in vectors)
        {
            ushort x = coord.x;
            ushort y = coord.y;

            #region Set blocks
            ref WorldCellData block = ref GameManager.Instance.GetWorldCellDataRef(x, y);
            ref WorldCellData bottomBlock = ref GameManager.Instance.GetWorldCellDataRef(x, y - 1);
            ref WorldCellData topBlock = ref GameManager.Instance.GetWorldCellDataRef(x, y + 1);
            ref WorldCellData leftBlock = ref GameManager.Instance.GetWorldCellDataRef(x - 1, y);
            ref WorldCellData rightBlock = ref GameManager.Instance.GetWorldCellDataRef(x + 1, y);
            #endregion

            if (!block.BlockData.Waterproof && block.IsLiquid())
            {
                CreateBlock(x, y, airBlock);
            }

            if (block.BlockType == BlockTypes.Abstract)
            {

            }

            if (block.BlockType == BlockTypes.Solid)
            {

            }

            if (block.BlockType == BlockTypes.Dust)
            {
                if (block.CurrentActionTime < block.GetDustActionTime())
                {
                    block.CurrentActionTime++;
                    NeedToUpdate.Add(block.Coords);
                }
                else
                {
                    block.CurrentActionTime = 0;
                    if (bottomBlock.IsEmpty())
                    {
                        CreateBlock(x, (ushort)(y - 1), block.BlockData);
                        CreateBlock(x, y, airBlock);
                        NeedToUpdate.Add(bottomBlock.Coords);
                        if (topBlock.IsDust())
                        {
                            NeedToUpdate.Add(topBlock.Coords);
                        }
                    }
                }
            }

            if (block.IsLiquid() && block.IsEmptyForLiquid())
            {
                if (block.CurrentActionTime < block.GetLiquidActionTime())
                {
                    block.CurrentActionTime++;
                }
                else
                {
                    block.CurrentActionTime = 0;
                    startFlowValue = block.FlowValue;
                    remainingFlowValue = startFlowValue;
                    needToFlow = true;

                    #region Move to the bottom side
                    if (bottomBlock.IsEmptyForLiquid() || (bottomBlock.IsLiquid() && bottomBlock.FlowValue != 100))
                    {
                        //Determine rate of flow
                        bottomFlow = (bottomBlock.IsLiquid() ? bottomBlock.FlowValue : 0);
                        flow = block.FlowValue;

                        //Constrain flow
                        if (flow + bottomFlow > maxFlowValue)
                        {
                            flow = (100 - bottomFlow);
                        }

                        //Update flow values
                        if (flow != 0)
                        {
                            remainingFlowValue -= flow;
                            block.FlowValue -= flow;

                            //If bottom block is not liquid
                            if (!bottomBlock.IsLiquid())
                            {
                                CreateLiquidBlock(x, (ushort)(y - 1), block.LiquidId);
                                bottomBlock.FlowValue = 0.0f;
                                if (leftBlock.IsLiquid())
                                {
                                    NeedToUpdate.Add(leftBlock.Coords);
                                    leftBlock.CountToStop = 0;
                                }
                                if (rightBlock.IsLiquid())
                                {
                                    NeedToUpdate.Add(rightBlock.Coords);
                                    rightBlock.CountToStop = 0;
                                }
                            }
                            bottomBlock.FlowValue += flow;
                            bottomBlock.IsFlowsDown = true;
                            bottomBlock.CountToStop = 0;
                            NeedToUpdate.Add(bottomBlock.Coords);
                            if (topBlock.IsLiquid())
                            {
                                NeedToUpdate.Add(topBlock.Coords);
                                topBlock.CountToStop = 0;
                            }
                        }
                    }

                    if (remainingFlowValue < minFlowValue)
                    {
                        block.Drain();
                        needToFlow = false;
                    }
                    #endregion

                    #region Move to the left side
                    if ((leftBlock.IsEmptyForLiquid() || leftBlock.IsLiquid()) && needToFlow)
                    {
                        //Determine rate of flow
                        leftFlow = (leftBlock.IsLiquid() ? leftBlock.FlowValue : 0);
                        flow = (leftFlow > remainingFlowValue ? 0 : (remainingFlowValue - leftFlow) / 4f);

                        //Constrain flow
                        if (flow + leftFlow > maxFlowValue)
                        {
                            flow = (100 - leftFlow);
                        }

                        if (flow <= minForCounter)
                        {
                            flow = 0;
                        }

                        //Update flow values
                        if (flow != 0)
                        {
                            remainingFlowValue -= flow;
                            block.FlowValue -= flow;

                            //If left block is not liquid
                            if (!leftBlock.IsLiquid())
                            {
                                CreateLiquidBlock((ushort)(x - 1), y, block.LiquidId);
                                leftBlock.FlowValue = 0;
                            }
                            leftBlock.FlowValue += flow;
                            leftBlock.CountToStop = 0;
                            NeedToUpdate.Add(leftBlock.Coords);

                            if (rightBlock.IsLiquid() && rightBlock.FlowValue > block.FlowValue)
                            {
                                NeedToUpdate.Add(rightBlock.Coords);
                                rightBlock.CountToStop = 0;
                            }
                            if (topBlock.IsLiquid())
                            {
                                NeedToUpdate.Add(topBlock.Coords);
                                topBlock.CountToStop = 0;
                            }
                        }
                    }

                    if (remainingFlowValue < minFlowValue)
                    {
                        block.Drain();
                        needToFlow = false;
                    }
                    #endregion

                    #region Move to the right side
                    if ((rightBlock.IsEmptyForLiquid() || rightBlock.IsLiquid()) && needToFlow)
                    {
                        //Determine rate of flow
                        rightFlow = (rightBlock.IsLiquid() ? rightBlock.FlowValue : 0);
                        flow = (rightFlow > remainingFlowValue ? 0 : (remainingFlowValue - rightFlow) / 3f);

                        //Constrain flow
                        if (flow + rightFlow > maxFlowValue)
                        {
                            flow = (100 - rightFlow);
                        }

                        if (flow <= minForCounter)
                        {
                            flow = 0;
                        }

                        //Update flow values
                        if (flow != 0)
                        {
                            remainingFlowValue -= flow;
                            block.FlowValue -= flow;

                            //If right block is not liquid
                            if (!rightBlock.IsLiquid())
                            {
                                CreateLiquidBlock((ushort)(x + 1), y, block.LiquidId);
                                rightBlock.FlowValue = 0;
                            }
                            rightBlock.FlowValue += flow;
                            rightBlock.CountToStop = 0;
                            NeedToUpdate.Add(rightBlock.Coords);

                            if (leftBlock.IsLiquid() && leftBlock.FlowValue > block.FlowValue)
                            {
                                NeedToUpdate.Add(leftBlock.Coords);
                                leftBlock.CountToStop = 0;
                            }
                            if (topBlock.IsLiquid())
                            {
                                NeedToUpdate.Add(topBlock.Coords);
                                topBlock.CountToStop = 0;
                            }
                        }
                    }

                    if (remainingFlowValue < minFlowValue)
                    {
                        block.Drain();
                    }
                    #endregion

                    #region Top side processing
                    if (topBlock.IsLiquid())
                    {
                        block.IsFlowsDown = true;
                    }
                    else
                    {
                        block.IsFlowsDown = false;
                    }
                    if (block.IsLiquid() && bottomBlock.IsEmptyForLiquid())
                    {
                        bottomBlock.IsFlowsDown = true;
                    }
                    else
                    {
                        bottomBlock.IsFlowsDown = true;
                    }
                    #endregion

                    if (startFlowValue - remainingFlowValue < minForCounter)
                    {
                        block.CountToStop++;
                    }
                    else
                    {
                        block.CountToStop = 0;
                    }

                    if (block.CountToStop >= 100)
                    {
                        block.CountToStop = 0;
                        needToStop = true;
                    }

                    if (block.FlowValue >= 99 && block.FlowValue < 100)
                    {
                        block.FlowValue = 100f;
                    }

                    if (block.IsLiquid() && !needToStop)
                    {
                        NeedToUpdate.Add(block.Coords);
                    }
                }
            }

            if (block.BlockType == BlockTypes.Plant)
            {
                plant = block.BlockData as PlantSO;

                if (plant.IsTopBlockSolid && topBlock.IsEmptyForPlant())
                {
                    if (bottomBlock.IsPlant() && bottomBlock.CompareBlock(block.BlockData))
                    {
                        NeedToUpdate.Add(bottomBlock.Coords);
                    }
                    CreateBlock((ushort)x, (ushort)y, airBlock);
                }

                if (plant.IsBottomBlockSolid && bottomBlock.IsEmptyForPlant())
                {
                    if (topBlock.IsPlant() && topBlock.CompareBlock(block.BlockData))
                    {
                        NeedToUpdate.Add(topBlock.Coords);
                    }
                    CreateBlock((ushort)x, (ushort)y, airBlock);
                }
            }
        }
    }

    public void RandomUpdateWorldData()
    {
        ushort minX = 0;
        ushort minY = 0;
        ushort maxX = (ushort)(GameManager.Instance.CurrentTerrainWidth - 1);
        ushort maxY = (ushort)(GameManager.Instance.CurrentTerrainHeight - 1);
        int x;
        int y;
        byte chanceToAction;

        WorldCellData block = new WorldCellData();
        WorldCellData bottomBlock = new WorldCellData();
        WorldCellData topBlock = new WorldCellData();
        WorldCellData leftBlock = new WorldCellData();
        WorldCellData rightBlock = new WorldCellData();

        PlantSO plant;

        System.Random randomVar = new System.Random(GameManager.Instance.Seed);

        while (!GameManager.Instance.IsGameSession)
        {

        }
        while (GameManager.Instance.IsGameSession)
        {
            if (!GameManager.Instance.IsGameSession)
            {
                continue;
            }

            x = (ushort)randomVar.Next(minX, maxX + 1);
            y = (ushort)randomVar.Next(minY, maxY + 1);

            #region Set blocks
            block = GameManager.Instance.GetWorldCellDataRef(x, y);
            if (y - 1 >= minY)
            {
                bottomBlock = GameManager.Instance.GetWorldCellDataRef(x, y - 1);
            }
            if (y + 1 <= maxY)
            {
                topBlock = GameManager.Instance.GetWorldCellDataRef(x, y + 1);
            }
            if (x - 1 >= minX)
            {
                leftBlock = GameManager.Instance.GetWorldCellDataRef(x - 1, y);
            }
            if (x + 1 <= maxX)
            {
                rightBlock = GameManager.Instance.GetWorldCellDataRef(x + 1, y);
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
                plant = block.BlockData as PlantSO;
                if (plant.CanGrow)
                {
                    chanceToAction = (byte)randomVar.Next(0, 101);
                    if (chanceToAction > plant.ChanceToSpawn)
                    {
                        continue;
                    }
                    if (plant.IsBottomBlockSolid)
                    {
                        if (y + 1 <= maxY && topBlock.IsEmpty())
                        {
                            CreateBlock((ushort)x, (ushort)(y + 1), plant);
                        }
                    }
                    if (plant.IsTopBlockSolid)
                    {
                        if (y - 1 >= minY && bottomBlock.IsEmpty())
                        {
                            CreateBlock((ushort)x, (ushort)(y - 1), plant);
                        }
                    }
                }
            }
        }
    }

    public void RenderWorldData()
    {
        int i = 0;
        int difX;
        int difY;
        int size;

        if (_firstRender)
        {
            _prevCameraRect = GetCameraRectInt();
            _firstRender = false;
        }

        _currentCameraRect = GetCameraRectInt();

        //Calculate array size
        difX = Mathf.Abs(_prevCameraRect.x - _currentCameraRect.x);

        difY = Mathf.Abs(_prevCameraRect.y - _currentCameraRect.y);

        if (difX >= _prevCameraRect.width || difY >= _prevCameraRect.height)
        {
            size = _prevCameraRect.width * _prevCameraRect.height * 2;
        }
        else
        {
            size = _prevCameraRect.width * _prevCameraRect.height + _currentCameraRect.height * difX + _currentCameraRect.width * difY - difX * difY;
        }

        _blockTiles = _blockTilesPool.GetArray(size);
        _liquidTiles = _liquidTilesPool.GetArray(size);
        _backgroundTiles = _backgroundTilesPool.GetArray(size);
        _solidTiles = _solidTilesPool.GetArray(size);
        _tilesCoords = _tilesCoordsPool.GetArray(size);

        //Fill Tiles arrays with blocks to destroy
        Vector3Int vector = new Vector3Int();
        if (difX >= 1 || difY >= 1)
        {
            foreach (Vector2Int position in _prevCameraRect.allPositionsWithin)
            {
                if (!_currentCameraRect.Contains(position))
                {
                    vector.x = position.x;
                    vector.y = position.y;
                    _tilesCoords[i] = vector;
                    _blockTiles[i] = null;
                    _liquidTiles[i] = null;
                    _backgroundTiles[i] = null;
                    i++;
                }
            }
            _prevCameraRect = _currentCameraRect;
        }

        foreach (Vector2Int position in _currentCameraRect.allPositionsWithin)
        {
            if (IsInMapRange(position.x, position.y))
            {
                //ref WorldCellData block = ref GameManager.Instance.GetWorldCellDataRef(position.x, position.y);
                vector.x = position.x;
                vector.y = position.y;
                _tilesCoords[i] = vector;

                //Block
                _blockTiles[i] = _worldData[position.x, position.y].GetBlockTile();

                //Background
                _backgroundTiles[i] = _worldData[position.x, position.y].GetBackgroundTile();

                //Liquid
                _liquidTiles[i] = null;
                if (_worldData[position.x, position.y].IsEmptyForLiquid() && _worldData[position.x, position.y].IsLiquid())
                {
                    byte liquidIndex = (byte)(_worldData[position.x, position.y].FlowValue == 100f || _worldData[position.x, position.y].IsFlowsDown ? 9 : _worldData[position.x, position.y].FlowValue / 10);
                    _liquidTiles[i] = _allLiquidTiles[_worldData[position.x, position.y].LiquidId][liquidIndex];
                }

                //Solid
                _solidTiles[i] = null;
                if (_worldData[position.x, position.y].IsSolid())
                {
                    _solidTiles[i] = _solidTileBase;
                }
                i++;
            }
        }

        //Change Tilemap using Vector's array and Tile's array
        BlocksTilemap.SetTiles(_tilesCoords, _blockTiles);
        LiquidTilemap.SetTiles(_tilesCoords, _liquidTiles);
        BackgroundTilemap.SetTiles(_tilesCoords, _backgroundTiles);
        SolidTilemap.SetTiles(_tilesCoords, _solidTiles);
    }
    #endregion

    #endregion

    #region Helpful
    public void CreateBlock(ushort x, ushort y, BlockSO block)
    {
        _worldData[x, y].SetBlockData(block);
        if (GameManager.Instance.IsGameSession)
        {
            _worldData[x, y].SetRandomBlockTile(GameManager.Instance.RandomVar);
        }
    }

    public void CreateLiquidBlock(ushort x, ushort y, byte id)
    {
        _worldData[x, y].SetBlockData(id);
    }

    public void CreateBackground(ushort x, ushort y, BlockSO block)
    {
        _worldData[x, y].SetBackgroundData(block);
        if (GameManager.Instance.IsGameSession)
        {
            _worldData[x, y].SetRandomBackgroundTile(GameManager.Instance.RandomVar);
        }
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
    public static bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < GameManager.Instance.CurrentTerrainWidth && y >= 0 && y < GameManager.Instance.CurrentTerrainHeight;
    }
    #endregion

    #endregion
}
