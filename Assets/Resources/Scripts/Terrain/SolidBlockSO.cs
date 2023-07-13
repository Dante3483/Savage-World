using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "newSolidBlock", menuName = "Blocks/Solid")]
public class SolidBlockSO : BlockSO
{
    #region Private fields
    [SerializeField] private SolidBlocksID _id;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public SolidBlockSO()
    {
        Type = BlockTypes.Solid;
    }

    public override ushort GetId()
    {
        return (ushort) _id;
    }
    #endregion
}
