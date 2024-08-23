namespace SavageWorld.Runtime.Enums.Network
{
    public enum NetworkMessageTypes : byte
    {
        SendClientId,
        SendChunkData,
        Disconnect,

        CreatePlayer,
        CreateEnvironment,

        SendTransform,
        SendWorldCellData,
        SendEntityAnimation
    }
}