public abstract class PlayerActionBase : IPlayerAction
{
    #region Fields
    protected Player _player;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Public Methods
    public PlayerActionBase()
    {
        _player = GameManager.Instance.Player;
    }

    public abstract void Execute();
    #endregion

    #region Private Methods

    #endregion
}
