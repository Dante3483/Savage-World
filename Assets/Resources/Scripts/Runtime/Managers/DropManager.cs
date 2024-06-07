using Items;
using UnityEngine;

public class DropManager : Singleton<DropManager>
{
    #region Fields
    [SerializeField]
    private Drop _prefab;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public Drop CreateDrop(Vector3 position, ItemSO item, int quantity)
    {
        if (item == null)
        {
            return null;
        }
        Drop drop = Instantiate(_prefab, position, Quaternion.identity);
        drop.Quantity = quantity;
        drop.Item = item;
        return drop;
    }
    #endregion

    #region Private Methods

    #endregion
}
