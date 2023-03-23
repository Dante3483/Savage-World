using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectData
{   
    #region Private fields
    //Main
    private int _id;
    private int _idBackground;
    private ObjectType _type;
    private ObjectType _typeBackground;
    private Vector3Int _position;
    private Color _colorOnMap = Color.red;
    private Color _backgroundColorOnMap = Color.blue;
    private TileBase _currentTile;
    private TileBase _currentBackgroundTile;
    private TileBase _currentLiquidBackgroundTile;
    private float _durability = -1;

    //Liquid
    private float _currentFlowValue = 0;
    private int _countPsevdoFull = 0;
    private DateTime _creationTime;
    private bool _isPsevdoFull = false;
    private bool _isFull;
    private bool _isAboveNotEmpty = false;

    //Caves
    private bool _isChecked;

    //Plant
    private int _chanceToGrow;

    //Tree
    private bool _isTreeTrunk;
    private bool _isTreeFoliage;
    #endregion

    #region Properties
    public Vector3Int Position
    {
        get
        {
            return _position;
        }

        set
        {
            _position = value;
        }
    }

    public int Id
    {
        get
        {
            return _id;
        }

        set
        {
            _id = value;
        }
    }

    public ObjectType Type
    {
        get
        {
            return _type;
        }

        set
        {
            if (value == ObjectType.Empty)
            {
                CurrentFlowValue = 0f;
            }
            if (value == ObjectType.LiquidBlock)
            {
                CreationTime = DateTime.Now;
            }
            _type = value;
        }
    }

    public float CurrentFlowValue
    {
        get
        {
            return _currentFlowValue;
        }

        set
        {
            if (value >= 1f)
            {
                IsFull = true;
            }
            else
            {
                IsFull = false;
            }
            _currentFlowValue = value;
        }
    }

    public int CountPsevdoFull
    {
        get
        {
            return _countPsevdoFull;
        }

        set
        {
            _countPsevdoFull = value;
        }
    }

    public DateTime CreationTime
    {
        get
        {
            return _creationTime;
        }

        set
        {
            _creationTime = value;
        }
    }

    public bool IsPsevdoFull
    {
        get
        {
            return _isPsevdoFull;
        }

        set
        {
            _isPsevdoFull = value;
        }
    }

    public bool IsFull
    {
        get
        {
            return _isFull;
        }

        set
        {
            _isFull = value;
        }
    }

    public bool IsAboveNotEmpty
    {
        get
        {
            return _isAboveNotEmpty;
        }

        set
        {
            _isAboveNotEmpty = value;
        }
    }

    public TileBase CurrentTile
    {
        get
        {
            return _currentTile;
        }

        set
        {
            _currentTile = value;
        }
    }

    public TileBase CurrentBackgroundTile
    {
        get
        {
            return _currentBackgroundTile;
        }

        set
        {
            _currentBackgroundTile = value;
        }
    }

    public TileBase CurrentLiquidBackgroundTile
    {
        get
        {
            return _currentLiquidBackgroundTile;
        }

        set
        {
            _currentLiquidBackgroundTile = value;
        }
    }

    public bool IsChecked
    {
        get
        {
            return _isChecked;
        }

        set
        {
            _isChecked = value;
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

    public Color BackgroundColorOnMap
    {
        get
        {
            return _backgroundColorOnMap;
        }

        set
        {
            _backgroundColorOnMap = value;
        }
    }

    public int IdBackground
    {
        get
        {
            return _idBackground;
        }

        set
        {
            _idBackground = value;
        }
    }

    public ObjectType TypeBackground
    {
        get
        {
            return _typeBackground;
        }

        set
        {
            _typeBackground = value;
        }
    }

    public int ChanceToGrow
    {
        get
        {
            return _chanceToGrow;
        }

        set
        {
            _chanceToGrow = value;
        }
    }

    public bool IsTreeTrunk
    {
        get
        {
            return _isTreeTrunk;
        }

        set
        {
            _isTreeTrunk = value;
        }
    }

    public bool IsTreeFoliage
    {
        get
        {
            return _isTreeFoliage;
        }

        set
        {
            _isTreeFoliage = value;
        }
    }

    public float Durability
    {
        get
        {
            return _durability;
        }

        set
        {
            _durability = value;
        }
    }
    #endregion

    #region Methods
    public ObjectData()
    {
        Id = 0;
        Type = ObjectType.Empty;
    }
    public ObjectData(Vector3Int position)
    {
        Id = 0;
        Type = ObjectType.Empty;
        Position = position;
    }
    public ObjectData(int id, ObjectType type, Vector3Int position)
    {
        Id = id;
        Type = type;
        Position = position;
    }

    public bool IsSolidBlock()
    {
        return Type == ObjectType.SolidBlock || Type == ObjectType.DustBlock;
    }

    #endregion
}
