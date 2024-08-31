using SavageWorld.Runtime.Atlases;
using SavageWorld.Runtime.Console;
using SavageWorld.Runtime.Enums.Network;
using SavageWorld.Runtime.Enums.Types;
using SavageWorld.Runtime.GameSession;
using SavageWorld.Runtime.Managers;
using SavageWorld.Runtime.Terrain;
using SavageWorld.Runtime.Terrain.Tiles;
using SavageWorld.Runtime.Utilities;
using SavageWorld.Runtime.Utilities.Extensions;
using System.IO;

namespace SavageWorld.Runtime.Network.Messages
{
    public class SendChunkDataMessage : NetworkMessageBase
    {
        public SendChunkDataMessage(BinaryWriter writer, BinaryReader reader) : base(writer, reader)
        {
        }
        #region Fields

        #endregion

        #region Properties
        protected override NetworkMessageTypes MessageType
        {
            get
            {
                return NetworkMessageTypes.SendChunkData;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods
        protected override void ReadData()
        {
            if (_networkManager.IsClient)
            {
                ReadChunkData();
            }
            if (_networkManager.IsServer)
            {
                int chunkX = _reader.ReadInt32();
                int chunkY = _reader.ReadInt32();
                MessageData messageData = new()
                {
                    IntNumber1 = chunkX,
                    IntNumber2 = chunkY
                };
                _networkManager.SendMessageTo(NetworkMessageTypes.SendChunkData, messageData, _senderId);
            }
        }

        protected override void WriteData(MessageData messageData)
        {
            if (_networkManager.IsClient)
            {
                _writer.Write(messageData.IntNumber1);
                _writer.Write(messageData.IntNumber2);
            }
            if (_networkManager.IsServer)
            {
                WriteChunkData(messageData);
            }
        }

        private void WriteChunkData(MessageData messageData)
        {
            TilesManager worldDataManager = TilesManager.Instance;
            GameManager gameManager = GameManager.Instance;
            int chunkSize = gameManager.TerrainConfiguration.ChunkSize;
            int startX = messageData.IntNumber1 * chunkSize;
            int startY = messageData.IntNumber2 * chunkSize;
            _writer.Write(startX);
            _writer.Write(startY);
            for (int x = startX; x < startX + chunkSize; x++)
            {
                for (int y = startY; y < startY + chunkSize; y++)
                {
                    byte flags = 0;
                    ushort blockId = worldDataManager.GetBlockId(x, y);
                    ushort wallId = worldDataManager.GetWallId(x, y);
                    byte liquidId = worldDataManager.GetLiquidId(x, y);
                    float flowValue = worldDataManager.GetFlowValue(x, y);
                    byte tileId = worldDataManager.GetTileId(x, y);
                    byte colliderIndex = worldDataManager.GetColliderIndex(x, y);
                    byte blockFlags = worldDataManager.GetFlags(x, y);
                    TileTypes type = worldDataManager.GetBlockType(x, y);

                    int iterator = 1;
                    int countOfSameObject = 0;
                    while (gameManager.IsInMapRange(x, y + iterator))
                    {
                        ushort nextBlockId = worldDataManager.GetBlockId(x, y + iterator);
                        ushort nextWallId = worldDataManager.GetWallId(x, y + iterator);
                        byte nextLiquidId = worldDataManager.GetLiquidId(x, y + iterator);
                        float nextFlowValue = worldDataManager.GetFlowValue(x, y + iterator);
                        byte nextTileId = worldDataManager.GetTileId(x, y + iterator);
                        byte nextColliderIndex = worldDataManager.GetColliderIndex(x, y + iterator);
                        byte nextBlockFlags = worldDataManager.GetFlags(x, y + iterator);
                        TileTypes nextType = worldDataManager.GetBlockType(x, y + iterator);

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
                        if (colliderIndex != nextColliderIndex)
                        {
                            break;
                        }
                        if (blockFlags != nextBlockFlags)
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

                    if (blockId > byte.MaxValue)
                    {
                        flags = flags.SetBit(0, true);
                    }
                    if (wallId > byte.MaxValue)
                    {
                        flags = flags.SetBit(1, true);
                    }
                    if (flowValue > 0)
                    {
                        flags = flags.SetBit(2, true);
                    }
                    if (countOfSameObject > 0)
                    {
                        flags = flags.SetBit(3, true);
                    }
                    WrtieBlockData(flags, blockId, wallId, liquidId, flowValue, tileId, colliderIndex, blockFlags, type, countOfSameObject);
                    y += countOfSameObject;
                }
            }
        }

        private void WrtieBlockData(byte flags, ushort blockId, ushort wallId, byte liquidId, float flowValue, byte tileId, byte colliderIndex, byte blockFlags, TileTypes type, int countOfSameObject)
        {
            _writer.Write(flags);
            _writer.Write((byte)(blockId));
            if ((flags & StaticParameters.Bit0) == StaticParameters.Bit0)
            {
                _writer.Write((byte)(blockId >> 8));
            }
            _writer.Write((byte)(wallId));
            if ((flags & StaticParameters.Bit1) == StaticParameters.Bit1)
            {
                _writer.Write((byte)(wallId >> 8));
            }
            if ((flags & StaticParameters.Bit2) == StaticParameters.Bit2)
            {
                _writer.Write(liquidId);
                _writer.Write((byte)(flowValue * 2.54f));
            }
            _writer.Write(tileId);
            _writer.Write(colliderIndex);
            _writer.Write(blockFlags);
            _writer.Write((byte)type);
            if ((flags & StaticParameters.Bit3) == StaticParameters.Bit3)
            {
                _writer.Write((byte)countOfSameObject);
                _writer.Write((byte)(countOfSameObject >> 8));
            }
        }

        private void ReadChunkData()
        {
            TilesManager worldDataManager = TilesManager.Instance;
            ChunksManager chunksManager = ChunksManager.Instance;
            GameManager gameManager = GameManager.Instance;
            TilesAtlasSO blockAtlas = gameManager.TilesAtlas;
            int chunkSize = gameManager.TerrainConfiguration.ChunkSize;
            int startX = _reader.ReadInt32();
            int startY = _reader.ReadInt32();
            chunksManager.SetChunkLoaded(startX, startY);
            GameConsole.Log((startX, startY).ToString());
            for (int x = startX; x < startX + chunkSize; x++)
            {
                for (int y = startY; y < startY + chunkSize;)
                {
                    byte flags;
                    ushort blockId;
                    ushort wallId;
                    byte liquidId;
                    float flowValue;
                    byte tileId;
                    byte colliderIndex;
                    byte blockFlags;
                    TileTypes blockType;
                    int count;
                    TileBaseSO block;
                    TileBaseSO wall;
                    TileBaseSO liquid;

                    flags = _reader.ReadByte();

                    blockId = _reader.ReadByte();
                    if ((flags & StaticParameters.Bit0) == StaticParameters.Bit0)
                    {
                        blockId |= (ushort)(_reader.ReadByte() << 8);
                    }

                    wallId = _reader.ReadByte();
                    if ((flags & StaticParameters.Bit1) == StaticParameters.Bit1)
                    {
                        wallId |= (ushort)(_reader.ReadByte() << 8);
                    }

                    if ((flags & StaticParameters.Bit2) == StaticParameters.Bit2)
                    {
                        liquidId = _reader.ReadByte();
                        flowValue = _reader.ReadByte() / 2.54f;
                    }
                    else
                    {
                        liquidId = byte.MaxValue;
                        flowValue = 0f;
                    }

                    tileId = _reader.ReadByte();
                    colliderIndex = _reader.ReadByte();
                    blockFlags = _reader.ReadByte();
                    blockType = (TileTypes)_reader.ReadByte();

                    if ((flags & StaticParameters.Bit3) == StaticParameters.Bit3)
                    {
                        count = _reader.ReadByte();
                        count |= _reader.ReadByte() << 8;
                        count++;
                    }
                    else
                    {
                        count = 1;
                    }

                    block = blockAtlas.GetBlockByTypeAndId(blockType, blockId);
                    wall = blockAtlas.GetBlockByTypeAndId(TileTypes.Wall, wallId);
                    liquid = liquidId == byte.MaxValue ? null : blockAtlas.GetBlockById(liquidId);
                    for (int i = 0; i < count; i++, y++)
                    {
                        worldDataManager.SetTileData(x, y, block, wall, liquid, flowValue, colliderIndex, tileId, blockFlags);
                    }
                }
            }
        }
        #endregion
    }
}
