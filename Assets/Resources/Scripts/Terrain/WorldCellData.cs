using UnityEngine;
using UnityEngine.Tilemaps;

public struct WorldCellData
{
    #region Private fields

    #region Main
    private ushort _id;
    private byte _tileId;
    private BlockTypes _blockType;
    private BlockSO _blockData;
    private Vector2Ushort _coords;
    #endregion

    #region Liquid
    private byte _liquidId;
    private bool _isFlowsDown;
    private float _flowValue;
    #endregion
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public ushort Id
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

    public BlockTypes BlockType
    {
        get
        {
            return _blockType;
        }

        set
        {
            _blockType = value;
        }
    }

    public BlockSO BlockData
    {
        get
        {
            return _blockData;
        }

        set
        {
            _blockData = value;
        }
    }

    public byte TileId
    {
        get
        {
            return _tileId;
        }

        set
        {
            _tileId = value;
        }
    }

    public Vector2Ushort Coords
    {
        get
        {
            return _coords;
        }

        set
        {
            _coords = value;
        }
    }

    public float FlowValue
    {
        get
        {
            return _flowValue;
        }

        set
        {
            _flowValue = value;
        }
    }

    public byte LiquidId
    {
        get
        {
            return _liquidId;
        }

        set
        {
            _liquidId = value;
        }
    }

    public bool IsFlowsDown
    {
        get
        {
            return _isFlowsDown;
        }

        set
        {
            _isFlowsDown = value;
        }
    }
    #endregion

    #region Methods
    public WorldCellData(ushort xPosition, ushort yPosition)
    {
        //Set Ait block by default
        _id = 0;
        _tileId = 255;
        _blockType = BlockTypes.Abstract;
        _blockData = GameManager.Instance.ObjectsAtlass.Air;
        _coords = new Vector2Ushort { x = xPosition, y = yPosition };

        _liquidId = 255;
        _isFlowsDown = false;
        _flowValue = 0;
    }

    public override string ToString()
    {
        return $"X: {_coords.x}\n" +
            $"Y: {_coords.y}\n" +
            $"ID: {_id}\n" +
            $"Tile ID: {_tileId}\n" +
            $"Block type: {_blockType}\n" +
            $"Name: {_blockData.name}\n" +
            $"Is liquid: {_liquidId != 255}\n" +
            $"Is flow down: {_isFlowsDown}\n" +
            $"Liquid ID: {_liquidId}\n" +
            $"Flow value: {_flowValue}";
    }

    public int GetSize()
    {
        int size = 0;
        size += System.Runtime.InteropServices.Marshal.SizeOf(_id);
        size += System.Runtime.InteropServices.Marshal.SizeOf(_tileId);
        size += System.Runtime.InteropServices.Marshal.SizeOf(System.Enum.GetUnderlyingType(typeof(BlockTypes)));
        size += System.IntPtr.Size;
        size += System.Runtime.InteropServices.Marshal.SizeOf(_coords);
        size += System.Runtime.InteropServices.Marshal.SizeOf(_liquidId);
        size += System.Runtime.InteropServices.Marshal.SizeOf(_isFlowsDown);
        size += System.Runtime.InteropServices.Marshal.SizeOf(_flowValue);
        return size;
    }

    public void SetData(BlockSO block)
    {
        Id = block.GetId();
        TileId = 255;
        BlockType = block.Type;
        BlockData = block;
    }

    public void SetData(byte id)
    {
        LiquidId = id;
        FlowValue = 100f;
    }

    public TileBase GetTile()
    {
        if (TileId != 255)
        {
            return BlockData.Tiles[_tileId];
        }
        if (BlockData.Tiles.Count == 0)
        {
            return null;
        }
        TileId = (byte)Random.Range(0, BlockData.Tiles.Count);
        return BlockData.Tiles[TileId];
    }

    public bool CompareBlock(BlockSO block)
    {
        return _blockData.GetId() == block.GetId() && _blockData.Type == block.Type;
    }

    public bool IsEmpty()
    {
        return _blockType == BlockTypes.Abstract && !IsLiquid();
    }

    public bool IsEmptyWithPlant()
    {
        return IsEmpty() || _blockType == BlockTypes.Plant;
    }

    public bool IsSolid()
    {
        return _blockType == BlockTypes.Solid || _blockType == BlockTypes.Dust;
    }

    public bool IsLiquid()
    {
        return LiquidId != 255;
    }
    #endregion
}
