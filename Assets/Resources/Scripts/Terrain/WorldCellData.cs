using UnityEngine;
using UnityEngine.Tilemaps;

public struct WorldCellData
{
    #region Private fields
    private ushort _id;
    private byte _tileId;
    private BlockTypes _blockType;
    private BlockSO _blockData;
    private Vector2Ushort _coords;
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
    }

    public void SetData(BlockSO block)
    {
        Id = block.GetId();
        TileId = 255;
        BlockType = block.Type;
        BlockData = block;
    }

    public void SetData(ushort id, BlockTypes blockType)
    {
        Id = id;
        BlockType = blockType;
        BlockData = GameManager.Instance.ObjectsAtlass.GetBlockById(blockType, id);
    }

    public override string ToString()
    {
        return $"{_coords.x}, {_coords.y}, {_id}, {_blockType}, {_blockData.name}";
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
        return _blockType == BlockTypes.Abstract;
    }

    public bool IsEmptyWithPlant()
    {
        return _blockType == BlockTypes.Abstract || _blockType == BlockTypes.Plant;
    }


    public bool IsSolid()
    {
        return _blockType == BlockTypes.Solid || _blockType == BlockTypes.Dust;
    }
    #endregion
}
