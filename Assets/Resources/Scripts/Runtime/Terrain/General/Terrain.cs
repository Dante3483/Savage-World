using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    #region Private fields
    [Header("Sections")]
    [SerializeField]
    private GameObject _trees;
    [SerializeField]
    private GameObject _pickUpItems;

    #region World data update
    private HashSet<Vector2Ushort> _needToUpdate;
    private List<Vector2Ushort> vectors = new();
    #endregion

    #region Threads
    private Thread _randomBlockProcessingThread;
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
        NeedToUpdate = new HashSet<Vector2Ushort>();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsPlayingState)
        {
            //UpdateWorldData();
        }
    }

    public void Initialize()
    {

    }

    public void CreateNewWorld()
    {
        try
        {
            ActionInMainThreadUtil.Instance.Invoke(() => GameManager.Instance.Seed = GameManager.Instance.IsStaticSeed ? GameManager.Instance.Seed : UnityEngine.Random.Range(-1000000, 1000000));
            GameManager.Instance.RandomVar = new System.Random(GameManager.Instance.Seed);
            TerrainGeneration terrainGeneration = new();
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
        //_randomBlockProcessingThread = new Thread(RandomUpdateWorldData);
        //_randomBlockProcessingThread.Start();
    }
    #endregion

    #region Update
    //public void UpdateWorldData()
    //{
    //    vectors.Clear();
    //    vectors.AddRange(NeedToUpdate);
    //    NeedToUpdate.Clear();

    //    BlockSO airBlock = GameManager.Instance.BlocksAtlas.Air;
    //    PlantSO plant;

    //    float minFlowValue = 0.1f;
    //    float maxFlowValue = 100f;
    //    float minForCounter = 0.01f;
    //    float startFlowValue;
    //    float remainingFlowValue;
    //    float flow;
    //    float bottomFlow;
    //    float leftFlow;
    //    float rightFlow;
    //    bool needToFlow;
    //    bool needToStop = false;

    //    foreach (Vector2Ushort coord in vectors)
    //    {
    //        ushort x = coord.x;
    //        ushort y = coord.y;

    //        #region Set blocks
    //        ref WorldCellData block = ref WorldDataManager.Instance.GetWorldCellData(x, y);
    //        ref WorldCellData bottomBlock = ref WorldDataManager.Instance.GetWorldCellData(x, y - 1);
    //        ref WorldCellData topBlock = ref WorldDataManager.Instance.GetWorldCellData(x, y + 1);
    //        ref WorldCellData leftBlock = ref WorldDataManager.Instance.GetWorldCellData(x - 1, y);
    //        ref WorldCellData rightBlock = ref WorldDataManager.Instance.GetWorldCellData(x + 1, y);
    //        #endregion

    //        if (!block.BlockData.IsWaterproof && block.IsLiquid)
    //        {
    //            CreateBlock(x, y, airBlock);
    //        }

    //        if (block.BlockType == BlockTypes.Abstract)
    //        {

    //        }

    //        if (block.BlockType == BlockTypes.Solid)
    //        {

    //        }

    //        if (block.BlockType == BlockTypes.Dust)
    //        {
    //            if (block.CurrentActionTime < block.GetDustFallingTime())
    //            {
    //                block.CurrentActionTime++;
    //                //NeedToUpdate.Add(block.Coords);
    //            }
    //            else
    //            {
    //                block.CurrentActionTime = 0;
    //                if (bottomBlock.IsEmpty)
    //                {
    //                    CreateBlock(x, y - 1, block.BlockData);
    //                    CreateBlock(x, y, airBlock);
    //                    //NeedToUpdate.Add(bottomBlock.Coords);
    //                    if (topBlock.IsDust)
    //                    {
    //                        //NeedToUpdate.Add(topBlock.Coords);
    //                    }
    //                }
    //            }
    //        }

    //        if (block.IsLiquid && block.IsValidForLiquid)
    //        {
    //            if (block.CurrentActionTime < block.GetLiquidFlowTime())
    //            {
    //                block.CurrentActionTime++;
    //            }
    //            else
    //            {
    //                block.CurrentActionTime = 0;
    //                startFlowValue = block.FlowValue;
    //                remainingFlowValue = startFlowValue;
    //                needToFlow = true;

    //                #region Move to the bottom side
    //                if (bottomBlock.IsValidForLiquid || (bottomBlock.IsLiquid && bottomBlock.FlowValue != 100))
    //                {
    //                    //Determine rate of flow
    //                    bottomFlow = (bottomBlock.IsLiquid ? bottomBlock.FlowValue : 0);
    //                    flow = block.FlowValue;

    //                    //Constrain flow
    //                    if (flow + bottomFlow > maxFlowValue)
    //                    {
    //                        flow = (100 - bottomFlow);
    //                    }

    //                    //Update flow values
    //                    if (flow != 0)
    //                    {
    //                        remainingFlowValue -= flow;
    //                        block.FlowValue -= flow;

    //                        //If bottom block is not liquid
    //                        if (!bottomBlock.IsLiquid)
    //                        {
    //                            CreateLiquidBlock(x, (ushort)(y - 1), block.LiquidId);
    //                            bottomBlock.FlowValue = 0.0f;
    //                            if (leftBlock.IsLiquid)
    //                            {
    //                                //NeedToUpdate.Add(leftBlock.Coords);
    //                                leftBlock.CountToStop = 0;
    //                            }
    //                            if (rightBlock.IsLiquid)
    //                            {
    //                                //NeedToUpdate.Add(rightBlock.Coords);
    //                                rightBlock.CountToStop = 0;
    //                            }
    //                        }
    //                        bottomBlock.FlowValue += flow;
    //                        bottomBlock.IsFlowsDown = true;
    //                        bottomBlock.CountToStop = 0;
    //                        //NeedToUpdate.Add(bottomBlock.Coords);
    //                        if (topBlock.IsLiquid)
    //                        {
    //                            //NeedToUpdate.Add(topBlock.Coords);
    //                            topBlock.CountToStop = 0;
    //                        }
    //                    }
    //                }

    //                if (remainingFlowValue < minFlowValue)
    //                {
    //                    block.Drain();
    //                    needToFlow = false;
    //                }
    //                #endregion

    //                #region Move to the left side
    //                if ((leftBlock.IsValidForLiquid || leftBlock.IsLiquid) && needToFlow)
    //                {
    //                    //Determine rate of flow
    //                    leftFlow = (leftBlock.IsLiquid ? leftBlock.FlowValue : 0);
    //                    flow = (leftFlow > remainingFlowValue ? 0 : (remainingFlowValue - leftFlow) / 4f);

    //                    //Constrain flow
    //                    if (flow + leftFlow > maxFlowValue)
    //                    {
    //                        flow = (100 - leftFlow);
    //                    }

    //                    if (flow <= minForCounter)
    //                    {
    //                        flow = 0;
    //                    }

    //                    //Update flow values
    //                    if (flow != 0)
    //                    {
    //                        remainingFlowValue -= flow;
    //                        block.FlowValue -= flow;

    //                        //If left block is not liquid
    //                        if (!leftBlock.IsLiquid)
    //                        {
    //                            CreateLiquidBlock((ushort)(x - 1), y, block.LiquidId);
    //                            leftBlock.FlowValue = 0;
    //                        }
    //                        leftBlock.FlowValue += flow;
    //                        leftBlock.CountToStop = 0;
    //                        //NeedToUpdate.Add(leftBlock.Coords);

    //                        if (rightBlock.IsLiquid && rightBlock.FlowValue > block.FlowValue)
    //                        {
    //                            //NeedToUpdate.Add(rightBlock.Coords);
    //                            rightBlock.CountToStop = 0;
    //                        }
    //                        if (topBlock.IsLiquid)
    //                        {
    //                            //NeedToUpdate.Add(topBlock.Coords);
    //                            topBlock.CountToStop = 0;
    //                        }
    //                    }
    //                }

    //                if (remainingFlowValue < minFlowValue)
    //                {
    //                    block.Drain();
    //                    needToFlow = false;
    //                }
    //                #endregion

    //                #region Move to the right side
    //                if ((rightBlock.IsValidForLiquid || rightBlock.IsLiquid) && needToFlow)
    //                {
    //                    //Determine rate of flow
    //                    rightFlow = (rightBlock.IsLiquid ? rightBlock.FlowValue : 0);
    //                    flow = (rightFlow > remainingFlowValue ? 0 : (remainingFlowValue - rightFlow) / 3f);

    //                    //Constrain flow
    //                    if (flow + rightFlow > maxFlowValue)
    //                    {
    //                        flow = (100 - rightFlow);
    //                    }

    //                    if (flow <= minForCounter)
    //                    {
    //                        flow = 0;
    //                    }

    //                    //Update flow values
    //                    if (flow != 0)
    //                    {
    //                        remainingFlowValue -= flow;
    //                        block.FlowValue -= flow;

    //                        //If right block is not liquid
    //                        if (!rightBlock.IsLiquid)
    //                        {
    //                            CreateLiquidBlock((ushort)(x + 1), y, block.LiquidId);
    //                            rightBlock.FlowValue = 0;
    //                        }
    //                        rightBlock.FlowValue += flow;
    //                        rightBlock.CountToStop = 0;
    //                        //NeedToUpdate.Add(rightBlock.Coords);

    //                        if (leftBlock.IsLiquid && leftBlock.FlowValue > block.FlowValue)
    //                        {
    //                            //NeedToUpdate.Add(leftBlock.Coords);
    //                            leftBlock.CountToStop = 0;
    //                        }
    //                        if (topBlock.IsLiquid)
    //                        {
    //                            //NeedToUpdate.Add(topBlock.Coords);
    //                            topBlock.CountToStop = 0;
    //                        }
    //                    }
    //                }

    //                if (remainingFlowValue < minFlowValue)
    //                {
    //                    block.Drain();
    //                }
    //                #endregion

    //                #region Top side processing
    //                if (topBlock.IsLiquid)
    //                {
    //                    block.IsFlowsDown = true;
    //                }
    //                else
    //                {
    //                    block.IsFlowsDown = false;
    //                }
    //                if (block.IsLiquid && bottomBlock.IsValidForLiquid)
    //                {
    //                    bottomBlock.IsFlowsDown = true;
    //                }
    //                else
    //                {
    //                    bottomBlock.IsFlowsDown = true;
    //                }
    //                #endregion

    //                if (startFlowValue - remainingFlowValue < minForCounter)
    //                {
    //                    block.CountToStop++;
    //                }
    //                else
    //                {
    //                    block.CountToStop = 0;
    //                }

    //                if (block.CountToStop >= 100)
    //                {
    //                    block.CountToStop = 0;
    //                    needToStop = true;
    //                }

    //                if (block.FlowValue >= 99 && block.FlowValue < 100)
    //                {
    //                    block.FlowValue = 100f;
    //                }

    //                if (block.IsLiquid && !needToStop)
    //                {
    //                    //NeedToUpdate.Add(block.Coords);
    //                }
    //            }
    //        }

    //        if (block.BlockType == BlockTypes.Plant)
    //        {
    //            plant = block.BlockData as PlantSO;

    //            if (plant.IsTopBlockSolid && topBlock.IsValidForPlant)
    //            {
    //                if (bottomBlock.IsPlant && bottomBlock.CompareBlock(block.BlockData))
    //                {
    //                    //NeedToUpdate.Add(bottomBlock.Coords);
    //                }
    //                CreateBlock(x, y, airBlock);
    //            }

    //            if (plant.IsBottomBlockSolid && bottomBlock.IsValidForPlant)
    //            {
    //                if (topBlock.IsPlant && topBlock.CompareBlock(block.BlockData))
    //                {
    //                    //NeedToUpdate.Add(topBlock.Coords);
    //                }
    //                CreateBlock(x, y, airBlock);
    //            }
    //        }
    //    }
    //}

    //public void RandomUpdateWorldData()
    //{
    //    ushort minX = 0;
    //    ushort minY = 0;
    //    ushort maxX = (ushort)(GameManager.Instance.CurrentTerrainWidth - 1);
    //    ushort maxY = (ushort)(GameManager.Instance.CurrentTerrainHeight - 1);
    //    int x;
    //    int y;
    //    byte chanceToAction;

    //    WorldCellData block = new();
    //    WorldCellData bottomBlock = new();
    //    WorldCellData topBlock = new();
    //    WorldCellData leftBlock = new();
    //    WorldCellData rightBlock = new();

    //    PlantSO plant;

    //    System.Random randomVar = new(GameManager.Instance.Seed);

    //    while (!GameManager.Instance.IsGameSession)
    //    {

    //    }
    //    while (GameManager.Instance.IsGameSession)
    //    {
    //        if (!GameManager.Instance.IsGameSession)
    //        {
    //            continue;
    //        }

    //        x = (ushort)randomVar.Next(minX, maxX + 1);
    //        y = (ushort)randomVar.Next(minY, maxY + 1);

    //        #region Set blocks
    //        block = WorldDataManager.Instance.GetWorldCellData(x, y);
    //        if (y - 1 >= minY)
    //        {
    //            bottomBlock = WorldDataManager.Instance.GetWorldCellData(x, y - 1);
    //        }
    //        if (y + 1 <= maxY)
    //        {
    //            topBlock = WorldDataManager.Instance.GetWorldCellData(x, y + 1);
    //        }
    //        if (x - 1 >= minX)
    //        {
    //            leftBlock = WorldDataManager.Instance.GetWorldCellData(x - 1, y);
    //        }
    //        if (x + 1 <= maxX)
    //        {
    //            rightBlock = WorldDataManager.Instance.GetWorldCellData(x + 1, y);
    //        }
    //        #endregion

    //        if (block.BlockType == BlockTypes.Abstract)
    //        {

    //        }

    //        if (block.BlockType == BlockTypes.Solid)
    //        {

    //        }

    //        if (block.BlockType == BlockTypes.Dust)
    //        {

    //        }

    //        if (block.BlockType == BlockTypes.Liquid)
    //        {

    //        }

    //        if (block.BlockType == BlockTypes.Plant)
    //        {
    //            plant = block.BlockData as PlantSO;
    //            if (plant.CanGrow)
    //            {
    //                chanceToAction = (byte)randomVar.Next(0, 101);
    //                if (chanceToAction > plant.ChanceToSpawn)
    //                {
    //                    continue;
    //                }
    //                if (plant.IsBottomBlockSolid)
    //                {
    //                    if (y + 1 <= maxY && topBlock.IsEmpty)
    //                    {
    //                        CreateBlock(x, y + 1, plant);
    //                    }
    //                }
    //                if (plant.IsTopBlockSolid)
    //                {
    //                    if (y - 1 >= minY && bottomBlock.IsEmpty)
    //                    {
    //                        CreateBlock(x, y - 1, plant);
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}
    #endregion

    #region Helpful

    #endregion

    #region Valid

    #endregion

    #endregion
}
