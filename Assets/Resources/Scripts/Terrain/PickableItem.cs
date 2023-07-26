using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PickableItem : MonoBehaviour
{
    #region Private fields
    [SerializeField]private PickableItemsID _id;
    [SerializeField] private int _chanceToSpawn;
    private Vector2Int _intPosition = new Vector2Int();
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public PickableItemsID Id
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
    #endregion

    #region Methods
    private void Update()
    {
        _intPosition.x = Mathf.FloorToInt(transform.position.x);
        _intPosition.y = Mathf.FloorToInt(transform.position.y);
        if (GameManager.Instance.WorldData[_intPosition.x, _intPosition.y - 1].IsEmpty())
        {
            DeleteObject();
        }
    }

    private void DeleteObject()
    {
        Destroy(gameObject);
    }
    #endregion

}
