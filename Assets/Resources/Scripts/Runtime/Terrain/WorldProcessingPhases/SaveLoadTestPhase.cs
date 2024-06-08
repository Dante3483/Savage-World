using System.Threading.Tasks;

public class SaveLoadTestPhase : WorldProcessingPhaseBase
{
    #region Fields

    #endregion

    #region Properties
    public override string Name => "Save/Load test";
    #endregion

    #region Events / Delegates

    #endregion

    #region Public Methods
    public override void StartPhase()
    {
        Parallel.For(0, _terrainWidth, (index) =>
        {
            int x = index;
            for (int y = 0; y < _terrainHeight; y++)
            {
                if (y % 2 == 0)
                {
                    SetBlockData(x, y, _dirt);
                    SetWallData(x, y, _dirtWall);
                    SetLiquidData(x, y, _water);
                }
                else
                {
                    SetBlockData(x, y, _stone);
                    SetWallData(x, y, _dirtWall);
                    SetLiquidData(x, y, _water);
                }
            }
        });
    }
    #endregion

    #region Private Methods

    #endregion
}
