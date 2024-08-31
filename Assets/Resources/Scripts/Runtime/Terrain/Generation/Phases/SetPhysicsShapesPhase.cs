using System.Threading.Tasks;

namespace SavageWorld.Runtime.Terrain.Generation.Phases
{
    public class SetPhysicsShapesPhase : WorldProcessingPhaseBase
    {
        #region Fields

        #endregion

        #region Properties
        public override string Name => "Set physics shapes";
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override void StartPhase()
        {
            Parallel.For(5, _terrainWidth - 5, x =>
            {
                for (int y = 5; y < _terrainHeight - 5; y++)
                {
                    _tilesManager.UpdateCornerColliderWithoutNotification(x, y, true);
                }
            });

            Parallel.For(5, _terrainWidth - 5, x =>
            {
                for (int y = 5; y < _terrainHeight - 5; y++)
                {
                    _tilesManager.UpdateBlockColliderWithoutNotification(x, y);
                }
            });
        }
        #endregion

        #region Private Methods

        #endregion
    }
}