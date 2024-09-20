namespace SavageWorld.Runtime.Console.Commands
{
    public interface IDeveloperCommand
    {
        #region Properties
        public string CommandWord { get; }
        #endregion

        #region Public Methods
        public bool Process(string[] args);
        #endregion
    }
}