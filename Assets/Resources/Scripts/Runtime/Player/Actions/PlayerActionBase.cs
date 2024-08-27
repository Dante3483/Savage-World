using SavageWorld.Runtime.GameSession;
using SavageWorld.Runtime.Managers;

namespace SavageWorld.Runtime.Player.Actions
{
    public abstract class PlayerActionBase : IPlayerAction
    {
        #region Fields
        protected GameManager _gameManager;
        protected WorldDataManager _worldDataManager;
        protected PlayerGameObject _player;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public PlayerActionBase()
        {
            _gameManager = GameManager.Instance;
            _worldDataManager = WorldDataManager.Instance;
            _player = _gameManager.Player;
        }

        public abstract void Execute();
        #endregion

        #region Private Methods

        #endregion
    }
}