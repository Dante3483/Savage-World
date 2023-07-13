using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newLevel", menuName = "Terrain/Level")]
public class TerrainLevelSO : ScriptableObject
{
    #region Private fields
    [SerializeField] private string _name;
    [SerializeField] private byte _countOfVerticalChunks;
    [SerializeField] private ushort _startY;
    [SerializeField] private ushort _endY;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public byte CountOfVerticalChunks
    {
        get
        {
            return _countOfVerticalChunks;
        }

        set
        {
            _countOfVerticalChunks = value;
        }
    }

    public string Name
    {
        get
        {
            return _name;
        }

        set
        {
            _name = value;
        }
    }

    public ushort StartY
    {
        get
        {
            return _startY;
        }

        set
        {
            _startY = value;
        }
    }

    public ushort EndY
    {
        get
        {
            return _endY;
        }

        set
        {
            _endY = value;
        }
    }
    #endregion

    #region Methods

    #endregion
}
