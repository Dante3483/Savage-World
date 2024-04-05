using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "newLiquidBlock", menuName = "Blocks/Liquid")]
public class LiquidBlockSO : BlockSO
{
    #region Private fields
    [SerializeField] private LiquidBlocksID _id;
    [SerializeField] private ushort _flowTime;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public ushort FlowTime
    {
        get
        {
            return _flowTime;
        }

        set
        {
            _flowTime = value;
        }
    }
    #endregion

    #region Methods
    public LiquidBlockSO()
    {
        _type = BlockTypes.Liquid;
    }

    public override ushort GetId()
    {
        return (ushort) _id;
    }
    #endregion
}
