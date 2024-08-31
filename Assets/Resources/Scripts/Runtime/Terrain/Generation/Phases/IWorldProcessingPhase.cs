namespace SavageWorld.Runtime.Terrain.Generation.Phases
{
    public interface IWorldProcessingPhase
    {
        #region Private fields

        #endregion

        #region Public fields
        public string Name { get; }
        #endregion

        #region Properties

        #endregion

        #region Methods
        public void StartPhase();
        #endregion
    }
}