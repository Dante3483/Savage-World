namespace SavageWorld.Runtime.Enums.Network
{
    public enum ConnectionStatuses
    {
        Undefined,
        Success,                  //client successfully connected. This may also be a successful reconnect.
        ServerFull,               //can't join, server is already at capacity.
        LoggedInAgain,            //logged in on a separate client, causing this one to be kicked out.
        UserRequestedDisconnect,  //Intentional Disconnect triggered by the user.
        GenericDisconnect,        //server disconnected, but no specific reason given.
        Reconnecting,             //client lost connection and is attempting to reconnect.
        IncompatibleBuildType,    //client build type is incompatible with server.
        ServerEndedSession,       //server intentionally ended the session.
        StartServerFailed,        //server failed to bind
        StartClientFailed         //failed to connect to server and/or invalid network endpoint
    }
}