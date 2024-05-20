using UnityEngine;

public class Player : MonoBehaviour
{
    #region Private fields
    [SerializeField]
    private string _name;
    [SerializeField]
    private PlayerStats _stats;
    [SerializeField]
    private Inventory _inventory;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public Inventory Inventory
    {
        get
        {
            return _inventory;
        }

        set
        {
            _inventory = value;
        }
    }

    public PlayerStats Stats
    {
        get
        {
            return _stats;
        }

        set
        {
            _stats = value;
        }
    }
    #endregion

    #region Methods
    public void Initialize()
    {
        _stats.Reset();
        _inventory.Initialize();
    }
    #endregion
}
