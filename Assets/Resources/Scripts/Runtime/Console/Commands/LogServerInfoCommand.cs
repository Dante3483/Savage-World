using SavageWorld.Runtime.Network;
using UnityEngine;

namespace SavageWorld.Runtime.Console.Commands
{
    [CreateAssetMenu(fileName = "LogServerInfoCommand", menuName = "Utilities/Developer/LogServerInfo")]
    public class LogServerInfoCommand : DeveloperCommand
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
            for (int i = 0; i < 1000; i++)
            {
                GameConsole.Log(NetworkManager.Instance.GetServerInfo());
            }
            return true;
        }
        #endregion

        #region Private Methods

        #endregion
    }
}