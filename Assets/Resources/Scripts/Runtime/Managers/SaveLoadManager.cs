using SavageWorld.Runtime.Atlases;
using SavageWorld.Runtime.Console;
using SavageWorld.Runtime.Enums.Id;
using SavageWorld.Runtime.Enums.Others;
using SavageWorld.Runtime.Enums.Types;
using SavageWorld.Runtime.GameSession;
using SavageWorld.Runtime.Player.Inventory;
using SavageWorld.Runtime.Player.Inventory.Items;
using SavageWorld.Runtime.Player.Research;
using SavageWorld.Runtime.Terrain;
using SavageWorld.Runtime.Terrain.Blocks;
using SavageWorld.Runtime.Terrain.Objects;
using SavageWorld.Runtime.Terrain.WorldProcessingPhases;
using SavageWorld.Runtime.Utilities;
using SavageWorld.Runtime.Utilities.DebugOnly;
using SavageWorld.Runtime.Utilities.Others;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Tree = SavageWorld.Runtime.Terrain.Objects.Tree;

namespace SavageWorld.Runtime.Managers
{
    public class SaveLoadManager : Singleton<SaveLoadManager>
    {
        #region Fields
        private GameManager _gameManager;
        private WorldDataManager _worldDataManager;
        private ChunksManager _chunksManager;
        private List<int> _worldColumnIndexes;
        private int _worldDataSize;
        private float _loadingStep;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods
        private void Start()
        {
            _gameManager = GameManager.Instance;
            _worldDataManager = WorldDataManager.Instance;
            _chunksManager = ChunksManager.Instance;
        }

        private void Update()
        {
            if (!_gameManager.IsInputTextInFocus && Input.GetKeyDown(KeyCode.U))
            {
                ExecutionTimeCalculator.Instance.Execute(() => Save());
            }
        }
        #endregion

        #region Public Methods
        public void Save()
        {
            _worldColumnIndexes = new List<int>();

            string worldDirectory = StaticParameters.WorldsDirectory + $"/{_gameManager.WorldName}";
            string worldSaveFile = worldDirectory + $"/{_gameManager.WorldName}.sw.world";
            string worldMetaFile = worldDirectory + $"/{_gameManager.WorldName}.swm.world";
            string playerFile = StaticParameters.PlayersDirectory + $"/{_gameManager.PlayerName}.sw.player";

            Directory.CreateDirectory(worldDirectory);

            using (BinaryWriter binaryWriter = new(File.Open(worldSaveFile, FileMode.Create)))
            {
                Debug.Log("Save data to: " + worldSaveFile);
                SaveChunks(binaryWriter);
                SaveWorldData(binaryWriter);
                SaveTrees(binaryWriter);
                SavePickUpItems(binaryWriter);
            }

            //using (BinaryWriter binaryWriter = new(File.Open(playerFile, FileMode.Create)))
            //{
            //    Debug.Log("Save data to: " + playerFile);
            //    SavePlayer(binaryWriter);
            //}

            using (BinaryWriter binaryWriter = new(File.Open(worldMetaFile, FileMode.Create)))
            {
                Debug.Log("Save data to: " + worldMetaFile);
                SaveMetaData(binaryWriter);
            }
            Debug.Log("Saving Complete");
        }

        public void Load()
        {
            _worldColumnIndexes = new List<int>();

            string worldDirectory = StaticParameters.WorldsDirectory + $"/{_gameManager.WorldName}";
            string worldSaveFile = worldDirectory + $"/{_gameManager.WorldName}.sw.world";
            string worldMetaFile = worldDirectory + $"/{_gameManager.WorldName}.swm.world";
            string playerFile = StaticParameters.PlayersDirectory + $"/{_gameManager.PlayerName}.sw.player";

            _loadingStep = 100f / 5f;

            using (BinaryReader binaryReader = new(File.Open(worldMetaFile, FileMode.Open)))
            {
                LoadMetaData(binaryReader);
            }

            using (BinaryReader binaryReader = new(File.Open(worldSaveFile, FileMode.Open)))
            {
                LoadChunks(binaryReader);
                LoadWorldData(binaryReader);
                MainThreadUtility.Instance.InvokeAndWait(() =>
                {
                    LoadTrees(binaryReader);
                    LoadPickUpItems(binaryReader);
                });
            }

            //using (BinaryReader binaryReader = new(File.Open(playerFile, FileMode.Open)))
            //{
            //    ActionInMainThreadUtil.Instance.Invoke(() =>
            //    {
            //        LoadPlayer(binaryReader);
            //    });
            //}

            new SetPhysicsShapesPhase().StartPhase();
            Debug.Log("Loading Complete");
        }
        #endregion

