using UnityEngine;

namespace SavageWorld.Runtime.Console.Commands
{
    [CreateAssetMenu(fileName = "ClearCommand", menuName = "Utilities/Developer/Clear")]
    public class ClearLogsCommand : DeveloperCommand
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
        public override bool Process(string[] args)
        {
            GameConsole.ClearText();
            return true;
        }
        #endregion

        #region Private Methods

        #endregion
    }
}