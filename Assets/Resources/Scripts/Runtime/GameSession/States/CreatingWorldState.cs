using System.Threading.Tasks;

namespace SavageWorld.Runtime.GameSession.States
{
    public class CreatingWorldState : GameStateBase
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
            Task.Run(CreateWorld);
        }

        public override void Exit()
        {
            UIManager.Instance.MainMenuProgressBarUI.IsActive = false;
        }

        private void CreateWorld()
        {
            _gameManager.Terrain.CreateNewWorld();
            ActionInMainThreadUtil.Instance.Invoke(() => _gameManager.ChangeState(_gameManager.CreatingPlayerState));
        }
        #endregion
    }
}