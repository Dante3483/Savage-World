using UnityEngine;

[CreateAssetMenu(fileName = "Pickaxe", menuName = "Items/Pickaxe")]
public class PickaxeItemSO : ToolItemSO
{
    #region Fields
    [SerializeField]
    [Min(1)]
    private float _miningDamage = 1;
    #endregion

    #region Properties
    public float MiningDamage
    {
        get
        {
            return _miningDamage;
        }
    }
    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public PickaxeItemSO()
    {
        _toolType = ToolTypes.Pickaxe;
        _using = "Can break blocks";
    }
    #endregion

    #region Private Methods

    #endregion
}