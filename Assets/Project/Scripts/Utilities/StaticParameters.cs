using UnityEngine;

namespace SavageWorld.Runtime.Utilities
{
    public class StaticParameters
    {
        #region Private fields
        private static string _playersDirectory;
        private static string _worldsDirectory;
        #endregion

        #region Public fields
        public readonly static string AssetsPath = "Assets/";
        public readonly static string EditorPath = AssetsPath + "Editor/";
        public readonly static string ProjectPath = AssetsPath + "Project/";
        public readonly static string StylesPath = EditorPath + "Styles/";

        public readonly static string StyleExtension = ".uss";

        public readonly static char[] DigitsCharArray = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' ' };
        public readonly static byte Bit0 = 0b0000_0001;
        public readonly static byte Bit1 = 0b0000_0010;
        public readonly static byte Bit2 = 0b0000_0100;
        public readonly static byte Bit3 = 0b0000_1000;
        public readonly static byte Bit4 = 0b0001_0000;
        public readonly static byte Bit5 = 0b0010_0000;
        public readonly static byte Bit6 = 0b0100_0000;
        public readonly static byte Bit7 = 0b1000_0000;
        public readonly static byte InvertedBit0 = (byte)~Bit0;
        public readonly static byte InvertedBit1 = (byte)~Bit1;
        public readonly static byte InvertedBit2 = (byte)~Bit2;
        public readonly static byte InvertedBit3 = (byte)~Bit3;
        public readonly static byte InvertedBit4 = (byte)~Bit4;
        public readonly static byte InvertedBit5 = (byte)~Bit5;
        public readonly static byte InvertedBit6 = (byte)~Bit6;
        public readonly static byte InvertedBit7 = (byte)~Bit7;
        #endregion

        #region Properties
        public static string PlayersDirectory
        {
            get
            {
                return _playersDirectory;
            }
        }

        public static string WorldsDirectory
        {
            get
            {
                return _worldsDirectory;
            }
        }
        #endregion

        #region Methods
        public static void Initialize()
        {
            _playersDirectory = Application.dataPath + "/Saves" + "/Players";
            _worldsDirectory = Application.dataPath + "/Saves" + "/Worlds";
        }
        #endregion
    }
}