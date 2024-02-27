using UnityEngine;

[CreateAssetMenu(fileName = "newAbstractBlock", menuName = "Blocks/Abstract")]
public class AbstractBlockSO : BlockSO
{
    #region Private fields
    [SerializeField] private AbstractBlocksID _id;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public AbstractBlockSO()
    {
        Type = BlockTypes.Abstract;
    }

    public override ushort GetId()
    {
        return (ushort)_id;
    }
    #endregion
}
