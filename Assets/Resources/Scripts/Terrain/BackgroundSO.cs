using UnityEngine;

[CreateAssetMenu(fileName = "newBackground", menuName = "Blocks/Background")]
public class BackgroundSO : BlockSO
{
    #region Private fields
    [SerializeField] private BackgroundsID _id;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public BackgroundSO()
    {
        Type = BlockTypes.Background;
    }
    public override ushort GetId()
    {
        return (ushort)_id;
    }
    #endregion
}
