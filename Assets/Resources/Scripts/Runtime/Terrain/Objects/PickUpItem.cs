using SavageWorld.Runtime;
using SavageWorld.Runtime.Enums.Network;
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
    }

    private void Update()
    {
        _intPosition.x = Mathf.FloorToInt(transform.position.x);
        _intPosition.y = Mathf.FloorToInt(transform.position.y);
        if (WorldDataManager.Instance.IsFree(_intPosition.x, _intPosition.y - 1))
        {
            WorldDataManager.Instance.SetOccupiedFlag(_intPosition.x, _intPosition.y, false);
            DeleteObject();
        }
    }
    #endregion

    #region Public Methods
    public override GameObjectBase CreateInstance(Vector3 position, bool isOwner = true)
    {
        PickUpItem pickUpItemGameObject = Instantiate(this, position, Quaternion.identity, GameManager.Instance.Terrain.PickUpItems.transform);
        pickUpItemGameObject.name = gameObject.name;
        NetworkObject.IsOwner = isOwner;
        NetworkObject.Type = NetworkObjectTypes.Environment;
        return pickUpItemGameObject;
    }
    #endregion

    #region Private Methods
    private void DeleteObject()
    {
        Destroy(gameObject);
    }
    #endregion
}
