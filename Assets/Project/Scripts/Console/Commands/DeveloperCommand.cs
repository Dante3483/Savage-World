using UnityEngine;

namespace SavageWorld.Runtime.Console.Commands
{
    public abstract class DeveloperCommand : ScriptableObject, IDeveloperCommand
    {
        #region Fields
        [SerializeField]
        private string _commandWord = string.Empty;
        #endregion

        #region Properties
        public string CommandWord => _commandWord;
        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
        public abstract bool Process(string[] args);
        #endregion

        #region Private Methods

        #endregion
    }
}