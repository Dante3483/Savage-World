using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class BlockSO : ScriptableObject
{
    #region Private fields
    [SerializeField] private List<TileBase> _tiles;
    [SerializeField] private BlockTypes _type;
    [SerializeField] private Color _colorOnMap = Color.white;
    [SerializeField] private float _lightValue;
    [SerializeField] private bool _isSourceOfLight;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public BlockTypes Type
    {
        get
        {
            return _type;
        }

        set
        {
            _type = value;
        }
    }

    public List<TileBase> Tiles
    {
        get
        {
            return _tiles;
        }

        set
        {
            _tiles = value;
        }
    }

    public Color ColorOnMap
    {
        get
        {
            return _colorOnMap;
        }

        set
        {
            _colorOnMap = value;
        }
    }

    public float LightValue
    {
        get
        {
            return _lightValue;
        }

        set
        {
            _lightValue = value;
        }
    }

    public bool IsSourceOfLight
    {
        get
        {
            return _isSourceOfLight;
        }

        set
        {
            _isSourceOfLight = value;
        }
    }
    #endregion

    #region Methods
    public abstract ushort GetId();
    #endregion
}