        #region Private Methods
        private void SaveMetaData(BinaryWriter binaryWriter)
        {
            Debug.Log("Save meta data");
            binaryWriter.Write(_gameManager.Seed);
            binaryWriter.Write(_gameManager.CurrentTerrainWidth);
            binaryWriter.Write(_gameManager.CurrentTerrainHeight);
            binaryWriter.Write(_worldDataSize);

            foreach (var worldColumnIndex in _worldColumnIndexes)
            {
                binaryWriter.Write(worldColumnIndex);
            }
        }

        private void SaveChunks(BinaryWriter binaryWriter)
        {
            Debug.Log("Save chunks");
            foreach (var chunk in _chunksManager.Chunks)
            {
                binaryWriter.Write((byte)chunk.Biome.Id);
            }
        }

        private void SaveWorldData(BinaryWriter binaryWriter)
        {
            Debug.Log("Save world data");
            int startColumn = 0;
            for (int x = 0; x < _gameManager.CurrentTerrainWidth; x++)
            {
                _worldColumnIndexes.Add(startColumn);
                for (int y = 0; y < _gameManager.CurrentTerrainHeight; y++)
                {
                    //Flag bits:
                    //1 - BlockID
                    //2 - WallID
                    //3 - LiquidID
                    //4 - RLE
                    //5 - 
                    //6 - 
                    //7 - 
                    //8 - 
                    ushort blockId = _worldDataManager.GetBlockId(x, y);
                    ushort wallId = _worldDataManager.GetWallId(x, y);
                    byte liquidId = _worldDataManager.GetLiquidId(x, y);
                    float flowValue = _worldDataManager.GetFlowValue(x, y);
                    byte tileId = _worldDataManager.GetTileId(x, y);
                    BlockTypes type = _worldDataManager.GetBlockType(x, y);
                    byte flags = 0;

                    #region Block ID
                    if (blockId > 255)
                    {
                        flags = (byte)(flags | 1);
                    }
                    #endregion

                    #region Wall ID
                    if (wallId > 255)
                    {
                        flags = (byte)(flags | 2);
                    }
                    #endregion

                    #region Liquid ID
                    if (flowValue > 0)
                    {
                        flags = (byte)(flags | 4);
                    }
                    #endregion

                    #region RLE Algorithm
                    int iterator = 1;
                    int countOfSameObject = 0;
                    while (_gameManager.IsInMapRange(x, y + iterator))
                    {
                        ushort nextBlockId = _worldDataManager.GetBlockId(x, y + iterator);
                        ushort nextWallId = _worldDataManager.GetWallId(x, y + iterator);
                        byte nextLiquidId = _worldDataManager.GetLiquidId(x, y + iterator);
                        float nextFlowValue = _worldDataManager.GetFlowValue(x, y + iterator);
                        byte nextTileId = _worldDataManager.GetTileId(x, y + iterator);
                        BlockTypes nextType = _worldDataManager.GetBlockType(x, y + iterator);

                        if (blockId != nextBlockId)
                        {
                            break;
                        }
                        if (wallId != nextWallId)
                        {
                            break;
                        }
                        if (liquidId != nextLiquidId)
                        {
                            break;
                        }
                        if (flowValue != nextFlowValue)
                        {
                            break;
                        }
                        if (tileId != nextTileId)
                        {
                            break;
                        }
                        if (type != nextType)
                        {
                            break;
                        }
                        countOfSameObject++;
                        iterator++;
                    }

                    if (countOfSameObject != 0)
                    {
                        flags = (byte)(flags | 8);
                        y += countOfSameObject;
                    }
                    #endregion

                    #region Set data
                    startColumn += 1;
                    binaryWriter.Write(flags);

                    startColumn += 1;
                    binaryWriter.Write((byte)blockId);
                    if ((flags & 0b0000_0001) != 0)
                    {
                        startColumn += 1;
                        binaryWriter.Write((byte)(blockId >> 8));
                    }

                    startColumn += 1;
                    binaryWriter.Write((byte)wallId);
                    if ((flags & 0b0000_0010) != 0)
                    {
                        startColumn += 1;
                        binaryWriter.Write((byte)(wallId >> 8));
                    }

                    if ((flags & 0b0000_0100) != 0)
                    {
                        startColumn += 2;
                        binaryWriter.Write(liquidId);
                        binaryWriter.Write((byte)(flowValue * 2.5f));
                    }

                    startColumn += 2;
                    binaryWriter.Write(tileId);
                    binaryWriter.Write((byte)type);

                    if ((flags & 0b0000_1000) != 0)
                    {
                        startColumn += 2;
                        binaryWriter.Write((byte)countOfSameObject);
                        binaryWriter.Write((byte)(countOfSameObject >> 8));
                    }
                    #endregion
                }
            }
            _worldDataSize = startColumn;
        }

