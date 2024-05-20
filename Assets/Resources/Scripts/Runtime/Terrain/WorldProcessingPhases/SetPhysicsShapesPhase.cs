using System.Threading.Tasks;

public class SetPhysicsShapesPhase : IWorldProcessingPhase
{
    #region Private fields

    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods

    #endregion
    public string Name => "Set physics shapes";

    public void StartPhase()
    {
        int terrainWidth = GameManager.Instance.CurrentTerrainWidth;
        int terrainHeight = GameManager.Instance.CurrentTerrainHeight;

        Parallel.For(5, terrainWidth - 5, x =>
        {
            for (int y = 5; y < terrainHeight - 5; y++)
            {
                WorldDataManager.Instance.UpdateCornerColliderWithoutNotification(x, y, true);
            }
        });

        Parallel.For(5, terrainWidth - 5, x =>
        {
            for (int y = 5; y < terrainHeight - 5; y++)
            {
                WorldDataManager.Instance.UpdateBlockColliderWithoutNotification(x, y);
            }
        });
    }
}
