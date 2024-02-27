using Random = System.Random;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

//Firt 4 bit of _tileId = blockTileId
//Last 4 bit of _tileId = backgroundTileId
public struct WorldCellData
{
    #region Private fields

    #region Main
    private ushort _blockId; //Save
    private ushort _backgroundId; //Save
    private byte _tileId; //Save
    private BlockTypes _blockType; //Save

    private BlockSO _blockData;
    private BlockSO _backgroundData;

    private Vector2Ushort _coords;
    private byte _currentActionTime;
    #endregion

    #region Liquid
    private byte _liquidId; //Save
    private bool _isFlowsDown;
    private float _flowValue; //Save
    private byte _countToStop;
    #endregion

    #endregion

    #region Public fields

    #endregion

    #region Properties
    public ushort BlockId
    {
        get
        {
            return _blockId;
        }

        set
        {
            _blockId = value;
        }
    }

    public ushort BackgroundId
    {
        get
        {
            return _backgroundId;
        }

        set
        {
            _backgroundId = value;
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

    public byte BlockTileId
    {
        get
        {
            return (byte)((byte)(_tileId & 0b0000_1111));
        }

        set
        {
            _tileId = (byte)((_tileId & 0b1111_0000) | value);
        }
    }

    public byte BackgroundTileId
    {
        get
        {
            return (byte)(_tileId >> 4);
        }

        set
        {
            _tileId = (byte)((_tileId & 0b0000_1111) | (value << 4));
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

    public byte CountToStop
    {
        get
        {
            return _countToStop;
        }

        set
        {
            _countToStop = value;
        }
    }
    #endregion

    #region Methods
    public WorldCellData(ushort xPosition, ushort yPosition, BlockSO block, BlockSO background)
    {
        //Set Ait block by default
        _blockId = 0;
        _backgroundId = 0;
        _tileId = 0;
        _blockType = BlockTypes.Abstract;
        _blockData = block;
        _backgroundData = background;
        _coords = new Vector2Ushort { x = xPosition, y = yPosition };
        _currentActionTime = 0;

        _liquidId = 255;
        _isFlowsDown = false;
        _flowValue = 0;
        _countToStop = 0;
    }

    public override string ToString()
    {
        return $"X: {_coords.x}\n" +
            $"Y: {_coords.y}\n" +
            $"ID: {_blockId}\n" +
            $"Tile ID: {BlockTileId}\n" +
            $"Block type: {_blockType}\n" +
            $"Name: {_blockData.name}\n" +
            $"Background ID: {_backgroundData.GetId()}\n" +
            $"Background type: {_backgroundData.Type}\n" +
            $"Background name: {_backgroundData.name}\n" +
            $"Is liquid: {_liquidId != 255}\n" +
            $"Is flow down: {_isFlowsDown}\n" +
            $"Liquid ID: {_liquidId}\n" +
            $"Flow value: {_flowValue}\n";
    }

    public Sprite GetBlockSprite()
    {
        if (BlockData.Sprites.Count == 0)
        {
            return null;
        }
        return BlockData.Sprites[BlockTileId];
    }

    public Sprite GetBackgroundSprite()
    {
        if (BackgroundData.Sprites.Count == 0)
        {
            return null;
        }
        return BackgroundData.Sprites[BackgroundTileId];
    }

    public Sprite GetLiquidSprite()
    {
        BlockSO liquidData = GetLiquid();
        int count = liquidData.Sprites.Count;
        if (count == 0)
        {
            return null;
        }
        if (_flowValue == 100f || _isFlowsDown)
        {
            return liquidData.Sprites.Last();
        }
        else
        {
            return liquidData.Sprites[(int)(_flowValue / count)];
        }
    }

    public BlockSO GetLiquid()
    {
        return GameManager.Instance.BlocksAtlas.GetBlockById(_liquidId);
    }

    public byte GetDustActionTime()
    {
        return (_blockData as DustBlockSO).FallingTime;
    }

    public byte GetLiquidActionTime()
    {
        return (GameManager.Instance.BlocksAtlas.GetBlockById(_liquidId) as LiquidBlockSO).FlowTime;
    }

    public void SetBlockData(BlockSO block)
    {
        BlockId = block.GetId();
        BlockType = block.Type;
        BlockData = block;
    }

    public void SetLiquidBlockData(byte id)
    {
        LiquidId = id;
        FlowValue = 100f;
    }

    public void SetLiquidBlockData(byte id, float flowValue)
    {
        LiquidId = id;
        FlowValue = flowValue;
    }

    public void SetBackgroundData(BlockSO background)
    {
        BackgroundId = background.GetId();
        BackgroundData = background;
    }

    public void SetRandomBlockTile(Random randomVar)
    {
        BlockTileId = (byte)randomVar.Next(0, BlockData.Sprites.Count);
    }

    public void SetRandomBackgroundTile(Random randomVar)
    {
        BackgroundTileId = (byte)randomVar.Next(0, BackgroundData.Sprites.Count);
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

    public bool IsDust()
    {
        return _blockType == BlockTypes.Dust;
    }

    public bool IsLiquid()
    {
        return LiquidId != 255;
    }

    public bool IsFurniture()
    {
        return BlockType == BlockTypes.Furniture;
    }

    public bool IsEmptyForTree()
    {
        return _blockType == BlockTypes.Abstract || _blockType == BlockTypes.Plant;
    }

    public bool IsEmptyForLiquid()
    {
        return BlockType == BlockTypes.Abstract || BlockType == BlockTypes.Plant || BlockType == BlockTypes.Furniture;
    }

    public bool IsEmptyForPlant()
    {
        return BlockType == BlockTypes.Abstract || BlockType == BlockTypes.Furniture;
    }

    public bool IsBackground()
    {
        return BackgroundData != GameManager.Instance.BlocksAtlas.AirBG;
    }

    public bool IsDayLightBlock()
    {
        return BackgroundData.GetId() == (ushort)BackgroundsID.Air;
    }

    public bool IsFullLiquidBlock()
    {
        return IsLiquid() && FlowValue == 100;
    }

    public void Drain()
    {
        FlowValue = 0f;
        LiquidId = 255;
    }
    #endregion
}