        private void SaveTrees(BinaryWriter binaryWriter)
        {
            Debug.Log("Save trees");
            binaryWriter.Write(_gameManager.Terrain.Trees.transform.childCount);
            foreach (Transform treeTransform in _gameManager.Terrain.Trees.transform)
            {
                Tree tree = treeTransform.GetComponent<Tree>();
                binaryWriter.Write((byte)tree.Id);
                binaryWriter.Write((ushort)treeTransform.position.x);
                binaryWriter.Write((ushort)treeTransform.position.y);
            }
        }

        private void SavePickUpItems(BinaryWriter binaryWriter)
        {
            Debug.Log("Save pickup items");
            binaryWriter.Write(_gameManager.Terrain.PickUpItems.transform.childCount);
            int count = 0;
            foreach (Transform pickUpItemTransform in _gameManager.Terrain.PickUpItems.transform)
            {
                PickUpItem pickUpItem = pickUpItemTransform.GetComponent<PickUpItem>();
                binaryWriter.Write((byte)pickUpItem.Id);
                binaryWriter.Write((ushort)pickUpItemTransform.position.x);
                binaryWriter.Write((ushort)pickUpItemTransform.position.y);
                count++;
            }
            GameConsole.Log($"Pickup items saved: {count}");
        }

        private void SavePlayer(BinaryWriter binaryWriter)
        {
            Debug.Log("Save player");
            SavePlayerPosition(binaryWriter);
            SavePlayerInventory(binaryWriter);
            SavePlayerResearches(binaryWriter);
        }

        private void SavePlayerPosition(BinaryWriter binaryWriter)
        {
            Vector3 position = _gameManager.GetPlayerTransform().position;
            binaryWriter.Write(position.x);
            binaryWriter.Write(position.y);
        }

        private void SavePlayerInventory(BinaryWriter binaryWriter)
        {
            InventoryModel inventory = _gameManager.GetPlayerInventory();
            SavePlayerInventoryPart(binaryWriter, inventory, inventory.HotbarSize, ItemLocations.Hotbar);
            SavePlayerInventoryPart(binaryWriter, inventory, inventory.StorageSize, ItemLocations.Storage);
            SavePlayerInventoryPart(binaryWriter, inventory, inventory.AccessoriesSize, ItemLocations.Accessories);
            SavePlayerInventoryPart(binaryWriter, inventory, inventory.ArmorSize, ItemLocations.Armor);
        }

