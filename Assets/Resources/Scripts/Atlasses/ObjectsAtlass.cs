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
    public BlockSO Clay;
    public BlockSO IronOre;
    public BlockSO CopperOre;
    #endregion

    #region Dust blocks
    [Header("Dust")]
    public BlockSO Sand;
    #endregion

    #region Liquid blocks
    [Header("Liquid")]
    public BlockSO Water;
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
            Clay,
            IronOre,
            CopperOre,
        }.ToList().OrderBy(x => x.GetId()).ToArray());
        #endregion

        #region Dust
        Blocks.Add(BlockTypes.Dust, new BlockSO[]
        {
            Sand,
        }.ToList().OrderBy(x => x.GetId()).ToArray());
        #endregion

        #region Liquid
        Blocks.Add(BlockTypes.Liquid, new BlockSO[]
        {
            Water,
        }.ToList().OrderBy(x => x.GetId()).ToArray());
        #endregion
    }

    public BlockSO GetBlockById(BlockTypes type, object id)
    {
        return Blocks[type][(ushort)id];
    }
    #endregion
}
