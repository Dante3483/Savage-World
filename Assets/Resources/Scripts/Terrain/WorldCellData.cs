using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public struct WorldCellData
{
    #region Private fields

    #region Main
    private ushort _id;
    private byte _blockTileId;
    private byte _backgroundTileId;
    private BlockTypes _blockType;

    private BlockSO _blockData;
    private BlockSO _backgroundData;

    private Vector2Ushort _coords;
    private byte _currentActionTime;

    public float Brightness;
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

    public byte BlockTileId
    {
        get
        {
            return _blockTileId;
        }

        set
        {
            _blockTileId = value;
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

    public byte CurrentActionTime
    {
        get
        {
            return _currentActionTime;
        }

        set
        {
            _currentActionTime = value;
        }
    }

    public BlockSO BackgroundData
    {
        get
        {
            return _backgroundData;
        }

        set
        {
            _backgroundData = value;
        }
    }

    public byte BackgroundTileId
    {
        get
        {
            return _backgroundTileId;
        }

        set
        {
            _backgroundTileId = value;
        }
    }
    #endregion

    #region Methods
    public WorldCellData(ushort xPosition, ushort yPosition)
    {
        //Set Ait block by default
        _id = 0;
        _blockTileId = 255;
        _backgroundTileId = 255;
        _blockType = BlockTypes.Abstract;
        _blockData = GameManager.Instance.ObjectsAtlass.Air;
        _backgroundData = GameManager.Instance.ObjectsAtlass.AirBG;
        _coords = new Vector2Ushort { x = xPosition, y = yPosition };
        _currentActionTime = 0;

        _liquidId = 255;
        _isFlowsDown = false;
        _flowValue = 0;

        Brightness = 0;
    }

    public override string ToString()
    {
        return $"X: {_coords.x}\n" +
            $"Y: {_coords.y}\n" +
            $"ID: {_id}\n" +
            $"Tile ID: {_blockTileId}\n" +
            $"Block type: {_blockType}\n" +
            $"Name: {_blockData.name}\n" +
            $"Background ID: {_backgroundData.GetId()}\n" +
            $"Background type: {_backgroundData.Type}\n" +
            $"Background name: {_backgroundData.name}\n" +
            $"Is liquid: {_liquidId != 255}\n" +
            $"Is flow down: {_isFlowsDown}\n" +
            $"Liquid ID: {_liquidId}\n" +
            $"Flow value: {_flowValue}\n" +
            $"Brightness: {Brightness}";
    }

    public TileBase GetBlockTile()
    {
        if (BlockTileId != 255)
        {
            return BlockData.Tiles[BlockTileId];
        }
        if (BlockData.Tiles.Count == 0)
        {
            return null;
        }
        BlockTileId = (byte)Random.Range(0, BlockData.Tiles.Count);
        return BlockData.Tiles[BlockTileId];
    }

    public TileBase GetBackgroundTile()
    {
        if (BackgroundTileId != 255)
        {
            return BackgroundData.Tiles[BackgroundTileId];
        }
        if (BackgroundData.Tiles.Count == 0)
        {
            return null;
        }
        BackgroundTileId = (byte)Random.Range(0, BackgroundData.Tiles.Count);
        return BackgroundData.Tiles[BackgroundTileId];
    }

    public BlockSO GetBlock()
    {
        return GameManager.Instance.ObjectsAtlass.Blocks[_blockType][_id];
    }

    public BlockSO GetLiquid()
    {
        return GameManager.Instance.ObjectsAtlass.Blocks[BlockTypes.Liquid][_liquidId];
    }

    public BlockSO GetBackground()
    {
        return GameManager.Instance.ObjectsAtlass.Blocks[BlockTypes.Background][0];
    }

    public byte GetDustActionTime()
    {
        return (_blockData as DustBlockSO).FallingTime;
    }

    public byte GetLiquidActionTime()
    {
        return (GameManager.Instance.ObjectsAtlass.Blocks[BlockTypes.Liquid][_liquidId] as LiquidBlockSO).FlowTime;
    }

    public void SetBlockData(BlockSO block)
    {
        Id = block.GetId();
        BlockTileId = 255;
        BlockType = block.Type;
        BlockData = block;
    }

    public void SetBlockData(byte id)
    {
        LiquidId = id;
        FlowValue = 100f;
    }

    public void SetBackgroundData(BlockSO background)
    {
        BackgroundData = background;
    }

    public bool CompareBlock(BlockSO block)
    {
        return _blockData.GetId() == block.GetId() && _blockData.Type == block.Type;
    }

    public bool IsEmpty()
    {
        return _blockType == BlockTypes.Abstract;
    }

    public bool IsPlant()
    {
        return _blockType == BlockTypes.Plant;
    }

    public bool IsSolid()
    {
        return _blockType == BlockTypes.Solid || _blockType == BlockTypes.Dust;
    }

    public bool IsLiquid()
    {
        return LiquidId != 255;
    }

    public bool IsEmptyOrPlant()
    {
        return _blockType == BlockTypes.Abstract || _blockType == BlockTypes.Plant;
    }

    public bool IsBackground()
    {
        return BackgroundData != GameManager.Instance.ObjectsAtlass.AirBG;
    }

    public bool IsDayLightBlock()
    {
        return BackgroundData.GetId() == (ushort)BackgroundsID.Air;
    }

    public bool IsFullLiquidBlock()
    {
        return IsLiquid() && FlowValue == 100;
    }
    #endregion
}
