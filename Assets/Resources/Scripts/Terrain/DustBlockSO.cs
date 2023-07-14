using UnityEngine;

[CreateAssetMenu(fileName = "newDustBlock", menuName = "Blocks/Dust")]
public class DustBlockSO : BlockSO
{
    #region Private fields
    [SerializeField] private DustBlocksID _id;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public DustBlockSO()
    {
        Type = BlockTypes.Dust;
    }

    public override ushort GetId()
    {
        return (ushort) _id;
    }
    #endregion
}
