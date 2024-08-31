using SavageWorld.Runtime.Atlases;
using SavageWorld.Runtime.Enums.Network;
using SavageWorld.Runtime.Enums.Types;
using SavageWorld.Runtime.GameSession;
using SavageWorld.Runtime.Terrain;
using SavageWorld.Runtime.Terrain.Tiles;
using SavageWorld.Runtime.Utilities;
using System.IO;

namespace SavageWorld.Runtime.Network.Messages
{
    public class SendWorldCellDataMessage : NetworkMessageBase
    {
        #region Fields

        #endregion

        #region Properties
        protected override NetworkMessageTypes MessageType
        {
            get
            {
                return NetworkMessageTypes.SendWorldCellData;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public SendWorldCellDataMessage(BinaryWriter writer, BinaryReader reader) : base(writer, reader)
        {
        }
        #endregion

        #region Private Methods
        protected override void ReadData()
        {
            TilesManager worldDataManager = TilesManager.Instance;
            GameManager gameManager = GameManager.Instance;
            TilesAtlasSO blockAtlas = gameManager.TilesAtlas;
            if (_networkManager.IsServer)
            {
                int x = _reader.ReadInt32();
                int y = _reader.ReadInt32();
                TileTypes blockType = (TileTypes)_reader.ReadByte();
                ushort id = _reader.ReadUInt16();
                switch (blockType)
                {
                    case TileTypes.Abstract:
                    case TileTypes.Solid:
                    case TileTypes.Dust:
                    case TileTypes.Plant:
                    case TileTypes.Furniture:
                        {
                            TileBaseSO block = blockAtlas.GetBlockByTypeAndId(blockType, id);
                            MainThreadUtility.Instance.InvokeInNextUpdate(() => worldDataManager.SetBlockData(x, y, block));
                        }
                        break;
                    case TileTypes.Wall:
                        {
                            TileBaseSO wall = blockAtlas.GetBlockByTypeAndId(TileTypes.Wall, id);
                            worldDataManager.SetWallData(x, y, wall);
                        }
                        break;
                    case TileTypes.Liquid:
                        {
                            TileBaseSO liquid = id == byte.MaxValue ? null : blockAtlas.GetBlockById((byte)id);
                            worldDataManager.SetLiquidData(x, y, liquid);
                        }
                        break;
                    default:
                        break;
                }
            }
            else if (_networkManager.IsClient)
            {
                int x = _reader.ReadInt32();
                int y = _reader.ReadInt32();
                ushort blockId = _reader.ReadUInt16();
                ushort wallId = _reader.ReadUInt16();
                byte liquidId = _reader.ReadByte();
                float flowValue = _reader.ReadSingle();
                byte colliderIndex = _reader.ReadByte();
                byte tileId = _reader.ReadByte();
                byte blockFlags = _reader.ReadByte();
                TileTypes blockType = (TileTypes)_reader.ReadByte();
                TileBaseSO block = blockAtlas.GetBlockByTypeAndId(blockType, blockId);
                TileBaseSO wall = blockAtlas.GetBlockByTypeAndId(TileTypes.Wall, wallId);
                TileBaseSO liquid = liquidId == byte.MaxValue ? null : blockAtlas.GetBlockById(liquidId);
                MainThreadUtility.Instance.InvokeInNextUpdate(() =>
                {
                    worldDataManager.SetTileData(x, y, block, wall, liquid, flowValue, colliderIndex, tileId, blockFlags);
                    worldDataManager.SetUpBlockData(x, y);
                });
            }
        }

        protected override void WriteData(MessageData messageData)
        {
            if (_networkManager.IsClient)
            {
                _writer.Write(messageData.IntNumber1);
                _writer.Write(messageData.IntNumber2);
                _writer.Write((byte)messageData.IntNumber3);
                _writer.Write((ushort)messageData.IntNumber4);
            }
            if (_networkManager.IsServer)
            {
                TilesManager worldDataManager = TilesManager.Instance;
                int x = messageData.IntNumber1;
                int y = messageData.IntNumber2;
                _writer.Write(x);
                _writer.Write(y);
                _writer.Write(worldDataManager.GetBlockId(x, y));
                _writer.Write(worldDataManager.GetWallId(x, y));
                _writer.Write(worldDataManager.GetLiquidId(x, y));
                _writer.Write(worldDataManager.GetFlowValue(x, y));
                _writer.Write(worldDataManager.GetColliderIndex(x, y));
                _writer.Write(worldDataManager.GetTileId(x, y));
                _writer.Write(worldDataManager.GetFlags(x, y));
                _writer.Write((byte)worldDataManager.GetBlockType(x, y));
            }
        }
        #endregion
    }
}
