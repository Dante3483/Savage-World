using SavageWorld.Runtime.Enums.Network;
using SavageWorld.Runtime.Entities.Player.Interactions;
using SavageWorld.Runtime.Utilities;
using System.IO;

namespace SavageWorld.Runtime.Network.Messages
{
    public class AddDamageToTileMessage : NetworkMessageBase
    {
        #region Fields

        #endregion

        #region Properties
        protected override NetworkMessageTypes MessageType
        {
            get
            {
                return NetworkMessageTypes.AddDamageToTile;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public AddDamageToTileMessage(BinaryWriter writer, BinaryReader reader) : base(writer, reader)
        {
        }
        #endregion

        #region Private Methods
        protected override void ReadData()
        {
            int x = _reader.ReadInt32();
            int y = _reader.ReadInt32();
            float damage = _reader.ReadSingle();
            MainThreadUtility.Instance.InvokeInNextUpdate(() =>
            {
                MiningDamageController.Instance.AddDamageToBlock(new(x, y), damage);
                if (_networkManager.IsServer)
                {
                    MessageData messageData = new()
                    {
                        IntNumber1 = x,
                        IntNumber2 = y,
                        FloatNumber1 = damage
                    };
                    _networkManager.BroadcastMessage(NetworkMessageTypes.AddDamageToTile, messageData);
                }
            });
        }

        protected override void WriteData(MessageData messageData)
        {
            _writer.Write(messageData.IntNumber1);
            _writer.Write(messageData.IntNumber2);
            _writer.Write(messageData.FloatNumber1);
        }
        #endregion
    }
}
