using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public class SaveLoadSystem : MonoBehaviour
{
    [SerializeField] private World _world;
    [SerializeField] private GameObject _player;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            SaveAllData();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadAllData();
        }
    }

    private void SaveAllData()
    {
        string path = Application.dataPath + Path.AltDirectorySeparatorChar + "SaveData.fun";
        string persistancePath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "SaveData.fun";

        Debug.Log("Saved data to: " + path);

        using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
            SaveConfigurationData(binaryWriter);
            SaveWorldChunks(binaryWriter);
            SaveWorldObjectsData(binaryWriter);
            SaveTrees(binaryWriter);
            SavePlayer(binaryWriter);
        }

        Debug.Log("Save Complete");
    }

    private void LoadAllData()
    {
        string path = Application.dataPath + Path.AltDirectorySeparatorChar + "SaveData.fun";
        string persistancePath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "SaveData.fun";

        Debug.Log("Load data from: " + path);

        _world.IsLoadind = true;

        using (BinaryReader binaryReader = new BinaryReader(File.Open(path, FileMode.Open)))
        {
            LoadConfigurationData(binaryReader);    
            LoadWorldChunks(binaryReader);
            LoadWorldObjectsData(binaryReader);
            LoadTrees(binaryReader);
            LoadPlayer(binaryReader);
        }

        Debug.Log("Load Complete");
    }

    public void SaveConfigurationData(BinaryWriter binaryWriter)
    {
        byte horizontalChunksCount = Convert.ToByte(_world.TerrainConfiguration.horizontalChunksCount);

        binaryWriter.Write(horizontalChunksCount);
    }

    public void LoadConfigurationData(BinaryReader binaryReader)
    {
        byte horizontalChunksCount = binaryReader.ReadByte();

        _world.TerrainConfiguration.horizontalChunksCount = (int)horizontalChunksCount;

        _world.TerrainConfiguration.WorldWitdh = (int)horizontalChunksCount * _world.TerrainConfiguration.chunkSize;
    }

    private void SaveWorldChunks(BinaryWriter binaryWriter)
    {
        for (int x = 0; x < _world.TerrainConfiguration.horizontalChunksCount; x++)
        {
            for (int y = 0; y < _world.TerrainConfiguration.VerticalChunksCount; y++)
            {
                byte biomeID = 0;
                if (_world.Chunks[x, y].BiomeID == BiomesID.NonBiom)
                {
                    biomeID = (byte)(biomeID | 128);
                }
                else
                {
                    biomeID = Convert.ToByte(_world.Chunks[x, y].BiomeID);
                }
                binaryWriter.Write(biomeID);
            }
        }
    }

    private void LoadWorldChunks(BinaryReader binaryReader)
    {
        for (int x = 0; x < _world.TerrainConfiguration.horizontalChunksCount; x++)
        {
            for (int y = 0; y < _world.TerrainConfiguration.VerticalChunksCount; y++)
            {
                byte biomeID = binaryReader.ReadByte();
                if (biomeID == 128)
                {
                    _world.Chunks[x, y].BiomeID = BiomesID.NonBiom;
                }
                else
                {
                    _world.Chunks[x, y].BiomeID = (BiomesID)biomeID;
                }
            }
        }
    }

    private void SaveWorldObjectsData(BinaryWriter binaryWriter)
    {
        for (int x = 0; x < _world.TerrainConfiguration.WorldWitdh; x++)
        {
            for (int y = 0; y < _world.TerrainConfiguration.WorldHeight; y++)
            {
                List<byte> writeResult = new List<byte>();

                ObjectData data = _world.ObjectsData[x, y];

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
                while (_world.IsInMapRange(x, y + iterator) &&
                    _world.ObjectsData[x, y + iterator].Id == _world.ObjectsData[x, y].Id &&
                    _world.ObjectsData[x, y + iterator].IdBackground == _world.ObjectsData[x, y].IdBackground &&
                    _world.ObjectsData[x, y + iterator].Type == _world.ObjectsData[x, y].Type &&
                    _world.ObjectsData[x, y + iterator].TypeBackground == _world.ObjectsData[x, y].TypeBackground &&
                    _world.ObjectsData[x, y + iterator].IsTreeTrunk == _world.ObjectsData[x, y].IsTreeTrunk &&
                    _world.ObjectsData[x, y + iterator].IsTreeFoliage == _world.ObjectsData[x, y].IsTreeFoliage &&
                    _world.ObjectsData[x, y + iterator].CurrentFlowValue == _world.ObjectsData[x, y].CurrentFlowValue)
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
        Debug.Log("Load to saveData complete");
    }

    private void LoadWorldObjectsData(BinaryReader binaryReader)
    {
        for (int x = 0; x < _world.TerrainConfiguration.WorldWitdh; x++)
        {
            for (int y = 0; y < _world.TerrainConfiguration.WorldHeight; y++)
            {
                int id = 0;
                int backgroundId = 0;
                ObjectType type = ObjectType.Empty;
                ObjectType backgroundType = ObjectType.Empty;
                bool isTreeTrunk = false;
                bool isTreeFoliage = false;
                int countSameVertical = 0;
                float flowValue = 0f;

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
                BlockSO block = _world.ObjectAtlas.GetBlockByID(type, id);
                _world.CreateBlock(block, x, y);
                _world.ObjectsData[x, y].IsTreeTrunk = isTreeTrunk;
                _world.ObjectsData[x, y].IsTreeFoliage = isTreeFoliage;
                _world.ObjectsData[x, y].CurrentFlowValue = flowValue;
                if (countSameVertical != 0)
                {
                    int inc = 1;
                    int createdCount = 0;
                    while (countSameVertical > 0)
                    {
                        _world.CreateBlock(block, x, y + inc);
                        _world.ObjectsData[x, y + inc].IsTreeTrunk = isTreeTrunk;
                        _world.ObjectsData[x, y + inc].IsTreeFoliage = isTreeFoliage;
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
        GameObject trees = _world.gameObject.transform.Find("Trees").gameObject;
        int treeCount = trees.transform.childCount;

        byte flags = 0;
        byte[] count = new byte[2];

        count[0] = (byte)(treeCount & 127);
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
        GameObject trees = _world.gameObject.transform.Find("Trees").gameObject;
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
            GameObject treeGameObject = Instantiate(_world.ObjectAtlas.GetTreeByID(id));
            Tree tree = treeGameObject.GetComponent<Tree>();
            treeGameObject.transform.position = new Vector3(x, y);
            treeGameObject.transform.parent = trees.transform;
        }
    }

    private void SavePlayer(BinaryWriter binaryWriter)
    {
        InventoryController controller = _player.GetComponent<InventoryController>();
        InventorySO inventoryData = controller.GetInventory();

        float x = _player.transform.position.x;
        float y = _player.transform.position.y;
        Debug.Log(new Vector3(x, y));
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
        InventoryController controller = _player.GetComponent<InventoryController>();
        InventorySO inventoryData = controller.GetInventory();

        float x = binaryReader.ReadSingle();
        float y = binaryReader.ReadSingle();
        Debug.Log(new Vector3(x, y));
        byte inventorySize = binaryReader.ReadByte();

        _player.transform.position = new Vector3(x, y);
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
                ItemSO itemSO = controller.ItemsAtlas.GetItemByID(itemID);
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
                ArmorItemSO armorSO = controller.ItemsAtlas.GetItemByID(itemID) as ArmorItemSO;
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
