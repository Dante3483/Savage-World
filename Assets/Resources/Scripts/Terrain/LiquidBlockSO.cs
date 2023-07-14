using UnityEngine;

[CreateAssetMenu(fileName = "newLiquidBlock", menuName = "Blocks/Liquid")]
public class LiquidBlockSO : BlockSO
{
    #region Private fields
    [SerializeField] private LiquidBlocksID _id;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public LiquidBlockSO()
    {
        Type = BlockTypes.Liquid;
    }

    public override ushort GetId()
    {
        return (ushort) _id;
    }
    #endregion
}
