using Random = System.Random;

public class SetRandomTilesGenerationPhase : IGenerationPhase
{
    #region Private fields
    private WorldCellData[,] _worldData = GameManager.Instance.WorldData;
    private Random _randomVar = GameManager.Instance.RandomVar;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public string Name => "Set random tiles";
    #endregion

    #region Methods
    public void StartPhase()
    {

        for (int x = 0; x < GameManager.Instance.CurrentTerrainWidth; x++)
        {
            for (int y = 0; y < GameManager.Instance.CurrentTerrainHeight; y++)
            {
                _worldData[x, y].SetRandomBlockTile(_randomVar);
                _worldData[x, y].SetRandomBackgroundTile(_randomVar);
            }
        }
    }
    #endregion
}
