using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct WorldCellData
{
    #region Fields
    public ushort BlockId;
    public ushort WallId;
    public byte LiquidId;
    public BlockTypes BlockType;
    public float FlowValue;
    public byte ColliderIndex;
    /// <summary>
    /// <br>First 4 bits of _tileId = blockTileId</br>
    /// <br>Last 4 bits of _tileId = wallTileId</br>
    /// </summary>
    public byte TileId;
    /// <summary>
    /// <br>1 bit of _flags = is unbreakable</br>
    /// <br>2 bit of _flags = is occupied</br>
    /// <br>3 bit of _flags = is tree</br>
    /// <br>4 bit of _flags = is tree trunk</br>
    /// <br>5 bit of _flags = is collider horizontal flipped</br>
    /// </summary>
    public byte Flags;
    #endregion

    #region Properties
    public int BlockTileId => TileId & 0b0000_1111;

    public int WallTileId => TileId >> 4;

    public bool IsEmpty => BlockType == BlockTypes.Abstract;

    public bool IsSolid => BlockType == BlockTypes.Solid;

    public bool IsDust => BlockType == BlockTypes.Dust;

    public bool IsLiquid => LiquidId != byte.MaxValue;

    public bool IsPlant => BlockType == BlockTypes.Plant;

    public bool IsWall => WallId != (ushort)WallsID.Air;

    public bool IsFurniture => BlockType == BlockTypes.Furniture;

    public bool IsUnbreakable => (Flags & StaticInfo.Bit1) == StaticInfo.Bit1;

    public bool IsOccupied => (Flags & StaticInfo.Bit2) == StaticInfo.Bit2;

    public bool IsTree => (Flags & StaticInfo.Bit3) == StaticInfo.Bit3;

    public bool IsTreeTrunk => (Flags & StaticInfo.Bit4) == StaticInfo.Bit4;

    public bool IsColliderHorizontalFlipped => (Flags & StaticInfo.Bit5) == StaticInfo.Bit5;

    public bool IsFree => !IsOccupied && !IsTree && !IsTreeTrunk;

    public bool IsValidForTree => IsEmpty || IsPlant;

    public bool IsValidForLiquid => IsEmpty || IsPlant || IsFurniture;

    public bool IsValidForPlant => IsEmpty || IsFurniture;

    public bool IsDayLightBlock => WallId == (ushort)WallsID.Air;

    public bool IsLiquidFull => IsLiquid && FlowValue == 100;

    public bool IsPhysicallySolidBlock => IsSolid || IsDust;
    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public static WorldCellData GetEmpty()
    {
        return new()
        {
            BlockId = 0,
            WallId = 0,
            TileId = 0,
            BlockType = BlockTypes.Abstract,
            Flags = 0,
            ColliderIndex = byte.MaxValue,
            LiquidId = byte.MaxValue,
            FlowValue = 0,
        };
    }

    public void SetBlockData(BlockSO data)
    {
        if (data == null)
        {
            return;
        }
        BlockId = data.GetId();
        BlockType = data.Type;
    }

    public void SetWallData(BlockSO data)
    {
        if (data == null)
        {
            return;
        }
        WallId = data.GetId();
    }

    public void SetLiquidData(BlockSO data)
    {
        SetLiquidData(data, 100);
    }

    public void SetLiquidData(BlockSO data, float flowValue)
    {
        if (data == null)
        {
            return;
        }
        LiquidId = (byte)data.GetId();
        FlowValue = flowValue;
    }

    public void SetUnbreakableFlag(bool value)
    {
        if (value)
        {
            Flags |= StaticInfo.Bit1;
        }
        else
        {
            Flags &= StaticInfo.InvertedBit1;
        }
    }

    public void SetOccupiedFlag(bool value)
    {
        if (value)
        {
            Flags |= StaticInfo.Bit2;
        }
        else
        {
            Flags &= StaticInfo.InvertedBit2;
        }
    }

    public void SetTreeFlag(bool value)
    {
        if (value)
        {
            Flags |= StaticInfo.Bit3;
        }
        else
        {
            Flags &= StaticInfo.InvertedBit3;
        }
    }

    public void SetTreeTrunkFlag(bool value)
    {
        if (value)
        {
            Flags |= StaticInfo.Bit4;
        }
        else
        {
            Flags &= StaticInfo.InvertedBit4;
        }
    }

    public void SetColliderHorizontalFlippedFlag(bool value)
    {
        if (value)
        {
            Flags |= StaticInfo.Bit5;
        }
        else
        {
            Flags &= StaticInfo.InvertedBit5;
        }
    }

    public void SetBlockTile(byte id)
    {
        TileId &= 0b1111_0000;
        TileId |= id;
    }

    public void SetWallTile(byte id)
    {
        TileId &= 0b0000_1111;
        TileId |= id;
    }
    #endregion

    #region Private Methods

    #endregion
}
