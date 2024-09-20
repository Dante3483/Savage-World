namespace SavageWorld.Runtime.Terrain.Generation.Phases
{
    public class SetRandomTilesPhase : WorldProcessingPhaseBase
    {
        #region Fields

        #endregion

        #region Properties
        public override string Name => "Set random tiles";
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override void StartPhase()
        {

            for (int x = 0; x < _terrainWidth; x++)
            {
                for (int y = 0; y < _terrainHeight; y++)
                {
                    _tilesManager.SetRandomBlockSprite(x, y);
                    _tilesManager.SetRandomWallSprite(x, y);
                }
            }
        }
        #endregion

        #region Private Methods

        #endregion
    }
}