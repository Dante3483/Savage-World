using UnityEngine;

[CreateAssetMenu(fileName = "newDustBlock", menuName = "Blocks/Dust")]
public class DustBlockSO : BlockSO
{
    #region Private fields
    [SerializeField] private DustBlocksID _id;
    [SerializeField] private byte _fallingTime;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion
    public byte FallingTime
    {
        get
        {
            return _fallingTime;
        }

        set
        {
            _fallingTime = value;
        }
    }
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
