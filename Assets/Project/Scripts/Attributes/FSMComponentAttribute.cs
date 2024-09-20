using System;

namespace SavageWorld.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FSMComponentAttribute : Attribute
    {
        #region Fields
        private string _name;
        private string _path;
        #endregion

        #region Properties
        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string Path
        {
            get
            {
                return _path;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public FSMComponentAttribute(string name, string path)
        {
            _name = name;
            _path = path;
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
