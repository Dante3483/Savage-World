using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PickUpItem : MonoBehaviour
{
    #region Private fields
    [Header("Main Properties")]
    [SerializeField] private PickUpItemsID _id;

    [Header("Spawn Properties")]
    [SerializeField] private BiomesID[] _biomesToSpawn;
    [SerializeField] private int _chanceToSpawn;

    private Vector2Int _intPosition = new();
    #endregion

    #region Public fields

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

    #region Methods
    private void Awake()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    private void Update()
    {
        _intPosition.x = Mathf.FloorToInt(transform.position.x);
        _intPosition.y = Mathf.FloorToInt(transform.position.y);
        if (WorldDataManager.Instance.WorldData[_intPosition.x, _intPosition.y - 1].IsEmpty)
        {
            WorldDataManager.Instance.MakeFree(_intPosition.x, _intPosition.y);
            DeleteObject();
        }
    }

    private void DeleteObject()
    {
        Destroy(gameObject);
    }
    #endregion
}
