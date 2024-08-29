using UnityEngine;

namespace SavageWorld.Runtime.Utilities.Extensions
{
    public static class ColorExtension
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        public static Color GetColor(byte r, byte g, byte b)
        {
            return new Color(r / 255f, g / 255f, b / 255f);
        }

        public static Color GetColor(string htmlString)
        {
            if (!htmlString.StartsWith("#"))
            {
                htmlString = "#" + htmlString;
            }
            if (ColorUtility.TryParseHtmlString(htmlString, out Color color))
            {
                return color;
            }
            return Color.white;
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
