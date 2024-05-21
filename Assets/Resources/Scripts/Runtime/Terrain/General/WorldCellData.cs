using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Random = System.Random;

public struct WorldCellData : INetworkSerializable
{
    #region Private fields

    #region Main
    private ushort _blockId;
    private ushort _wallId;
    private BlockTypes _blockType;
    private Vector2Ushort _coords;

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
    private byte _currentActionTime;
    private byte _blockDamagePercent;
    private byte _wallDamagePercent;
    private byte _colliderIndex;
    #endregion

    #region Liquid
    private byte _liquidId;
    private bool _isFlowsDown;
    private float _flowValue;
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

    public ushort WallId
    {
        get
        {
            return _wallId;
        }

        set
        {
            _wallId = value;
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

    public byte WallTileId
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

    public byte BlockDamagePercent
    {
        get
        {
            return _blockDamagePercent;
        }
    }

    public byte WallDamagePercent
    {
        get
        {
            return _wallDamagePercent;
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
    #endregion

    #region Methods
    public WorldCellData(ushort xPosition, ushort yPosition, BlockSO block, BlockSO wall)
    {
        _blockId = 0;
        _wallId = 0;
        _tileId = 0;
        _blockType = BlockTypes.Abstract;
        _blockData = block;
        _wallData = wall;
        _coords = new Vector2Ushort { x = xPosition, y = yPosition };
        _currentActionTime = 0;
        _blockDamagePercent = 0;
        _wallDamagePercent = 0;
        _flags = 0;
        _colliderIndex = byte.MaxValue;

        _liquidId = byte.MaxValue;
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
            $"Wall ID: {_wallData.GetId()}\n" +
            $"Wall type: {_wallData.Type}\n" +
            $"Wall name: {_wallData.name}\n" +
            $"Is liquid: {_liquidId != 255}\n" +
            $"Is flow down: {_isFlowsDown}\n" +
            $"Liquid ID: {_liquidId}\n" +
            $"Flow value: {_flowValue}\n" +
            $"Block damage: {_blockDamagePercent}\n" +
            $"Wall damage: {_wallDamagePercent}\n" +
            $"Flags: {_flags}\n" +
            $"Is breakable: {IsBreakable()}\n" +
            $"Is tree: {IsTree()}\n" +
            $"Is tree trunk: {IsTreeTrunk()}\n" +
            $"Is free: {IsFree()}\n" +
            $"Is horizontal flip: {IsColliderHorizontalFlipped()}" +
            $"Collider type: {_colliderIndex}\n";
    }

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

    public uint GetDustActionTime()
    {
        return (_blockData as DustBlockSO).FallingTime;
    }

    public ushort GetLiquidActionTime()
    {
        return (GameManager.Instance.BlocksAtlas.GetBlockById(_liquidId) as LiquidBlockSO).FlowTime;
    }

    public void SetBlockData(BlockSO block)
    {
        _blockId = block.GetId();
        _blockType = block.Type;
        _blockData = block;
        _blockDamagePercent = 0;
    }

    public void SetLiquidBlockData(byte id)
    {
        _liquidId = id;
        _flowValue = 100f;
    }

    public void SetLiquidBlockData(byte id, float flowValue)
    {
        _liquidId = id;
        _flowValue = flowValue;
    }

    public void SetWallData(BlockSO wall)
    {
        _wallId = wall.GetId();
        _wallData = wall;
        _wallDamagePercent = 0;
    }

    public void SetRandomBlockTile(Random randomVar)
    {
        BlockTileId = (byte)randomVar.Next(0, BlockData.Sprites.Count);
    }

    public void SetRandomWallTile(Random randomVar)
    {
        WallTileId = (byte)randomVar.Next(0, WallData.Sprites.Count);
    }

    public void SetBlockDamagePercent(float damage)
    {
        _blockDamagePercent = (byte)(Mathf.Min(damage / _blockData.DamageToBreak, 1) * 100);
    }

    public void SetWallDamagePercent(float damage)
    {
        _wallDamagePercent = (byte)(Mathf.Min(damage / WallData.DamageToBreak, 1) * 100);
    }

    public void MakeUnbreakable()
    {
        _flags |= StaticInfo.Bit1;
    }

    public void MakeBreakable()
    {
        _flags &= StaticInfo.InvertedBit1;
    }

    public void MakeOccupied()
    {
        _flags |= StaticInfo.Bit2;
    }

    public void MakeFree()
    {
        _flags &= StaticInfo.InvertedBit2;
    }

    public void MakeTree()
    {
        _flags |= StaticInfo.Bit3;
    }

    public void RemoveTree()
    {
        _flags &= StaticInfo.InvertedBit3;
    }

    public void MakeTreeTrunk()
    {
        _flags |= StaticInfo.Bit4;
    }

    public void RemoveTreeTrunk()
    {
        _flags &= StaticInfo.InvertedBit4;
    }

    public void MakeColliderHorizontalFlipped()
    {
        _flags |= StaticInfo.Bit5;
    }

    public void RemoveColliderHorizontalFlipped()
    {
        _flags &= StaticInfo.InvertedBit5;
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
        return _liquidId != 255;
    }

    public bool IsFurniture()
    {
        return _blockType == BlockTypes.Furniture;
    }

    public bool IsEmptyForTree()
    {
        return _blockType == BlockTypes.Abstract || _blockType == BlockTypes.Plant;
    }

    public bool IsEmptyForLiquid()
    {
        return _blockType == BlockTypes.Abstract || _blockType == BlockTypes.Plant || _blockType == BlockTypes.Furniture;
    }

    public bool IsEmptyForPlant()
    {
        return _blockType == BlockTypes.Abstract || _blockType == BlockTypes.Furniture;
    }

    public bool IsWall()
    {
        return _wallData != GameManager.Instance.BlocksAtlas.AirWall;
    }

    public bool IsDayLightBlock()
    {
        return _wallData.GetId() == (ushort)WallsID.Air;
    }

    public bool IsFullLiquidBlock()
    {
        return IsLiquid() && _flowValue == 100;
    }

    public bool IsBreakable()
    {
        return (_flags & StaticInfo.Bit1) == 0;
    }

    public bool IsFree()
    {
        return (_flags & StaticInfo.Bit2) == 0 && !IsTree() && !IsTreeTrunk();
    }

    public bool IsTree()
    {
        return (_flags & StaticInfo.Bit3) == StaticInfo.Bit3;
    }

    public bool IsTreeTrunk()
    {
        return (_flags & StaticInfo.Bit4) == StaticInfo.Bit4;
    }

    public bool IsColliderHorizontalFlipped()
    {
        return (_flags & StaticInfo.Bit5) == StaticInfo.Bit5;
    }

    public void Drain()
    {
        _liquidId = 255;
        _flowValue = 0f;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _blockId);
        serializer.SerializeValue(ref _wallId);
        serializer.SerializeValue(ref _blockType);
    }
    #endregion
}
