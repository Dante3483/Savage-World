using SavageWorld.Runtime.Enums.Id;
using SavageWorld.Runtime.Enums.Types;
using SavageWorld.Runtime.Terrain.Tiles;
using SavageWorld.Runtime.Utilities;
using SavageWorld.Runtime.Utilities.Extensions;
using System.Runtime.InteropServices;

namespace SavageWorld.Runtime.Terrain
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Tile
    {
        #region Fields
        public ushort BlockId;
        public ushort WallId;
        public byte LiquidId;
        public TileTypes BlockType;
        public float FlowValue;
        public byte ColliderIndex;
        /// <summary>
        /// <br>First 4 bits of SpriteId = BlockSpriteId</br>
        /// <br>Last 4 bits of SpriteId = WallSpriteId</br>
        /// </summary>
        public byte SpriteId;
        /// <summary>
        /// <br>0 bit of _flags = is unbreakable</br>
        /// <br>1 bit of _flags = is occupied</br>
        /// <br>2 bit of _flags = is tree</br>
        /// <br>3 bit of _flags = is tree trunk</br>
        /// <br>4 bit of _flags = is collider horizontal flipped</br>
        /// <br>5 bit of _flags = is liquid settled</br>
        /// </summary>
        public byte Flags;
        #endregion

        #region Properties
        public int BlockSpriteId => SpriteId & 0b0000_1111;

        public int WallSpriteId => SpriteId >> 4;

        public bool IsEmpty => IsAbstract && !IsLiquid && !IsWall;

        public bool IsAbstract => BlockType == TileTypes.Abstract;

        public bool IsSolid => BlockType == TileTypes.Solid;

        public bool IsDust => BlockType == TileTypes.Dust;

        public bool IsLiquid => LiquidId != byte.MaxValue;

        public bool IsPlant => BlockType == TileTypes.Plant;

        public bool IsWall => WallId != (ushort)WallTilesId.Air;

        public bool IsFurniture => BlockType == TileTypes.Furniture;

        public bool IsUnbreakable => (Flags & StaticParameters.Bit0) == StaticParameters.Bit0;

        public bool IsOccupied => (Flags & StaticParameters.Bit1) == StaticParameters.Bit1;

        public bool IsTree => (Flags & StaticParameters.Bit2) == StaticParameters.Bit2;

        public bool IsTreeTrunk => (Flags & StaticParameters.Bit3) == StaticParameters.Bit3;

        public bool IsColliderHorizontalFlipped => (Flags & StaticParameters.Bit4) == StaticParameters.Bit4;

        public bool IsLiquidSettled => (Flags & StaticParameters.Bit5) == StaticParameters.Bit5;

        public bool IsFree => !IsOccupied && !IsTree && !IsTreeTrunk;

        public bool IsValidForTree => IsAbstract || IsPlant;

        public bool IsValidForLiquid => IsAbstract || IsPlant || IsFurniture;

        public bool IsValidForPlant => IsAbstract || IsFurniture;

        public bool IsDayLightBlock => WallId == (ushort)WallTilesId.Air;

        public bool IsLiquidFull => IsLiquid && FlowValue >= 99f;

        public bool IsPhysicallySolidBlock => IsSolid || IsDust;
        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
        public static Tile GetEmpty()
        {
            return new()
            {
                BlockId = 0,
                WallId = 0,
                SpriteId = 0,
                BlockType = TileTypes.Abstract,
                Flags = 0,
                ColliderIndex = byte.MaxValue,
                LiquidId = byte.MaxValue,
                FlowValue = 0,
            };
        }

        public void SetBlockData(TileBaseSO data)
        {
            if (data == null)
            {
                return;
            }
            BlockId = data.GetId();
            BlockType = data.Type;
        }

        public void SetWallData(TileBaseSO data)
        {
            if (data == null)
            {
                return;
            }
            WallId = data.GetId();
        }

        public void SetLiquidData(TileBaseSO data)
        {
            SetLiquidData(data, 100);
        }

        public void SetLiquidData(TileBaseSO data, float flowValue)
        {
            if (data == null)
            {
                return;
            }
            if (SetFlowValue(flowValue))
            {
                LiquidId = (byte)data.GetId();
            }
        }

        public bool SetFlowValue(float flowValue)
        {
            bool isStillLiquid = true;
            if (flowValue == 0f)
            {
                LiquidId = byte.MaxValue;
                isStillLiquid = false;
            }
            if (FlowValue != flowValue)
            {
                SetLiquidSettledFlag(false);
            }
            FlowValue = flowValue;
            return isStillLiquid;
        }

        public void SetUnbreakableFlag(bool value)
        {
            if (value)
            {
                Flags |= StaticParameters.Bit0;
            }
            else
            {
                Flags &= StaticParameters.InvertedBit0;
            }
        }

        public void SetOccupiedFlag(bool value)
        {
            if (value)
            {
                Flags |= StaticParameters.Bit1;
            }
            else
            {
                Flags &= StaticParameters.InvertedBit1;
            }
        }

        public void SetTreeFlag(bool value)
        {
            if (value)
            {
                Flags |= StaticParameters.Bit2;
            }
            else
            {
                Flags &= StaticParameters.InvertedBit2;
            }
        }

        public void SetTreeTrunkFlag(bool value)
        {
            if (value)
            {
                Flags |= StaticParameters.Bit3;
            }
            else
            {
                Flags &= StaticParameters.InvertedBit3;
            }
        }

        public void SetColliderHorizontalFlippedFlag(bool value)
        {
            if (value)
            {
                Flags |= StaticParameters.Bit4;
            }
            else
            {
                Flags &= StaticParameters.InvertedBit4;
            }
        }

        public void SetLiquidSettledFlag(bool value)
        {
            Flags = Flags.SetBit(5, value);
        }

        public void SetBlockSpriteId(byte id)
        {
            SpriteId &= 0b1111_0000;
            SpriteId |= id;
        }

        public void SetWallSpriteId(byte id)
        {
            SpriteId &= 0b0000_1111;
            SpriteId |= id;
        }
        #endregion

        #region Private Methods

        #endregion
    }
}