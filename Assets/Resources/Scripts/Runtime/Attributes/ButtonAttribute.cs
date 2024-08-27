using System;

namespace SavageWorld.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonAttribute : Attribute
    {
        #region Private fields
        private string _buttonName;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public string ButtonName
        {
            get
            {
                return _buttonName;
            }
        }
        #endregion

        #region Methods
        public ButtonAttribute(string buttonName = "Button")
        {
            _buttonName = buttonName;
        }
        #endregion
    }
}