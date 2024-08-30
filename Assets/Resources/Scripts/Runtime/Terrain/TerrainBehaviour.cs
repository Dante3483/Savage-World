using SavageWorld.Runtime.GameSession;
using SavageWorld.Runtime.Managers;
using SavageWorld.Runtime.Utilities;
using SavageWorld.Runtime.Utilities.Vectors;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace SavageWorld.Runtime.Terrain
{
    public class TerrainBehaviour : MonoBehaviour
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
                MainThreadUtility.Instance.InvokeAndWait(() => GameManager.Instance.Seed = GameManager.Instance.IsStaticSeed ? GameManager.Instance.Seed : UnityEngine.Random.Range(-1000000, 1000000));
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
}