using UnityEngine;

[CreateAssetMenu(fileName = "Pickaxe", menuName = "Items/Pickaxe")]
public class PickaxeItemSO : ToolItemSO
{
    #region Fields
    [SerializeField]
    [Min(1)]
    private float _miningDamage = 1;
    [SerializeField]
    [Min(0.01f)]
    private float _miningSpeed = 0.5f;
    #endregion

    #region Properties
    public float MiningDamage
    {
        get
        {
            return _miningDamage;
        }
    }

    public float MiningSpeed
    {
        get
        {
            return _miningSpeed;
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