using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PickableItem : MonoBehaviour
{
    #region Private fields
    [SerializeField]private PickableItemsID _id;
    [SerializeField] private int _chanceToSpawn;
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

    #endregion

}
