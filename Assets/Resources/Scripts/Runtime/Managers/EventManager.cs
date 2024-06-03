using System;

public static class EventManager<TEventArgs>
{
    #region Private fields

    #endregion

    #region Public fields
    public static Action PlayerSpawned;
    #endregion

    #region Properties

    #endregion

    #region Methods
    public static void OnPlayerSpawned() => PlayerSpawned?.Invoke();
    #endregion
}
