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
    #endregion

    #region Methods
    public WorldCellData()
    {
        //Set Ait block by default
        _blockData = GameManager.Instance.ObjectsAtlass.Dirt;
    }

    public WorldCellData(ushort xPosition, ushort yPosition)
    {
        //Set Ait block by default
        _id = 0;
        _blockData = GameManager.Instance.ObjectsAtlass.Air;
        _coords = new Vector2Ushort { x = xPosition, y = yPosition };
    }

    public void SetData(BlockSO block)
    {
        Id = block.GetId();
        BlockType = block.Type;
        _blockData = block;
    }

    public void SetData(ushort id, BlockTypes blockType)
    {
        Id = id;
        BlockType = blockType;
        _blockData = GameManager.Instance.ObjectsAtlass.GetBlockById(blockType, id);
    }

    public override string ToString()
    {
        return $"X: {_coords.x}\nY: {_coords.y}";
    }

    public TileBase GetTile()
    {
        if (_blockData.Tiles.Count == 0)
        {
            return null;
        }
        return _blockData.Tiles[Random.Range(0, _blockData.Tiles.Count)];
    }
    #endregion
}
