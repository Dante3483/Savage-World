using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldCellData
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
    public WorldCellData()
    {
        //Set Ait block by default
        BlockData = GameManager.Instance.ObjectsAtlass.Dirt;
    }

    public WorldCellData(ushort xPosition, ushort yPosition)
    {
        //Set Ait block by default
        _id = 0;
        BlockData = GameManager.Instance.ObjectsAtlass.Air;
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
        return $"X: {_coords.x}\nY: {_coords.y}";
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
