using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    #region Private fields
    private List<int> _worldColumnIndexes;
    private int _worldColumnSize;
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
        binaryWriter.Write(GameManager.Instance.Seed);
        binaryWriter.Write(GameManager.Instance.CurrentTerrainWidth);
        binaryWriter.Write(GameManager.Instance.CurrentTerrainHeight);
        binaryWriter.Write(_worldColumnSize);

        foreach (var worldColumnIndex in _worldColumnIndexes)
        {
            binaryWriter.Write(worldColumnIndex);
        }
    }

    private void SaveChunks(BinaryWriter binaryWriter)
    {
        foreach (var chunk in ChunksManager.Instance.Chunks)
        {
            binaryWriter.Write((byte)chunk.Biome.Id);
        }
    }

    private void SaveWorldCellData(BinaryWriter binaryWriter)
    {
        int startColumn = 0;
        for (int x = 0; x < GameManager.Instance.CurrentTerrainWidth; x++)
        {
            _worldColumnIndexes.Add(startColumn);
            for (int y = 0; y < GameManager.Instance.CurrentTerrainHeight; y++)
            {
                ref WorldCellData data = ref WorldDataManager.Instance.WorldData[x, y];

                //Flag bits:
                //1 - BlockID
                //2 - BackgroundID
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

                #region Background ID
                if (data.BackgroundId > 255)
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
                    if (data.BackgroundId != nextData.BackgroundId)
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
                binaryWriter.Write((byte)(data.BackgroundId));
                if ((flags & 0b0000_0010) != 0)
                {
                    startColumn += 1;
                    binaryWriter.Write((byte)(data.BackgroundId >> 8));
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
        _worldColumnSize = startColumn;
    }

    private void SaveTrees(BinaryWriter binaryWriter)
    {
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
        binaryWriter.Write(GameManager.Instance.Terrain.PickUpItems.transform.childCount);
        foreach (Transform pickUpItemTransform in GameManager.Instance.Terrain.PickUpItems.transform)
        {
            PickUpItem pickUpItem = pickUpItemTransform.GetComponent<PickUpItem>();
            binaryWriter.Write((byte)pickUpItem.Id);
            binaryWriter.Write((ushort)pickUpItemTransform.position.x);
            binaryWriter.Write((ushort)pickUpItemTransform.position.y);
        }
    }

    private void LoadMetaData(BinaryReader binaryReader)
    {
        GameManager.Instance.Seed = binaryReader.ReadInt32();
        GameManager.Instance.CurrentTerrainWidth = binaryReader.ReadInt32();
        GameManager.Instance.CurrentTerrainHeight = binaryReader.ReadInt32();
        _worldColumnSize = binaryReader.ReadInt32();

        for (int x = 0; x < GameManager.Instance.CurrentTerrainWidth; x++)
        {
            _worldColumnIndexes.Add(binaryReader.ReadInt32());
        }
    }

    private void LoadChunks(BinaryReader binaryReader)
    {
        TerrainConfigurationSO terrainConfiguration = GameManager.Instance.TerrainConfiguration;
        foreach (var chunk in ChunksManager.Instance.Chunks)
        {
            ChunksManager.Instance.SetChunkBiome(chunk, terrainConfiguration.GetBiome((BiomesID)binaryReader.ReadByte()));
        }
    }

    private void LoadWorldCellData(BinaryReader binaryReader)
    {
        Terrain terrain = GameManager.Instance.Terrain;
        BlocksAtlas blockAtlas = GameManager.Instance.BlocksAtlas;

        byte[] worldData = binaryReader.ReadBytes(_worldColumnSize);
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
                ushort backgroundId;
                int count;
                float flowValue;
                BlockTypes blockType;
                bool isLiquid;
                BlockSO block;
                BlockSO background;

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
                        backgroundId = worldData[start + byteIndex++];
                        backgroundId = (ushort)(worldData[start + byteIndex++] << 8);
                    }
                    else
                    {
                        backgroundId = worldData[start + byteIndex++];
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
                    background = blockAtlas.GetBlockByTypeAndId(BlockTypes.Background, backgroundId);

                    for (int i = 0; i < count; i++)
                    {
                        terrain.CreateBlock(x, y + i, block);
                        terrain.CreateBackground(x, y + i, background);
                        terrain.SetTileId(x, y + i, tileId);
                        terrain.CreateLiquidBlock(x, y + i, liquidId, flowValue);
                    }

                    y += count;
                }
            });
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private void LoadTrees(BinaryReader binaryReader)
    {
        foreach (Transform child in GameManager.Instance.Terrain.Trees.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        int treesCount = binaryReader.ReadInt32();
        Vector3 position = new Vector3();

        for (int i = 0; i < treesCount; i++)
        {
            TreesID treeId = (TreesID)binaryReader.ReadByte();
            position.x = binaryReader.ReadUInt16();
            position.y = binaryReader.ReadUInt16();

            Tree tree = GameManager.Instance.TreesAtlas.GetTreeById(treeId);

            GameObject treeGameObject = GameObject.Instantiate(tree.gameObject, position, Quaternion.identity, GameManager.Instance.Terrain.Trees.transform);
            treeGameObject.name = tree.gameObject.name;
        }
    }

    private void LoadPickUpItems(BinaryReader binaryReader)
    {
        foreach (Transform child in GameManager.Instance.Terrain.PickUpItems.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        int pickUpItemsCount = binaryReader.ReadInt32();
        Vector3 position = new Vector3();

        for (int i = 0; i < pickUpItemsCount; i++)
        {
            PickUpItemsID pickUpItemId = (PickUpItemsID)binaryReader.ReadByte();
            position.x = binaryReader.ReadUInt16();
            position.y = binaryReader.ReadUInt16();

            PickUpItem pickUpItem = GameManager.Instance.PickUpItemsAtlas.GetPickUpItemById(pickUpItemId);

            GameObject pickUpItemGameObject = GameObject.Instantiate(pickUpItem.gameObject, position, Quaternion.identity, GameManager.Instance.Terrain.PickUpItems.transform);
            pickUpItemGameObject.name = pickUpItem.gameObject.name;
        }
    }

    public void Save()
    {
        _worldColumnIndexes = new List<int>();

        string worldDirectory = StaticInfo.WorldsDirectory + $"/{GameManager.Instance.WorldName}";
        string worldSaveFile = worldDirectory + $"/{GameManager.Instance.WorldName}.sw.world";
        string worldMetaFile = worldDirectory + $"/{GameManager.Instance.WorldName}.swm.world";

        Directory.CreateDirectory(worldDirectory);

        using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(worldSaveFile, FileMode.Create)))
        {
            Debug.Log("Save data to: " + worldSaveFile);
            SaveChunks(binaryWriter);
            Debug.Log("Chunks saved");
            SaveWorldCellData(binaryWriter);
            Debug.Log("World saved");
            SaveTrees(binaryWriter);
            Debug.Log("Trees saved");
            SavePickUpItems(binaryWriter);
            Debug.Log("PickUp items saved");
            //SavePlayer(binaryWriter);
            //Debug.Log("Player saved");
        }

        using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(worldMetaFile, FileMode.Create)))
        {
            Debug.Log("Save data to: " + worldMetaFile);
            SaveMetaData(binaryWriter);
            Debug.Log("Meta data saved");
        }
        Debug.Log("Save Complete");
    }

    public void Load()
    {
        _worldColumnIndexes = new List<int>();

        string worldDirectory = StaticInfo.WorldsDirectory + $"/{GameManager.Instance.WorldName}";
        string worldSaveFile = worldDirectory + $"/{GameManager.Instance.WorldName}.sw.world";
        string worldMetaFile = worldDirectory + $"/{GameManager.Instance.WorldName}.swm.world";

        float loadingStep = 100f / 5f;

        using (BinaryReader binaryReader = new BinaryReader(File.Open(worldMetaFile, FileMode.Open)))
        {
            LoadMetaData(binaryReader);
            GameManager.Instance.LoadingValue += loadingStep;
        }

        using (BinaryReader binaryReader = new BinaryReader(File.Open(worldSaveFile, FileMode.Open)))
        {
            LoadChunks(binaryReader);
            GameManager.Instance.LoadingValue += loadingStep;
            LoadWorldCellData(binaryReader);
            GameManager.Instance.LoadingValue += loadingStep;
            ActionInMainThreadUtil.Instance.Invoke(() =>
            {
                LoadTrees(binaryReader);
                GameManager.Instance.LoadingValue += loadingStep;
                LoadPickUpItems(binaryReader);
                GameManager.Instance.LoadingValue += loadingStep;
            });
            //SavePlayer(binaryWriter);
            //Debug.Log("Player loaded");
        }
        Debug.Log("Load Complete");
    }
    #endregion
}
