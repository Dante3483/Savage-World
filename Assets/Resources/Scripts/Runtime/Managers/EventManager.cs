using System;

public static class EventManager
{
    #region Fields

    #endregion

    #region Properties

    #endregion

    #region Events / Delegates
    public static Action<ulong> PlayerConnectedAsClient;
    public static Action BookOpened;
    public static Action BookClosed;
    #endregion

    #region Public Methods
    public static void OnPlayerConnected(ulong clientId) => PlayerConnectedAsClient?.Invoke(clientId);

    public static void OnBookOpened() => BookOpened?.Invoke();

    public static void OnBookClosed() => BookClosed?.Invoke();
    #endregion

    #region Private Methods

    #endregion
}
