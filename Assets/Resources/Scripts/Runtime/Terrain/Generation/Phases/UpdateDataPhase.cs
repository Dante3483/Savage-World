using System.Threading.Tasks;

namespace SavageWorld.Runtime.Terrain.Generation.Phases
{
    public class UpdateDataPhase : WorldProcessingPhaseBase
    {
        #region Fields

        #endregion

        #region Properties
        public override string Name => "Update data";
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public override void StartPhase()
        {
            Parallel.For(5, _terrainWidth - 5, (index) =>
            {
                for (int y = 5; y < _terrainHeight - 5; y++)
                {
                    if (IsDust(index, y) && IsEmpty(index, y - 1))
                    {
                        lock (this)
                        {
                            //_terrain.NeedToUpdate.Add(new Vector2Ushort(index, y));
                        }
                    }
                }
            });

            //while (_terrain.NeedToUpdate.Count != 0)
            //{
            //    //_terrain.UpdateWorldData();
            //}
        }
        #endregion

        #region Private Methods

        #endregion
    }
}