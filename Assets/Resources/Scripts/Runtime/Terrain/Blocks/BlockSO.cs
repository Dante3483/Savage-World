using Items;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BlockSO : ScriptableObject
{
    #region Private fields
    [SerializeField] private List<Sprite> _sprites;
    [SerializeField] private ItemSO _drop;
    [SerializeField] private Color _colorOnMap = Color.white;
    [SerializeField] private float _lightValue;
    [SerializeField] private float _maximumDamage;
    [SerializeField] private Color32 _lightColor = Color.black;
    [SerializeField] private bool _isSurfaceLight;
    [SerializeField] private bool _waterproof = true;
    protected BlockTypes _type;
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
    }

    public List<Sprite> Sprites
    {
        get
        {
            return _sprites;
        }

        set
        {
            _sprites = value;
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

    public bool IsSurfaceLight
    {
        get
        {
            return _isSurfaceLight;
        }

        set
        {
            _isSurfaceLight = value;
        }
    }

    public Color32 LightColor
    {
        get
        {
            return _lightColor;
        }

        set
        {
            _lightColor = value;
        }
    }

    public bool Waterproof
    {
        get
        {
            return _waterproof;
        }

        set
        {
            _waterproof = value;
        }
    }

    public ItemSO Drop
    {
        get
        {
            return _drop;
        }

        set
        {
            _drop = value;
        }
    }

    public float MaximumDamage
    {
        get
        {
            return _maximumDamage;
        }
    }
    #endregion

    #region Methods
    public BlockSO()
    {
        _sprites = new List<Sprite>();
    }

    public abstract ushort GetId();
    #endregion
}
