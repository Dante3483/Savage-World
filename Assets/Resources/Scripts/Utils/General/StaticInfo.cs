using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticInfo
{
    #region Private fields
    private static string _playersDirectory;
    private static string _worldsDirectory;
    private static char[] _digitsCharArray = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' '};
    #endregion

    #region Public fields

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

    public static char[] DigitsCharArray
    {
        get
        {
            return _digitsCharArray;
        }

        set
        {
            _digitsCharArray = value;
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
