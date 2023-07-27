using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newLevel", menuName = "Terrain/Level")]
public class TerrainLevelSO : ScriptableObject
{
    #region Private fields
    [Header("Main")]
    [SerializeField] private string _name;
    [SerializeField] private byte _countOfVerticalChunks;
    [SerializeField] private ushort _startY;
    [SerializeField] private ushort _endY;
    [SerializeField] private BlockSO _defaultBackground;

    [Header("Stone")]
    [SerializeField] private float _stoneAmplitude;
    [SerializeField] private float _stoneScale;
    [SerializeField] private float _stoneIntensity;
    [SerializeField] private List<BiomesID> _biomes;
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

    public float StoneIntensity
    {
        get
        {
            return _stoneIntensity;
        }

        set
        {
            _stoneIntensity = value;
        }
    }

    public float StoneAmplitude
    {
        get
        {
            return _stoneAmplitude;
        }

        set
        {
            _stoneAmplitude = value;
        }
    }

    public float StoneScale
    {
        get
        {
            return _stoneScale;
        }

        set
        {
            _stoneScale = value;
        }
    }

    public BlockSO DefaultBackground
    {
        get
        {
            return _defaultBackground;
        }

        set
        {
            _defaultBackground = value;
        }
    }
    #endregion

    #region Methods

    #endregion
}