        private void SavePlayerInventoryPart(BinaryWriter binaryWriter, InventoryModel inventory, int size, ItemLocations location)
        {
            for (int i = 0; i < size; i++)
            {
                ItemSO data = inventory.GetItemData(i, location);
                int quantity = inventory.GetItemQuantity(i, location);
                bool isEmpty = data is null;
                binaryWriter.Write(isEmpty);
                if (!isEmpty)
                {
                    binaryWriter.Write((ushort)data.Id);
                    binaryWriter.Write((ushort)quantity);
                }
            }
        }

        private void SavePlayerResearches(BinaryWriter binaryWriter)
        {
            ResearchesModelSO researches = _gameManager.GetResearches();
            for (int i = 0; i < researches.GetResearchesCount(); i++)
            {
                binaryWriter.Write((byte)researches.GetState(i));
            }
        }

        private void LoadMetaData(BinaryReader binaryReader)
        {
            Debug.Log("Load meta data");
            _gameManager.Seed = binaryReader.ReadInt32();
            _gameManager.CurrentTerrainWidth = binaryReader.ReadInt32();
            _gameManager.CurrentTerrainHeight = binaryReader.ReadInt32();
            _worldDataSize = binaryReader.ReadInt32();

            for (int x = 0; x < _gameManager.CurrentTerrainWidth; x++)
            {
                _worldColumnIndexes.Add(binaryReader.ReadInt32());
            }
            _gameManager.LoadingValue += _loadingStep;
        }

        private void LoadChunks(BinaryReader binaryReader)
        {
            Debug.Log("Load chunks");
            TerrainConfigurationSO terrainConfiguration = _gameManager.TerrainConfiguration;
            foreach (var chunk in _chunksManager.Chunks)
            {
                _chunksManager.SetChunkBiome(chunk, terrainConfiguration.GetBiome((BiomesId)binaryReader.ReadByte()));
            }
            _gameManager.LoadingValue += _loadingStep;
        }

