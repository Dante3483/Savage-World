using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "newObjectsAtlass", menuName = "Atlasses/Objects")]
public class ObjectsAtlass : ScriptableObject
{
    #region Private fields

    #endregion

    #region Public fields

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
    public BlockSO Stone;
    #endregion

    #endregion

    #region Properties

    #endregion

    #region Methods
    public void Initialize()
    {
        //Initialize lists
        Blocks = new Dictionary<BlockTypes, BlockSO[]>();


        //Fill lists
        #region Solid
        Blocks.Add(BlockTypes.Solid, new BlockSO[]
        {
            Dirt,
            Stone,
        }.ToList().OrderBy(x => x.GetId()).ToArray());
        #endregion
    }

    public BlockSO GetBlockById(BlockTypes type, object id)
    {
        return Blocks[type][(ushort)id];
    }
    #endregion
}
