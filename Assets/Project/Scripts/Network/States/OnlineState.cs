namespace SavageWorld.Runtime.Network.States
{
    public abstract class OnlineState : ConnectionStateBase
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
        public OnlineState(ConnectionManager connectionManager) : base(connectionManager)
        {

        }

        public override void OnUserRequestedShutdown()
        {
            _connectionManager.ChangeState(_connectionManager.OfflineState);
        }
        #endregion

        #region Private Methods

        #endregion
    }
}