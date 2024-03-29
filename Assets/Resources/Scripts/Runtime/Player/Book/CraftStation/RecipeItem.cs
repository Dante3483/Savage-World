using Items;
using System;
using UnityEngine;

[Serializable]
public struct RecipeItem
{
    #region Private fields
    [SerializeField] private ItemSO _item;
    [Range(1, 9999)][SerializeField] private int _quantity;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public ItemSO Item
    {
        get
        {
            return _item;
        }
    }

    public int Quantity
    {
        get
        {
            return _quantity;
        }
    }
    #endregion

    #region Methods

    #endregion
}
