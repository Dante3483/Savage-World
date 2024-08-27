using SavageWorld.Runtime.Utilities.StateMachine;

namespace SavageWorld.Runtime.GameSession.States
{
    public abstract class GameStateBase : StateBase
    {
        #region Private fields
        protected GameManager _gameManager;
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        public GameStateBase()
        {
            _gameManager = GameManager.Instance;
        }
        #endregion
    }
}