using Items;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemsAtlas", menuName = "Atlases/ItemsAtlas")]
public class ItemsAtlasSO : AtlasSO
{
    #region Private fields
    [SerializeField]
    private ItemSO[] _items;

    private Dictionary<ItemsID, ItemSO> _itemsByIndex;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public override void InitializeAtlas()
    {
        _itemsByIndex = new();
        foreach (ItemSO item in _items)
        {
            _itemsByIndex.Add(item.Id, item);
        }
    }

    public ItemSO GetItemByID(ItemsID id)
    {
        return _itemsByIndex[id];
    }
    #endregion
}
