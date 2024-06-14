using System.Threading.Tasks;

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
        _gameManager.Seed = _gameManager.IsStaticSeed ? _gameManager.Seed : UnityEngine.Random.Range(-1000000, 1000000);
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
        //ActionInMainThreadUtil.Instance.Invoke(() => ConnectionManager.Instance.StartHostIp(_gameManager.PlayerName));
        ActionInMainThreadUtil.Instance.Invoke(() => _gameManager.ChangeState(_gameManager.PlayingState));
    }
    #endregion
}
