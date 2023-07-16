using UnityEngine;
using UnityEngine.Tilemaps;

public struct WorldCellData
{
    #region Private fields
    private ushort _id;
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
    #endregion

    #region Methods
    public WorldCellData(ushort xPosition, ushort yPosition)
    {
        //Set Ait block by default
        _id = 0;
        _blockType = BlockTypes.Abstract;
        _blockData = GameManager.Instance.ObjectsAtlass.Air;
        _coords = new Vector2Ushort { x = xPosition, y = yPosition };
    }

    public void SetData(BlockSO block)
    {
        Id = block.GetId();
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
        if (BlockData.Tiles.Count == 0)
        {
            return null;
        }
        return BlockData.Tiles[Random.Range(0, BlockData.Tiles.Count)];
    }

    public bool CompareBlock(BlockSO block)
    {
        return _blockData.GetId() == block.GetId() && _blockData.Type == block.Type;
    }
    #endregion
}
