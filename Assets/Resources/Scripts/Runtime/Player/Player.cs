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
    [SerializeField]
    private PlayerNetwork _playerNetwork;
    [SerializeField]
    private bool _isOwner;
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

    public PlayerNetwork PlayerNetwork
    {
        get
        {
            return _playerNetwork;
        }
    }

    public bool IsOwner
    {
        get
        {
            return _isHost;
        }

        set
        {
            _isHost = value;
        }
    }
    #endregion

    #region Methods
    private void Awake()
    {
        _playerNetwork = GetComponent<PlayerNetwork>();
    }

    public void Initialize()
    {
        _stats.Reset();
        _inventory.Initialize();
    }

    public void DisableMovement()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<PlayerMovementNew>().enabled = false;
    }

    public void EnableMovement()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GetComponent<PlayerMovementNew>().enabled = true;
    }
    #endregion
}
