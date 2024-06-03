using Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    #region Private fields
    private List<int> _worldColumnIndexes;
    private int _worldDataSize;
    private float _loadingStep;
    #endregion

    #region Public fields
    public static SaveLoadManager Instance;
    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!GameManager.Instance.IsInputTextInFocus && Input.GetKeyDown(KeyCode.V))
        {
            ExecutionTimeCalculator.Instance.Execute(() => Save());
        }
    }

    private void SaveMetaData(BinaryWriter binaryWriter)
    {
        Debug.Log("Save meta data");
        binaryWriter.Write(GameManager.Instance.Seed);
        binaryWriter.Write(GameManager.Instance.CurrentTerrainWidth);
        binaryWriter.Write(GameManager.Instance.CurrentTerrainHeight);
        binaryWriter.Write(_worldDataSize);

        foreach (var worldColumnIndex in _worldColumnIndexes)
        {
            binaryWriter.Write(worldColumnIndex);
        }
    }

    private void SaveChunks(BinaryWriter binaryWriter)
    {
        Debug.Log("Save chunks");
        foreach (var chunk in ChunksManager.Instance.Chunks)
        {
            binaryWriter.Write((byte)chunk.Biome.Id);
        }
    }

    private void SaveWorldData(BinaryWriter binaryWriter)
    {
        Debug.Log("Save world data");
        int startColumn = 0;
        for (int x = 0; x < GameManager.Instance.CurrentTerrainWidth; x++)
        {
            _worldColumnIndexes.Add(startColumn);
            for (int y = 0; y < GameManager.Instance.CurrentTerrainHeight; y++)
            {
                ref WorldCellData data = ref WorldDataManager.Instance.WorldData[x, y];

                //Flag bits:
                //1 - BlockID
                //2 - WallID
                //3 - LiquidID
                //4 - RLE
                //5 - 
                //6 - 
                //7 - 
                //8 - 
                byte flags = 0;

                #region Block ID
                if (data.BlockId > 255)
                {
                    flags = (byte)(flags | 1);
                }
                #endregion

                #region Wall ID
                if (data.WallId > 255)
                {
                    flags = (byte)(flags | 2);
                }
                #endregion

                #region Liquid ID
                if (data.FlowValue > 0)
                {
                    flags = (byte)(flags | 4);
                }
                #endregion

                #region RLE Algorithm
                int iterator = 1;
                int countOfSameObject = 0;
                while (Terrain.IsInMapRange(x, y + iterator))
                {
                    ref WorldCellData nextData = ref WorldDataManager.Instance.WorldData[x, y + iterator];
                    if (data.BlockId != nextData.BlockId)
                    {
                        break;
                    }
                    if (data.WallId != nextData.WallId)
                    {
                        break;
                    }
                    if (data.LiquidId != nextData.LiquidId)
                    {
                        break;
                    }
                    if (data.FlowValue != nextData.FlowValue)
                    {
                        break;
                    }
                    if (data.TileId != nextData.TileId)
                    {
                        break;
                    }
                    if (data.BlockType != nextData.BlockType)
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
                binaryWriter.Write((byte)(data.BlockId));
                if ((flags & 0b0000_0001) != 0)
                {
                    startColumn += 1;
                    binaryWriter.Write((byte)(data.BlockId >> 8));
                }

                startColumn += 1;
                binaryWriter.Write((byte)(data.WallId));
                if ((flags & 0b0000_0010) != 0)
                {
                    startColumn += 1;
                    binaryWriter.Write((byte)(data.WallId >> 8));
                }

                if ((flags & 0b0000_0100) != 0)
                {
                    startColumn += 2;
                    binaryWriter.Write(data.LiquidId);
                    binaryWriter.Write((byte)(data.FlowValue * 2.5f));
                }

                startColumn += 2;
                binaryWriter.Write(data.TileId);
                binaryWriter.Write((byte)data.BlockType);

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
        binaryWriter.Write(GameManager.Instance.Terrain.Trees.transform.childCount);
        foreach (Transform treeTransform in GameManager.Instance.Terrain.Trees.transform)
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
        binaryWriter.Write(GameManager.Instance.Terrain.PickUpItems.transform.childCount);
        foreach (Transform pickUpItemTransform in GameManager.Instance.Terrain.PickUpItems.transform)
        {
            PickUpItem pickUpItem = pickUpItemTransform.GetComponent<PickUpItem>();
            binaryWriter.Write((byte)pickUpItem.Id);
            binaryWriter.Write((ushort)pickUpItemTransform.position.x);
            binaryWriter.Write((ushort)pickUpItemTransform.position.y);
        }
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
        Vector3 position = GameManager.Instance.GetPlayerTransform().position;
        binaryWriter.Write(position.x);
        binaryWriter.Write(position.y);
    }

    private void SavePlayerInventory(BinaryWriter binaryWriter)
    {
        InventoryModelOld inventory = GameManager.Instance.GetPlayerInventory();
        SavePlayerInventoryPart(binaryWriter, inventory, inventory.HotbarSize, ItemLocations.Hotbar);
        SavePlayerInventoryPart(binaryWriter, inventory, inventory.StorageSize, ItemLocations.Storage);
        SavePlayerInventoryPart(binaryWriter, inventory, inventory.AccessoriesSize, ItemLocations.Accessories);
        SavePlayerInventoryPart(binaryWriter, inventory, inventory.ArmorSize, ItemLocations.Armor);
    }

    private void SavePlayerInventoryPart(BinaryWriter binaryWriter, InventoryModelOld inventory, int size, ItemLocations location)
    {
        for (int i = 0; i < size; i++)
        {
            //InventoryItem inventoryItem = inventory.GetItem(i, location);
            //bool isEmpty = inventoryItem.IsEmpty;
            //binaryWriter.Write(isEmpty);
            //if (!isEmpty)
            //{
            //    binaryWriter.Write((ushort)inventoryItem.Data.Id);
            //    binaryWriter.Write(inventoryItem.Quantity);
            //}
        }
    }

    private void SavePlayerResearches(BinaryWriter binaryWriter)
    {
        ResearchesSO researches = GameManager.Instance.GetPlayerResearches();
        for (int i = 0; i < researches.GetResearchesCount(); i++)
        {
            binaryWriter.Write((byte)researches.GetState(i));
        }
    }

    private void LoadMetaData(BinaryReader binaryReader)
    {
        Debug.Log("Load meta data");
        GameManager.Instance.Seed = binaryReader.ReadInt32();
        GameManager.Instance.CurrentTerrainWidth = binaryReader.ReadInt32();
        GameManager.Instance.CurrentTerrainHeight = binaryReader.ReadInt32();
        _worldDataSize = binaryReader.ReadInt32();

        for (int x = 0; x < GameManager.Instance.CurrentTerrainWidth; x++)
        {
            _worldColumnIndexes.Add(binaryReader.ReadInt32());
        }
        GameManager.Instance.LoadingValue += _loadingStep;
    }

    private void LoadChunks(BinaryReader binaryReader)
    {
        Debug.Log("Load chunks");
        TerrainConfigurationSO terrainConfiguration = GameManager.Instance.TerrainConfiguration;
        foreach (var chunk in ChunksManager.Instance.Chunks)
        {
            ChunksManager.Instance.SetChunkBiome(chunk, terrainConfiguration.GetBiome((BiomesID)binaryReader.ReadByte()));
        }
        GameManager.Instance.LoadingValue += _loadingStep;
    }

    private void LoadWorldData(BinaryReader binaryReader)
    {
        Debug.Log("Load world data");
        Terrain terrain = GameManager.Instance.Terrain;
        BlocksAtlasSO blockAtlas = GameManager.Instance.BlocksAtlas;

        byte[] worldData = binaryReader.ReadBytes(_worldDataSize);
        try
        {
            Parallel.For(0, GameManager.Instance.CurrentTerrainWidth, index =>
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
                bool isLiquid;
                BlockSO block;
                BlockSO wall;

                for (int y = 0; y < GameManager.Instance.CurrentTerrainHeight;)
                {
                    isLiquid = false;
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
                        isLiquid = true;
                        liquidId = worldData[start + byteIndex++];
                        flowValue = worldData[start + byteIndex++] / 2.5f;
                    }
                    else
                    {
                        liquidId = 255;
                        flowValue = 0f;
                    }

                    tileId = worldData[start + byteIndex++];
                    blockType = (BlockTypes)worldData[start + byteIndex++];

                    if ((flags & 0b0000_1000) != 0)
                    {
                        count = worldData[start + byteIndex++];
                        count |= (worldData[start + byteIndex++] << 8);
                        count++;
                    }

                    block = blockAtlas.GetBlockByTypeAndId(blockType, blockId);
                    wall = blockAtlas.GetBlockByTypeAndId(BlockTypes.Wall, wallId);

                    for (int i = 0; i < count; i++)
                    {
                        terrain.CreateBlock(x, y + i, block);
                        terrain.CreateWall(x, y + i, wall);
                        terrain.SetTileId(x, y + i, tileId);
                        terrain.CreateLiquidBlock(x, y + i, liquidId, flowValue);
                    }

                    y += count;
                }
            });
            GameManager.Instance.LoadingValue += _loadingStep;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private void LoadTrees(BinaryReader binaryReader)
    {
        Debug.Log("Load trees");
        foreach (Transform child in GameManager.Instance.Terrain.Trees.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        int treesCount = binaryReader.ReadInt32();
        Vector3 position = new();

        for (int i = 0; i < treesCount; i++)
        {
            TreesID treeId = (TreesID)binaryReader.ReadByte();
            position.x = binaryReader.ReadUInt16();
            position.y = binaryReader.ReadUInt16();

            Tree tree = GameManager.Instance.TreesAtlas.GetTreeById(treeId);

            GameObject treeGameObject = GameObject.Instantiate(tree.gameObject, position, Quaternion.identity, GameManager.Instance.Terrain.Trees.transform);
            treeGameObject.name = tree.gameObject.name;
        }
        GameManager.Instance.LoadingValue += _loadingStep;
    }

    private void LoadPickUpItems(BinaryReader binaryReader)
    {
        Debug.Log("Load pickup items");
        foreach (Transform child in GameManager.Instance.Terrain.PickUpItems.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        int pickUpItemsCount = binaryReader.ReadInt32();
        Vector3Int position = new();

        for (int i = 0; i < pickUpItemsCount; i++)
        {
            PickUpItemsID pickUpItemId = (PickUpItemsID)binaryReader.ReadByte();
            position.x = binaryReader.ReadUInt16();
            position.y = binaryReader.ReadUInt16();

            PickUpItem pickUpItem = GameManager.Instance.PickUpItemsAtlas.GetPickUpItemById(pickUpItemId);

            GameObject pickUpItemGameObject = GameObject.Instantiate(pickUpItem.gameObject, position, Quaternion.identity, GameManager.Instance.Terrain.PickUpItems.transform);
            pickUpItemGameObject.name = pickUpItem.gameObject.name;
            WorldDataManager.Instance.MakeOccupied(position.x, position.y);
        }
        GameManager.Instance.LoadingValue += _loadingStep;
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
        GameManager.Instance.SetPlayerPosition(x, y);
    }

    private void LoadPlayerInventory(BinaryReader binaryReader)
    {
        //InventoryModel inventory = GameManager.Instance.GetPlayerInventory();
        //inventory.Initialize();
        //LoadPlayerInventoryPart(binaryReader, inventory, inventory.HotbarSize, ItemLocations.Hotbar);
        //LoadPlayerInventoryPart(binaryReader, inventory, inventory.StorageSize, ItemLocations.Storage);
        //LoadPlayerInventoryPart(binaryReader, inventory, inventory.AccessoriesSize, ItemLocations.Accessories);
        //LoadPlayerInventoryPart(binaryReader, inventory, inventory.ArmorSize, ItemLocations.Armor);
    }

    private void LoadPlayerInventoryPart(BinaryReader binaryReader, InventoryModelOld inventory, int size, ItemLocations location)
    {
        for (int i = 0; i < size; i++)
        {
            bool isEmpty = binaryReader.ReadBoolean();
            if (!isEmpty)
            {
                ItemsID id = (ItemsID)binaryReader.ReadUInt16();
                int quantity = binaryReader.ReadInt32();
                ItemSO itemData = GameManager.Instance.ItemsAtlas.GetItemByID(id);
                Debug.Log(itemData);
                inventory.AddItemAtWithoutNotification(itemData, quantity, i, location);
            }
        }
    }

    private void LoadPlayerResearches(BinaryReader binaryReader)
    {
        ResearchesSO researches = GameManager.Instance.GetPlayerResearches();
        researches.Initialize();
        for (int i = 0; i < researches.GetResearchesCount(); i++)
        {
            ResearchState state = (ResearchState)binaryReader.ReadByte();
            researches.SetResearchState(i, state);
        }
    }

    public void Save()
    {
        _worldColumnIndexes = new List<int>();

        string worldDirectory = StaticInfo.WorldsDirectory + $"/{GameManager.Instance.WorldName}";
        string worldSaveFile = worldDirectory + $"/{GameManager.Instance.WorldName}.sw.world";
        string worldMetaFile = worldDirectory + $"/{GameManager.Instance.WorldName}.swm.world";
        string playerFile = StaticInfo.PlayersDirectory + $"/{GameManager.Instance.PlayerName}.sw.player";

        Directory.CreateDirectory(worldDirectory);

        using (BinaryWriter binaryWriter = new(File.Open(worldSaveFile, FileMode.Create)))
        {
            Debug.Log("Save data to: " + worldSaveFile);
            SaveChunks(binaryWriter);
            SaveWorldData(binaryWriter);
            SaveTrees(binaryWriter);
            SavePickUpItems(binaryWriter);
        }

        using (BinaryWriter binaryWriter = new(File.Open(playerFile, FileMode.Create)))
        {
            Debug.Log("Save data to: " + playerFile);
            SavePlayer(binaryWriter);
        }

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

        string worldDirectory = StaticInfo.WorldsDirectory + $"/{GameManager.Instance.WorldName}";
        string worldSaveFile = worldDirectory + $"/{GameManager.Instance.WorldName}.sw.world";
        string worldMetaFile = worldDirectory + $"/{GameManager.Instance.WorldName}.swm.world";
        string playerFile = StaticInfo.PlayersDirectory + $"/{GameManager.Instance.PlayerName}.sw.player";

        _loadingStep = 100f / 5f;

        using (BinaryReader binaryReader = new(File.Open(worldMetaFile, FileMode.Open)))
        {
            LoadMetaData(binaryReader);
        }

        using (BinaryReader binaryReader = new(File.Open(worldSaveFile, FileMode.Open)))
        {
            LoadChunks(binaryReader);
            LoadWorldData(binaryReader);
            ActionInMainThreadUtil.Instance.Invoke(() =>
            {
                LoadTrees(binaryReader);
                LoadPickUpItems(binaryReader);
            });
        }

        using (BinaryReader binaryReader = new(File.Open(playerFile, FileMode.Open)))
        {
            ActionInMainThreadUtil.Instance.Invoke(() =>
            {
                LoadPlayer(binaryReader);
            });
        }

        new SetPhysicsShapesPhase().StartPhase();
        Debug.Log("Loading Complete");
    }
    #endregion
}
