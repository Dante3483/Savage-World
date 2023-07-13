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
            _blockData = GameManager.Instance.ObjectsAtlass.FindBlockById(value, _id);
            _blockType = value;
        }
    }
    #endregion

    #region Methods
    public WorldCellData()
    {
        //Set Ait block by default
        _blockData = GameManager.Instance.ObjectsAtlass.Air;
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
    }

    public void SetData(ushort id, BlockTypes blockType)
    {
        Id = id;
        BlockType = blockType;
    }

    public override string ToString()
    {
        return $"X: {_coords.x}\nY: {_coords.y}";
    }
    #endregion
}
