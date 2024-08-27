namespace SavageWorld.Runtime.Enums.Network
{
    public enum NetworkMessageTypes : byte
    {
        SendClientId,
        SendChunkData,
        SendTime,
        Disconnect,

        CreatePlayer,
        CreateEnvironment,
        CreateDrop,
        DestroyObject,

        SendTransform,
        SendWorldCellData,
        SendEntityAnimation,

        AddDamageToTile,
        TakeDrop,
    }
}