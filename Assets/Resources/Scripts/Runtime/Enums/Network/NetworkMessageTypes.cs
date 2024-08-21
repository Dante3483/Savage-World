namespace SavageWorld.Runtime.Enums.Network
{
    public enum NetworkMessageTypes : byte
    {
        SendClientId,
        SendChunkData,
        CreatePlayer,
        Disconnect,

        SendTransform,
        SendWorldCellData,
        SendEntityAnimation
    }
}