using UnityEngine;
using Random = System.Random;

public struct WorldCellData
{
    #region Fields
    private BlockSO _blockData;
    private BlockSO _wallData;
    /// <summary>
    /// <br>First 4 bits of _tileId = blockTileId</br>
    /// <br>Last 4 bits of _tileId = wallTileId</br>
    /// </summary>
    private byte _tileId;
    /// <summary>
    /// <br>1 bit of _flags = is unbreakable</br>
    /// <br>2 bit of _flags = is occupied</br>
    /// <br>3 bit of _flags = is tree</br>
    /// <br>4 bit of _flags = is tree trunk</br>
    /// <br>5 bit of _flags = is collider horizontal flipped</br>
    /// </summary>
    private byte _flags;
    private byte _colliderIndex;

    private byte _liquidId;
    private float _flowValue;
    #endregion

    #region Properties
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

    public BlockSO WallData
    {
        get
        {
            return _wallData;
        }

        set
        {
            _wallData = value;
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

    public byte Flags
    {
        get
        {
            return _flags;
        }

        set
        {
            _flags = value;
        }
    }

    public byte ColliderIndex
    {
        get
        {
            return _colliderIndex;
        }

        set
        {
            _colliderIndex = value;
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

    public ushort BlockId => _blockData.GetId();

    public ushort WallId => _wallData.GetId();

    public BlockTypes BlockType => _blockData.Type;

    public int BlockTileId => _tileId & 0b0000_1111;

    public int WallTileId => _tileId >> 4;

    public bool IsEmpty => _blockData.Type == BlockTypes.Abstract;

    public bool IsSolid => _blockData.Type == BlockTypes.Solid;

    public bool IsDust => _blockData.Type == BlockTypes.Dust;

    public bool IsLiquid => _liquidId != byte.MaxValue;

    public bool IsPlant => _blockData.Type == BlockTypes.Plant;

    public bool IsWall => _wallData != GameManager.Instance.BlocksAtlas.AirWall;

    public bool IsFurniture => _blockData.Type == BlockTypes.Furniture;

    public bool IsUnbreakable => (_flags & StaticInfo.Bit1) == StaticInfo.Bit1;

    public bool IsOccupied => (_flags & StaticInfo.Bit2) == StaticInfo.Bit2;

    public bool IsTree => (_flags & StaticInfo.Bit3) == StaticInfo.Bit3;

    public bool IsTreeTrunk => (_flags & StaticInfo.Bit4) == StaticInfo.Bit4;

    public bool IsColliderHorizontalFlipped => (_flags & StaticInfo.Bit5) == StaticInfo.Bit5;

    public bool IsFree => !IsOccupied && !IsTree && !IsTreeTrunk;

    public bool IsValidForTree => IsEmpty || IsPlant;

    public bool IsValidForLiquid => IsEmpty || IsPlant || IsFurniture;

    public bool IsValidForPlant => IsEmpty || IsFurniture;

    public bool IsDatLightBlock => WallId == (ushort)WallsID.Air;

    public bool IsLiquidFull => IsLiquid && _flowValue == 100;
    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public WorldCellData(ushort xPosition, ushort yPosition, BlockSO block, BlockSO wall)
    {
        //_blockId = 0;
        //_wallId = 0;
        _tileId = 0;
        //_blockType = BlockTypes.Abstract;
        _blockData = block;
        _wallData = (WallSO)wall;
        //_coords = new Vector2Ushort { x = xPosition, y = yPosition };
        //_currentActionTime = 0;
        //_blockDamagePercent = 0;
        //_wallDamagePercent = 0;
        _flags = 0;
        _colliderIndex = byte.MaxValue;

        _liquidId = byte.MaxValue;
        //_isFlowsDown = false;
        _flowValue = 0;
        //_countToStop = 0;
    }

    public void SetBlockData(BlockSO data)
    {
        _blockData = data;
    }

    public void SetWallData(BlockSO data)
    {
        _wallData = data;
    }

    public void SetLiquidData(byte id)
    {
        SetLiquidData(id, 100);
    }

    public void SetLiquidData(byte id, float flowValue)
    {
        _liquidId = id;
        _flowValue = flowValue;
    }

    public void SetUnbreakableFlag(bool value)
    {
        if (value)
        {
            _flags |= StaticInfo.Bit1;
        }
        else
        {
            _flags &= StaticInfo.InvertedBit1;
        }
    }

    public void SetOccupiedFlag(bool value)
    {
        if (value)
        {
            _flags |= StaticInfo.Bit2;
        }
        else
        {
            _flags &= StaticInfo.InvertedBit2;
        }
    }

    public void SetTreeFlag(bool value)
    {
        if (value)
        {
            _flags |= StaticInfo.Bit3;
        }
        else
        {
            _flags &= StaticInfo.InvertedBit3;
        }
    }

    public void SetTreeTrunkFlag(bool value)
    {
        if (value)
        {
            _flags |= StaticInfo.Bit4;
        }
        else
        {
            _flags &= StaticInfo.InvertedBit4;
        }
    }

    public void SetColliderHorizontalFlippedFlag(bool value)
    {
        if (value)
        {
            _flags |= StaticInfo.Bit5;
        }
        else
        {
            _flags &= StaticInfo.InvertedBit5;
        }
    }

    public void SetRandomBlockTile(Random randomVar)
    {
        byte id = (byte)randomVar.Next(0, BlockData.Sprites.Count);
        _tileId &= 0b1111_0000;
        _tileId |= id;
    }

    public void SetRandomWallTile(Random randomVar)
    {
        byte id = (byte)(randomVar.Next(0, WallData.Sprites.Count) << 4);
        _tileId &= 0b0000_1111;
        _tileId |= id;
    }

    //public void SetBlockDamagePercent(float damage)
    //{
    //    _blockDamagePercent = (byte)(Mathf.Min(damage / _blockData.DamageToBreak, 1) * 100);
    //}

    //public void SetWallDamagePercent(float damage)
    //{
    //    _wallDamagePercent = (byte)(Mathf.Min(damage / WallData.DamageToBreak, 1) * 100);
    //}

    public Sprite GetBlockSprite()
    {
        if (_blockData.Sprites.Count == 0)
        {
            return null;
        }
        return _blockData.Sprites[BlockTileId];
    }

    public Sprite GetWallSprite()
    {
        if (_wallData.Sprites.Count == 0)
        {
            return null;
        }
        return _wallData.Sprites[WallTileId];
    }

    public Sprite GetLiquidSprite()
    {
        LiquidBlockSO liquidData = GetLiquidData();
        int count = liquidData.Sprites.Count;
        if (count == 0)
        {
            return null;
        }
        //if (_flowValue == 100f || _isFlowsDown)
        //{
        //    return liquidData.Sprites.Last();
        //}
        else
        {
            return liquidData.Sprites[(int)(_flowValue / count)];
        }
    }

    public uint GetDustFallingTime()
    {
        return (_blockData as DustBlockSO).FallingTime;
    }

    public ushort GetLiquidFlowTime()
    {
        return GetLiquidData().FlowTime;
    }

    public void Drain()
    {
        _liquidId = 255;
        _flowValue = 0f;
    }

    public bool CompareBlock(BlockSO block)
    {
        return _blockData.GetId() == block.GetId() && _blockData.Type == block.Type;
    }

    public override string ToString()
    {
        return //$"X: {_coords.x}\n" +
               //$"Y: {_coords.y}\n" +
               //$"ID: {_blockId}\n" +
            $"Tile ID: {BlockTileId}\n" +
            //$"Block type: {_blockType}\n" +
            $"Name: {_blockData.name}\n" +
            $"Wall ID: {_wallData.GetId()}\n" +
            $"Wall type: {_wallData.Type}\n" +
            $"Wall name: {_wallData.name}\n" +
            $"Is liquid: {_liquidId != 255}\n" +
            //$"Is flow down: {_isFlowsDown}\n" +
            $"Liquid ID: {_liquidId}\n" +
            $"Flow value: {_flowValue}\n" +
            //$"Block damage: {_blockDamagePercent}\n" +
            //$"Wall damage: {_wallDamagePercent}\n" +
            $"Flags: {_flags}\n" +
            //$"Is breakable: {IsBreakable()}\n" +
            //$"Is tree: {IsTree()}\n" +
            //$"Is tree trunk: {IsTreeTrunk()}\n" +
            //$"Is free: {IsFree()}\n" +
            //$"Is horizontal flip: {IsColliderHorizontalFlipped()}" +
            $"Collider type: {_colliderIndex}\n";
    }
    #endregion

    #region Private Methods
    private LiquidBlockSO GetLiquidData()
    {
        return (LiquidBlockSO)GameManager.Instance.BlocksAtlas.GetBlockById(_liquidId);
    }
    #endregion
}
