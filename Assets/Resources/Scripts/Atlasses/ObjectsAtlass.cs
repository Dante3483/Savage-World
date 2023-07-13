using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "newObjectsAtlass", menuName = "Atlasses/Objects")]
public class ObjectsAtlass : ScriptableObject
{
    #region Dictionaries
    public Dictionary<BlockTypes, BlockSO[]> Blocks;
    #endregion

    #region Abstract blocks
    [Header("Abstract")]
    public BlockSO Air;
    #endregion

    #region Solid blocks
    [Header("Solid")]
    public BlockSO Dirt;
    #endregion

    #region Methods
    public void Initialize()
    {
        Blocks = new Dictionary<BlockTypes, BlockSO[]>();

        #region Solid
        Blocks.Add(BlockTypes.Solid, new BlockSO[]
        {
            Dirt,
        });
        #endregion
    }

    public BlockSO FindBlockById(BlockTypes type, object id)
    {
        return Blocks[type].ToList().Find(b => b.GetId() == (ushort)id);
    }
    #endregion
}
