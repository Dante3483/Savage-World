using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticInfo
{
    #region Private fields
    private static string _playersDirectory;
    private static string _worldsDirectory;
    #endregion

    #region Public fields
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

    #region Properties

    #endregion

    #region Methods
    public static void Initialize()
    {
        _playersDirectory = Application.dataPath + "/Saves" + "/Players";
        _worldsDirectory = Application.dataPath + "/Saves" + "/Worlds";
    }
    #endregion
}
