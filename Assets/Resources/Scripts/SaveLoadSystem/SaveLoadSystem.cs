using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SaveLoadSystem : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            SaveAllData();
        }
    }

    public void SaveAllData()
    {
        string path = Application.dataPath + "/SaveData.fun";

        Debug.Log("Saved data to: " + path);

        using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
            SaveConfigurationData(binaryWriter);
            Debug.Log("Configuration saved");
            SaveWorldChunks(binaryWriter);
            Debug.Log("Chunks saved");
            SaveWorldObjectsData(binaryWriter);
            Debug.Log("World saved");
            SaveTrees(binaryWriter);
            Debug.Log("Trees saved");
            SavePlayer(binaryWriter);
            Debug.Log("Player saved");
        }

        Debug.Log("Save Complete");
    }

    public void LoadAllData()
    {
        string path = Application.dataPath + "/SaveData.fun";

        Debug.Log("Load data from: " + path);

        using (BinaryReader binaryReader = new BinaryReader(File.Open(path, FileMode.Open)))
        {
            LoadConfigurationData(binaryReader);
            Debug.Log("Configuration loaded");
            LoadWorldChunks(binaryReader);
            Debug.Log("Chunks loaded");
            LoadWorldObjectsData(binaryReader);
            Debug.Log("World loaded");
            LoadTrees(binaryReader);
            Debug.Log("Trees loaded");
            LoadPlayer(binaryReader);
            Debug.Log("Player loaded");
        }

        Debug.Log("Load Complete");
    }

    public void SaveConfigurationData(BinaryWriter binaryWriter)
    {
        byte horizontalChunksCount = Convert.ToByte(GameManager.Instance.World.TerrainConfiguration.horizontalChunksCount);

        binaryWriter.Write(horizontalChunksCount);
        binaryWriter.Write(GameManager.Instance.World.Seed);
    }

    public void LoadConfigurationData(BinaryReader binaryReader)
    {
        byte horizontalChunksCount = binaryReader.ReadByte();
        int seed = binaryReader.ReadInt32();

        GameManager.Instance.World.TerrainConfiguration.horizontalChunksCount = (int)horizontalChunksCount;

        GameManager.Instance.World.TerrainConfiguration.WorldWitdh = (int)horizontalChunksCount * GameManager.Instance.World.TerrainConfiguration.chunkSize;

        GameManager.Instance.World.Seed = seed;

        GameManager.Instance.World.RandomVar = new System.Random(seed.GetHashCode());
    }

    private void SaveWorldChunks(BinaryWriter binaryWriter)
    {
        for (int x = 0; x < GameManager.Instance.World.TerrainConfiguration.horizontalChunksCount; x++)
        {
            for (int y = 0; y < GameManager.Instance.World.TerrainConfiguration.VerticalChunksCount; y++)
            {
                byte biomeID = 0;
                if (GameManager.Instance.Chunks[x, y].BiomeID == BiomesID.NonBiom)
                {
                    biomeID = (byte)(biomeID | 128);
                }
                else
                {
                    biomeID = Convert.ToByte(GameManager.Instance.Chunks[x, y].BiomeID);
                }
                binaryWriter.Write(biomeID);
            }
        }
    }

    private void LoadWorldChunks(BinaryReader binaryReader)
    {
        for (int x = 0; x < GameManager.Instance.World.TerrainConfiguration.horizontalChunksCount; x++)
        {
            for (int y = 0; y < GameManager.Instance.World.TerrainConfiguration.VerticalChunksCount; y++)
            {
                byte biomeID = binaryReader.ReadByte();
                if (biomeID == 128)
                {
                    GameManager.Instance.Chunks[x, y].BiomeID = BiomesID.NonBiom;
                }
                else
                {
                    GameManager.Instance.Chunks[x, y].BiomeID = (BiomesID)biomeID;
                }
            }
        }
    }

    private void SaveWorldObjectsData(BinaryWriter binaryWriter)
    {
        for (int x = 0; x < GameManager.Instance.World.TerrainConfiguration.WorldWitdh; x++)
        {
            for (int y = 0; y < GameManager.Instance.World.TerrainConfiguration.WorldHeight; y++)
            {
                List<byte> writeResult = new List<byte>();

                ObjectData data = GameManager.Instance.ObjectsData[x, y];

                //Bits:
                //0 - If <Block ID> set
                //1 - If <Background ID> set
                //2 - If <Type, Background type, Tree trunk and foliage flags> set
                //3 - If <Counts of same object <= 255> set
                //4 - If <Counts of same object > 255> set
                //5 - If <Flow value > 0> set
                byte flags = 0;

                byte blockID;
                byte backgroundID = 0;
                byte otherData;
                byte[] sameObject = new byte[2];
                byte flowValue = 0;

                #region Block ID
                blockID = Convert.ToByte(data.Id);
                flags = (byte)(flags | 1);
                #endregion

                #region Background ID
                if (data.TypeBackground != ObjectType.Empty)
                {
                    backgroundID = Convert.ToByte(data.IdBackground);
                    flags = (byte)(flags | 2);
                }
                #endregion

                #region Type, Background type, Tree trunk and foliage flags
                otherData = (byte)(Convert.ToByte(data.Type) | (Convert.ToByte(data.TypeBackground) << 3) | (Convert.ToByte(data.IsTreeTrunk) << 6) | (Convert.ToByte(data.IsTreeFoliage) << 7));
                flags = (byte)(flags | 4);
                #endregion

                #region RLE Algorithm
                int iterator = 1;
                int countOfSameObject = 0;
                while (GameManager.Instance.World.IsInMapRange(x, y + iterator) &&
                    GameManager.Instance.ObjectsData[x, y + iterator].Id == GameManager.Instance.ObjectsData[x, y].Id &&
                    GameManager.Instance.ObjectsData[x, y + iterator].IdBackground == GameManager.Instance.ObjectsData[x, y].IdBackground &&
                    GameManager.Instance.ObjectsData[x, y + iterator].Type == GameManager.Instance.ObjectsData[x, y].Type &&
                    GameManager.Instance.ObjectsData[x, y + iterator].TypeBackground == GameManager.Instance.ObjectsData[x, y].TypeBackground &&
                    GameManager.Instance.ObjectsData[x, y + iterator].IsTreeTrunk == GameManager.Instance.ObjectsData[x, y].IsTreeTrunk &&
                    GameManager.Instance.ObjectsData[x, y + iterator].IsTreeFoliage == GameManager.Instance.ObjectsData[x, y].IsTreeFoliage &&
                    GameManager.Instance.ObjectsData[x, y + iterator].CurrentFlowValue == GameManager.Instance.ObjectsData[x, y].CurrentFlowValue)
                {
                    countOfSameObject++;
                    iterator++;
                }

                if (countOfSameObject != 0)
                {
                    sameObject[0] = (byte)(countOfSameObject & 255);
                    flags = (byte)(flags | 8);
                    if (countOfSameObject > 255)
                    {
                        sameObject[1] = (byte)((countOfSameObject >> 8) & 255);
                        flags = (byte)(flags | 16);
                    }
                    y += countOfSameObject;
                }
                #endregion

                #region Flow value
                if (data.CurrentFlowValue != 0f)
                {
                    int intFlowValue = (int)(data.CurrentFlowValue * 100);
                    flowValue = (byte)(intFlowValue | intFlowValue);
                    flags = (byte)(flags | 32);
                }
                #endregion

                #region Write data
                writeResult.Add(flags);
                if ((flags & 1) == 1)
                {
                    writeResult.Add(blockID);
                }
                if ((flags & 2) == 2)
                {
                    writeResult.Add(backgroundID);
                }
                if ((flags & 4) == 4)
                {
                    writeResult.Add(otherData);
                }
                if ((flags & 8) == 8)
                {
                    writeResult.Add(sameObject[0]);
                }
                if ((flags & 16) == 16)
                {
                    writeResult.Add(sameObject[1]);
                }
                if ((flags & 32) == 32)
                {
                    writeResult.Add(flowValue);
                }
                binaryWriter.Write(writeResult.ToArray());
                #endregion
            }
        }
    }

    private void LoadWorldObjectsData(BinaryReader binaryReader)
    {
        int id = 0;
        int backgroundId = 0;
        ObjectType type = ObjectType.Empty;
        ObjectType backgroundType = ObjectType.Empty;
        bool isTreeTrunk = false;
        bool isTreeFoliage = false;
        int countSameVertical = 0;
        float flowValue = 0f;

        for (int x = 0; x < GameManager.Instance.World.TerrainConfiguration.WorldWitdh; x++)
        {
            for (int y = 0; y < GameManager.Instance.World.TerrainConfiguration.WorldHeight; y++)
            {
                id = 0;
                backgroundId = 0;
                type = ObjectType.Empty;
                backgroundType = ObjectType.Empty;
                isTreeTrunk = false;
                isTreeFoliage = false;
                countSameVertical = 0;
                flowValue = 0f;

                byte flags = binaryReader.ReadByte();
                if ((flags & 1) == 1)
                {
                    id = binaryReader.ReadByte();
                }
                if ((flags & 2) == 2)
                {
                    backgroundId = binaryReader.ReadByte();
                }
                if ((flags & 4) == 4)
                {
                    byte otherDataInfo = binaryReader.ReadByte();
                    type = (ObjectType)(otherDataInfo & 7);
                    backgroundType = (ObjectType)(otherDataInfo & 56);
                    isTreeTrunk = (otherDataInfo & 64) == 64;
                    isTreeFoliage = (otherDataInfo & 128) == 128;
                }
                if ((flags & 8) == 8 && (flags & 16) != 16)
                {
                    countSameVertical = binaryReader.ReadByte();
                }
                if ((flags & 16) == 16)
                {
                    int firstPart = binaryReader.ReadByte();
                    int secondPart = binaryReader.ReadByte();
                    countSameVertical = firstPart | (secondPart << 8);
                }
                if ((flags & 32) == 32)
                {
                    int intFlowValue = (int)binaryReader.ReadByte();
                    flowValue = intFlowValue / 100f;
                }
                BlockSO block = GameManager.Instance.WorldObjectAtlas.GetBlockByID(type, id);
                GameManager.Instance.World.CreateBlock(block, x, y);
                GameManager.Instance.ObjectsData[x, y].IsTreeTrunk = isTreeTrunk;
                GameManager.Instance.ObjectsData[x, y].IsTreeFoliage = isTreeFoliage;
                GameManager.Instance.ObjectsData[x, y].CurrentFlowValue = flowValue;
                if (countSameVertical != 0)
                {
                    int inc = 1;
                    int createdCount = 0;
                    while (countSameVertical > 0)
                    {
                        GameManager.Instance.World.CreateBlock(block, x, y + inc);
                        GameManager.Instance.ObjectsData[x, y + inc].IsTreeTrunk = isTreeTrunk;
                        GameManager.Instance.ObjectsData[x, y + inc].IsTreeFoliage = isTreeFoliage;
                        GameManager.Instance.ObjectsData[x, y + inc].CurrentFlowValue = flowValue;
                        inc++;
                        countSameVertical--;
                        createdCount++;
                    }
                    y += createdCount;
                }
            }
        }
    }

    private void SaveTrees(BinaryWriter binaryWriter)
    {
        GameObject trees = GameManager.Instance.TreesSection;
        int treeCount = trees.transform.childCount;

        byte flags = 0;
        byte[] count = new byte[2];

        count[0] = (byte)(treeCount & 255);
        flags = (byte)(flags | 1);

        if (treeCount > 255)
        {
            count[1] = (byte)((treeCount >> 8) & 255);
            flags = (byte)(flags | 2);
        }

        binaryWriter.Write(flags);
        if ((flags & 1) == 1)
        {
            binaryWriter.Write(count[0]);
        }
        if ((flags & 2) == 2)
        {
            binaryWriter.Write(count[1]);
        }

        foreach (Transform child in trees.transform)
        {
            byte id = Convert.ToByte(child.GetComponent<Tree>().Id);
            float x = child.transform.position.x;
            float y = child.transform.position.y;
            binaryWriter.Write(id);
            binaryWriter.Write(x);
            binaryWriter.Write(y);
        }
    }

    private void LoadTrees(BinaryReader binaryReader) 
    {
        GameObject trees = GameManager.Instance.TreesSection;
        trees.transform.parent = GameManager.Instance.World.gameObject.transform;

        foreach (Transform child in trees.transform)
        {
            Destroy(child.gameObject);
        }
        int treeCount = trees.transform.childCount;

        byte flags = binaryReader.ReadByte();

        if ((flags & 1) == 1 && (flags & 2) != 2)
        {
            treeCount = (int)binaryReader.ReadByte();
        }
        if ((flags & 2) == 2)
        {
            int firstPart = binaryReader.ReadByte();
            int secondPart = binaryReader.ReadByte();
            treeCount = firstPart | (secondPart << 8);
        }

        int createdCount = 0;
        while (createdCount != treeCount)
        {
            TreesID id = (TreesID)binaryReader.ReadByte();
            float x = binaryReader.ReadSingle();
            float y = binaryReader.ReadSingle();
            createdCount++;
            GameObject treeGameObject = Instantiate(GameManager.Instance.WorldObjectAtlas.GetTreeByID(id), new Vector3(x, y), Quaternion.identity, trees.transform);
            Tree tree = treeGameObject.GetComponent<Tree>();
        }
    }

    private void SavePlayer(BinaryWriter binaryWriter)
    {
        InventoryController controller = GameManager.Instance.Player.GetComponent<InventoryController>();
        InventorySO inventoryData = controller.GetInventory();

        float x = GameManager.Instance.Player.transform.position.x;
        float y = GameManager.Instance.Player.transform.position.y;
        byte inventorySize = Convert.ToByte(inventoryData.Size);

        binaryWriter.Write(x);
        binaryWriter.Write(y);
        binaryWriter.Write(inventorySize);
        foreach (var item in inventoryData.GetCurrentInventoryState())
        {
            byte itemFlags = 0;
            ushort itemID = 0;
            ushort itemQuantity = 0;

            if (!item.Value.IsEmpty)
            {
                itemFlags = (byte)(itemFlags | 1);
                itemID = (ushort)item.Value.Item.Id;
                itemQuantity = (ushort)item.Value.Quantity;

                binaryWriter.Write(itemFlags);
                binaryWriter.Write(itemID);
                binaryWriter.Write(itemQuantity);
            }
            else
            {
                binaryWriter.Write(itemFlags);
            }
        }
        foreach (var item in inventoryData.GetArmorInventoryState())
        {
            byte itemFlags = 0;
            ushort itemID = 0;
            ushort itemQuantity = 0;

            if (!item.IsEmpty)
            {
                itemFlags = (byte)(itemFlags | 1);
                itemID = (ushort)item.Item.Id;
                itemQuantity = (ushort)item.Quantity;

                binaryWriter.Write(itemFlags);
                binaryWriter.Write(itemID);
                binaryWriter.Write(itemQuantity);
            }
            else
            {
                binaryWriter.Write(itemFlags);
            }
        }
    }

    private void LoadPlayer(BinaryReader binaryReader)
    {
        InventoryController controller = GameManager.Instance.Player.GetComponent<InventoryController>();
        InventorySO inventoryData = controller.GetInventory();

        float x = binaryReader.ReadSingle();
        float y = binaryReader.ReadSingle();
        byte inventorySize = binaryReader.ReadByte();

        GameManager.Instance.Player.transform.position = new Vector3(x, y);
        GameManager.Instance.Player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        inventoryData.Size = inventorySize;
        inventoryData.Initialize();

        for (int i = 0; i < inventorySize; i++)
        {
            byte itemFlags = binaryReader.ReadByte();

            if (itemFlags == 0)
            {
                continue;
            }
            else
            {
                ItemsID itemID = (ItemsID)binaryReader.ReadUInt16();
                ItemSO itemSO = GameManager.Instance.ItemsAtlas.GetItemByID(itemID);
                int itemQuantity = (int)binaryReader.ReadUInt16();
                InventoryItem item = new InventoryItem()
                {
                    Item = itemSO,
                    Quantity = itemQuantity,
                };
                inventoryData.AddItemAt(item, i);
            }
        }

        for (int i = 0; i < 6; i++)
        {
            byte itemFlags = binaryReader.ReadByte();

            if (itemFlags == 0)
            {
                continue;
            }
            else
            {
                ItemsID itemID = (ItemsID)binaryReader.ReadUInt16();
                ArmorItemSO armorSO = GameManager.Instance.ItemsAtlas.GetItemByID(itemID) as ArmorItemSO;
                int itemQuantity = (int)binaryReader.ReadUInt16();
                InventoryItem item = new InventoryItem()
                {
                    Item = armorSO,
                    Quantity = itemQuantity,
                };
                inventoryData.SetArmorAt(item, i);
            }
        }

        inventoryData.InformAboutChange();
    }
}
