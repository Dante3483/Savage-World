namespace SavageWorld.Runtime.Enums.Network
{
    public enum NetworkMessageTypes : byte
    {
        SendClientId,
        SendChunkData,
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