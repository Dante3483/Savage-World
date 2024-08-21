using System.Threading.Tasks;

namespace SavageWorld.Runtime.GameSession.States
{
    public class LoadingWorldState : GameStateBase
    {
        #region Private fields

        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        public override void Enter()
        {
            UIManager.Instance.MainMenuProgressBarUI.IsActive = true;
            _gameManager.ResetLoadingValue();
            _gameManager.TerrainGameObject.SetActive(true);
            _gameManager.Terrain.StartCoroutinesAndThreads();
            Task.Run(LoadWorld);
        }

        public override void Exit()
        {
            UIManager.Instance.MainMenuProgressBarUI.IsActive = false;
        }

        private void LoadWorld()
        {
            _gameManager.Terrain.LoadWorld();
            ActionInMainThreadUtil.Instance.InvokeAndWait(() => _gameManager.ChangeState(_gameManager.CreatingPlayerState));
        }
        #endregion
    }
}