using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Terrain : MonoBehaviour
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private int _renderWidth;
    [SerializeField] private int _renderHeight;

    [Header("Tilemaps")]
    [SerializeField] private SolidRuleTile _solidRuleTIle;
    [SerializeField] private CornerRuleTile _cornerRuleTile;
    [SerializeField] private Tilemap _solidTilemap;
    [SerializeField] private CustomTilemap.Tilemap _blocksTilemap;

    [Header("Sections")]
    [SerializeField] private GameObject _trees;
    [SerializeField] private GameObject _pickUpItems;

    private WorldCellData[,] _worldData;

    #region World data update
    private HashSet<Vector2Ushort> _needToUpdate;
    private List<Vector2Ushort> vectors = new List<Vector2Ushort>();
    #endregion

    #region World data render
    private bool _firstRender;
    private RectInt _currentCameraRect;
    private RectInt _prevCameraRect;
    private Vector3Int[] _tilesCoords;
    private TileBase[] _solidTiles;
    private PoolForDynamicEmptyArraysGeneric<TileBase> _solidTilesPool;
    private PoolForDynamicEmptyArraysGeneric<Vector3Int> _tilesCoordsPool;
    private List<List<TileBase>> _allLiquidTiles;
    #endregion

    #region Threads
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

    public GameObject PickUpItems
    {
        get
        {
            return _pickUpItems;
        }

        set
        {
            _pickUpItems = value;
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
    #endregion

    #region Methods

    #region General
    private void Awake()
    {
        #region Setup tilemaps
        _solidTilemap = transform.Find("SolidTilemap").GetComponent<Tilemap>();
        if (_solidTilemap == null)
        {
            throw new NullReferenceException("SolidTilemap is null");
        }

        _blocksTilemap = transform.Find("BlocksTilemap").GetComponent<CustomTilemap.Tilemap>();
        if (_blocksTilemap == null)
        {
            throw new NullReferenceException("BlocksTilemap is null");
        }
        #endregion

        #region Setup sections
        Trees = transform.Find("Trees").gameObject;
        if (Trees == null)
        {
            throw new NullReferenceException("Trees is null");
        }

        PickUpItems = transform.Find("PickUpItems").gameObject;
        if (PickUpItems == null)
        {
            throw new NullReferenceException("PickUpItems is null");
        }
        #endregion

        #region Initialization
        NeedToUpdate = new HashSet<Vector2Ushort>();
        _currentCameraRect = new RectInt();
        _prevCameraRect = new RectInt();
        _tilesCoords = new Vector3Int[_renderWidth * _renderHeight];
        _solidTiles = new TileBase[_renderWidth * _renderHeight];
        _solidTilesPool = new PoolForDynamicEmptyArraysGeneric<TileBase>();
        _tilesCoordsPool = new PoolForDynamicEmptyArraysGeneric<Vector3Int>();
        _firstRender = true;
        #endregion
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsGameSession)
        {
            UpdateWorldData();
        }
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameSession)
        {
            RenderWorldData();
        }
    }

    public void Initialize()
    {
        _worldData = WorldDataManager.Instance.WorldData;
    }

    public void CreateNewWorld()
    {
        try
        {
            GameManager.Instance.RandomVar = new System.Random(GameManager.Instance.Seed);

            TerrainGeneration terrainGeneration = new TerrainGeneration();
            terrainGeneration.StartTerrainGeneration();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public void LoadWorld()
    {
        try
        {
            SaveLoadManager.Instance.Load();
            GameManager.Instance.RandomVar = new System.Random(GameManager.Instance.Seed);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public void StartCoroutinesAndThreads()
    {
        //Start random block processing
        _randomBlockProcessingThread = new Thread(RandomUpdateWorldData);
        _randomBlockProcessingThread.Start();
    }
    #endregion

    #region Update
    public void UpdateWorldData()
    {
        vectors.Clear();
        vectors.AddRange(NeedToUpdate);
        NeedToUpdate.Clear();

        BlockSO airBlock = GameManager.Instance.BlocksAtlas.Air;
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
            ref WorldCellData block = ref WorldDataManager.Instance.GetWorldCellData(x, y);
            ref WorldCellData bottomBlock = ref WorldDataManager.Instance.GetWorldCellData(x, y - 1);
            ref WorldCellData topBlock = ref WorldDataManager.Instance.GetWorldCellData(x, y + 1);
            ref WorldCellData leftBlock = ref WorldDataManager.Instance.GetWorldCellData(x - 1, y);
            ref WorldCellData rightBlock = ref WorldDataManager.Instance.GetWorldCellData(x + 1, y);
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
            block = WorldDataManager.Instance.GetWorldCellData(x, y);
            if (y - 1 >= minY)
            {
                bottomBlock = WorldDataManager.Instance.GetWorldCellData(x, y - 1);
            }
            if (y + 1 <= maxY)
            {
                topBlock = WorldDataManager.Instance.GetWorldCellData(x, y + 1);
            }
            if (x - 1 >= minX)
            {
                leftBlock = WorldDataManager.Instance.GetWorldCellData(x - 1, y);
            }
            if (x + 1 <= maxX)
            {
                rightBlock = WorldDataManager.Instance.GetWorldCellData(x + 1, y);
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
        int difX;
        int difY;

        if (_firstRender)
        {
            _prevCameraRect = GetCameraRectInt();
            _firstRender = false;
        }

        _currentCameraRect = GetCameraRectInt();

        //Calculate prev and current camera rect differences
        difX = Mathf.Abs(_prevCameraRect.x - _currentCameraRect.x);
        difY = Mathf.Abs(_prevCameraRect.y - _currentCameraRect.y);

        //ExecutionTimeCalculator.Instance.Execute(() => FillSolidTilemap(difX, difY));
        FillTilemaps(difX, difY); //Execution time 4.5 ms

        _prevCameraRect = _currentCameraRect;
    }

    public void FillTilemaps(int difX, int difY)
    {
        CustomTilemap.TileSprites tileSprites = new CustomTilemap.TileSprites();
        int i = 0;
        int size;

        //Calculate array size
        if (difX >= _prevCameraRect.width || difY >= _prevCameraRect.height)
        {
            size = _prevCameraRect.width * _prevCameraRect.height * 2;
        }
        else
        {
            size = _prevCameraRect.width * _prevCameraRect.height + _currentCameraRect.height * difX + _currentCameraRect.width * difY - difX * difY;
        }

        _solidTiles = _solidTilesPool.GetArray(size);
        _tilesCoords = _tilesCoordsPool.GetArray(size);

        //Fill Tiles array with blocks to destroy
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
                    _solidTiles[i] = null;
                    i++;
                }
            }
        }

        foreach (Vector2Int position in _currentCameraRect.allPositionsWithin)
        {
            if (IsInMapRange(position.x, position.y))
            {
                //Coords
                vector.x = position.x;
                vector.y = position.y;
                _tilesCoords[i] = vector;

                //Block
                tileSprites.BlockSprite = _worldData[position.x, position.y].GetBlockSprite();

                //Background
                tileSprites.BackgroundSprite = _worldData[position.x, position.y].GetBackgroundSprite();

                //Liquid
                tileSprites.LiquidSprite = null;
                if (_worldData[position.x, position.y].IsEmptyForLiquid() && _worldData[position.x, position.y].IsLiquid())
                {
                    tileSprites.LiquidSprite = _worldData[position.x, position.y].GetLiquidSprite();
                }
                _blocksTilemap.SetTile(vector, tileSprites);

                //Solid
                _solidTiles[i] = null;
                if (_worldData[position.x, position.y].IsSolid())
                {
                    _solidTiles[i] = _solidRuleTIle;
                }
                else if (_worldData[position.x, position.y - 1].IsSolid())
                {
                    bool isLeftSolid = _worldData[position.x - 1, position.y].IsSolid();
                    bool isRightSolid = _worldData[position.x + 1, position.y].IsSolid();
                    if ((isLeftSolid && !isRightSolid) || (!isLeftSolid && isRightSolid))
                    {
                        _solidTiles[i] = _cornerRuleTile;
                    }
                }
                i++;
            }
        }
        //Change Tilemap using Vector's array and Tile's array
        _solidTilemap.SetTiles(_tilesCoords, _solidTiles);
    }
    #endregion

    #region Helpful
    public void CreateBlock(int x, int y, BlockSO block)
    {
        if (block == null)
        {
            return;
        }
        _worldData[x, y].SetBlockData(block);
        if (GameManager.Instance.IsGameSession && !GameManager.Instance.IsWorldLoading)
        {
            _worldData[x, y].SetRandomBlockTile(GameManager.Instance.RandomVar);
        }
    }

    public void CreateBackground(int x, int y, BlockSO block)
    {
        _worldData[x, y].SetBackgroundData(block);
        if (GameManager.Instance.IsGameSession && !GameManager.Instance.IsWorldLoading)
        {
            _worldData[x, y].SetRandomBackgroundTile(GameManager.Instance.RandomVar);
        }
    }

    public void CreateLiquidBlock(int x, int y, byte id)
    {
        _worldData[x, y].SetLiquidBlockData(id);
    }

    public void CreateLiquidBlock(int x, int y, byte id, float flowValue)
    {
        _worldData[x, y].SetLiquidBlockData(id, flowValue);
    }

    public void SetTileId(int x, int y, byte tileId)
    {
        _worldData[x, y].TileId = tileId;
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
