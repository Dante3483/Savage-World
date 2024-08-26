using SavageWorld.Runtime;
using SavageWorld.Runtime.Enums.Network;
using SavageWorld.Runtime.Network.Objects;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PickUpItem : GameObjectBase
{
    #region Fields
    [Header("Main Properties")]
    [SerializeField] private PickUpItemsID _id;

    [Header("Spawn Properties")]
    [SerializeField] private BiomesID[] _biomesToSpawn;
    [SerializeField] private int _chanceToSpawn;

    private Vector2Int _intPosition = new();
    #endregion

    #region Properties
    public PickUpItemsID Id
    {
        get
        {
            return _id;
        }

        set
        {
            _id = value;
        }
    }

    public int ChanceToSpawn
    {
        get
        {
            return _chanceToSpawn;
        }

        set
        {
            _chanceToSpawn = value;
        }
    }

    public BiomesID[] BiomesToSpawn
    {
        get
        {
            return _biomesToSpawn;
        }

        set
        {
            _biomesToSpawn = value;
        }
    }
    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods
    private void Awake()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        NetworkObject.Type = NetworkObjectTypes.Environment;
        _intPosition.x = Mathf.FloorToInt(transform.position.x);
        _intPosition.y = Mathf.FloorToInt(transform.position.y);
        WorldDataManager.Instance.SetOccupiedFlag(_intPosition.x, _intPosition.y, true);
    }

    private void Update()
    {
        if (WorldDataManager.Instance.IsEmpty(_intPosition.x, _intPosition.y - 1))
        {
            WorldDataManager.Instance.SetOccupiedFlag(_intPosition.x, _intPosition.y, false);
            DeleteObject();
        }
    }
    #endregion

    #region Public Methods
    public override GameObjectBase CreateInstance(Vector3 position, Transform parent = null, bool isOwner = true)
    {
        return base.CreateInstance(position, GameManager.Instance.Terrain.PickUpItems.transform, isOwner);
    }
    #endregion

    #region Private Methods
    private void DeleteObject()
    {
        Destroy(gameObject);
    }
    #endregion
}
