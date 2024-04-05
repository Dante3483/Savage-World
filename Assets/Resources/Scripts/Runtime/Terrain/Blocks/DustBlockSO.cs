using UnityEngine;

[CreateAssetMenu(fileName = "newDustBlock", menuName = "Blocks/Dust")]
public class DustBlockSO : BlockSO
{
    #region Private fields
    [SerializeField] private DustBlocksID _id;
    [SerializeField] private uint _fallingTime;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public uint FallingTime
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
    #endregion

    #region Methods
    public DustBlockSO()
    {
        _type = BlockTypes.Dust;
    }

    public override ushort GetId()
    {
        return (ushort) _id;
    }
    #endregion
}
