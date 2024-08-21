using SavageWorld.Runtime.Enums.Network;
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
            WorldDataManager worldDataManager = WorldDataManager.Instance;
            GameManager gameManager = GameManager.Instance;
            BlocksAtlasSO blockAtlas = gameManager.BlocksAtlas;
            if (_networkManager.IsClient)
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
                BlockTypes blockType = (BlockTypes)_reader.ReadByte();
                BlockSO block = blockAtlas.GetBlockByTypeAndId(blockType, blockId);
                BlockSO wall = blockAtlas.GetBlockByTypeAndId(BlockTypes.Wall, wallId);
                BlockSO liquid = liquidId == byte.MaxValue ? null : blockAtlas.GetBlockById(liquidId);
                ActionInMainThreadUtil.Instance.InvokeInNextUpdate(() => worldDataManager.SetFullData(x, y, block, wall, liquid, flowValue, colliderIndex, tileId, blockFlags));
                ActionInMainThreadUtil.Instance.InvokeInNextUpdate(() => worldDataManager.SetUpBlockData(x, y));
            }
            if (_networkManager.IsServer)
            {
                int x = _reader.ReadInt32();
                int y = _reader.ReadInt32();
                BlockTypes blockType = (BlockTypes)_reader.ReadByte();
                ushort id = _reader.ReadUInt16();
                switch (blockType)
                {
                    case BlockTypes.Abstract:
                    case BlockTypes.Solid:
                    case BlockTypes.Dust:
                    case BlockTypes.Plant:
                    case BlockTypes.Furniture:
                        {
                            BlockSO block = blockAtlas.GetBlockByTypeAndId(blockType, id);
                            ActionInMainThreadUtil.Instance.InvokeInNextUpdate(() => worldDataManager.SetBlockData(x, y, block));
                        }
                        break;
                    case BlockTypes.Wall:
                        {
                            BlockSO wall = blockAtlas.GetBlockByTypeAndId(BlockTypes.Wall, id);
                            worldDataManager.SetWallData(x, y, wall);
                        }
                        break;
                    case BlockTypes.Liquid:
                        {
                            BlockSO liquid = id == byte.MaxValue ? null : blockAtlas.GetBlockById((byte)id);
                            worldDataManager.SetLiquidData(x, y, liquid);
                        }
                        break;
                    default:
                        break;
                }
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
                WorldDataManager worldDataManager = WorldDataManager.Instance;
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
