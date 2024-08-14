namespace SavageWorld.Runtime.Enums.Network
{
    public enum NetworkMessageTypes : byte
    {
        SendClientId,
        CreatePlayer,
        Disconnect,

        SendTransform
    }
}