        private void LoadWorldData(BinaryReader binaryReader)
        {
            Debug.Log("Load world data");
            BlocksAtlasSO blockAtlas = _gameManager.BlocksAtlas;

            byte[] worldData = binaryReader.ReadBytes(_worldDataSize);
            try
            {
                Parallel.For(0, _gameManager.CurrentTerrainWidth, index =>
                {
                    int x = index;
                    int byteIndex = 0;
                    int start = _worldColumnIndexes[x];

                    byte flags;
                    byte tileId;
                    byte liquidId;
                    ushort blockId;
                    ushort wallId;
                    int count;
                    float flowValue;
                    BlockTypes blockType;
                    BlockSO block;
                    BlockSO wall;
                    BlockSO liquid;

                    for (int y = 0; y < _gameManager.CurrentTerrainHeight;)
                    {
                        count = 1;

                        flags = worldData[start + byteIndex++];

                        if ((flags & 0b0000_0001) != 0)
                        {
                            blockId = worldData[start + byteIndex++];
                            blockId |= (ushort)(worldData[start + byteIndex++] << 8);
                        }
                        else
                        {
                            blockId = worldData[start + byteIndex++];
                        }

                        if ((flags & 0b0000_0010) != 0)
                        {
                            wallId = worldData[start + byteIndex++];
                            wallId = (ushort)(worldData[start + byteIndex++] << 8);
                        }
                        else
                        {
                            wallId = worldData[start + byteIndex++];
                        }

                        if ((flags & 0b0000_0100) != 0)
                        {
                            liquidId = worldData[start + byteIndex++];
                            flowValue = worldData[start + byteIndex++] / 2.5f;
                        }
                        else
                        {
                            liquidId = byte.MaxValue;
                            flowValue = 0f;
                        }

                        tileId = worldData[start + byteIndex++];
                        blockType = (BlockTypes)worldData[start + byteIndex++];

                        if ((flags & 0b0000_1000) != 0)
                        {
                            count = worldData[start + byteIndex++];
                            count |= worldData[start + byteIndex++] << 8;
                            count++;
                        }

                        block = blockAtlas.GetBlockByTypeAndId(blockType, blockId);
                        wall = blockAtlas.GetBlockByTypeAndId(BlockTypes.Wall, wallId);
                        liquid = liquidId == byte.MaxValue ? null : blockAtlas.GetBlockById(liquidId);

                        for (int i = 0; i < count; i++)
                        {
                            _worldDataManager.SetBlockData(x, y + i, block);
                            _worldDataManager.SetWallData(x, y + i, wall);
                            _worldDataManager.SetLiquidData(x, y + i, liquid, flowValue);
                            _worldDataManager.SetTileId(x, y + i, tileId);
                        }

                        y += count;
                    }
                });
                _gameManager.LoadingValue += _loadingStep;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        private void LoadTrees(BinaryReader binaryReader)
        {
            Debug.Log("Load trees");
            foreach (Transform child in _gameManager.Terrain.Trees.transform)
            {
                Destroy(child.gameObject);
            }

            int treesCount = binaryReader.ReadInt32();
            Vector3 position = new();

            for (int i = 0; i < treesCount; i++)
            {
                TreesId treeId = (TreesId)binaryReader.ReadByte();
                position.x = binaryReader.ReadUInt16();
                position.y = binaryReader.ReadUInt16();
                _gameManager.TreesAtlas.GetTreeById(treeId).CreateInstance(position);
            }
            _gameManager.LoadingValue += _loadingStep;
        }

        private void LoadPickUpItems(BinaryReader binaryReader)
        {
            Debug.Log("Load pickup items");
            foreach (Transform child in _gameManager.Terrain.PickUpItems.transform)
            {
                Destroy(child.gameObject);
            }

            int pickUpItemsCount = binaryReader.ReadInt32();
            Vector3Int position = new();

            for (int i = 0; i < pickUpItemsCount; i++)
            {
                PickUpItemsId pickUpItemId = (PickUpItemsId)binaryReader.ReadByte();
                position.x = binaryReader.ReadUInt16();
                position.y = binaryReader.ReadUInt16();
                _gameManager.PickUpItemsAtlas.GetPickUpItemById(pickUpItemId).CreateInstance(position);
            }
            _gameManager.LoadingValue += _loadingStep;
        }

        private void LoadPlayer(BinaryReader binaryReader)
        {
            Debug.Log("Load player");
            LoadPlayerPosition(binaryReader);
            LoadPlayerInventory(binaryReader);
            LoadPlayerResearches(binaryReader);
        }

        private void LoadPlayerPosition(BinaryReader binaryReader)
        {
            float x = binaryReader.ReadSingle();
            float y = binaryReader.ReadSingle();
            _gameManager.SetPlayerPosition(x, y);
        }

        private void LoadPlayerInventory(BinaryReader binaryReader)
        {
            InventoryModel inventory = _gameManager.GetPlayerInventory();
            LoadPlayerInventoryPart(binaryReader, inventory, inventory.HotbarSize, ItemLocations.Hotbar);
            LoadPlayerInventoryPart(binaryReader, inventory, inventory.StorageSize, ItemLocations.Storage);
            LoadPlayerInventoryPart(binaryReader, inventory, inventory.AccessoriesSize, ItemLocations.Accessories);
            LoadPlayerInventoryPart(binaryReader, inventory, inventory.ArmorSize, ItemLocations.Armor);
        }

        private void LoadPlayerInventoryPart(BinaryReader binaryReader, InventoryModel inventory, int size, ItemLocations location)
        {
            for (int i = 0; i < size; i++)
            {
                bool isEmpty = binaryReader.ReadBoolean();
                if (!isEmpty)
                {
                    ItemsId id = (ItemsId)binaryReader.ReadUInt16();
                    int quantity = binaryReader.ReadUInt16();
                    ItemSO itemData = _gameManager.ItemsAtlas.GetItemByID(id);
                    inventory.AddItemToEmptyCellByIndex(itemData, quantity, i, location);
                }
            }
        }

        private void LoadPlayerResearches(BinaryReader binaryReader)
        {
            ResearchesModelSO researches = _gameManager.GetResearches();
            researches.Initialize();
            for (int i = 0; i < researches.GetResearchesCount(); i++)
            {
                ResearchState state = (ResearchState)binaryReader.ReadByte();
                researches.SetResearchState(i, state);
            }
        }
        #endregion
    }
}