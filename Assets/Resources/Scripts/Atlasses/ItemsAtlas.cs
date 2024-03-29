using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newObjectAtlas", menuName = "Atlasses/Item atlas")]
public class ItemsAtlas : ScriptableObject
{
    private List<ItemSO> items;

    #region Blocks
    [Header("Blocks")]
    public BlockItemSO DirtBlock;
    public BlockItemSO StoneBlock;
    public BlockItemSO SandBlock;
    public BlockItemSO ClayBlock;
    public BlockItemSO IronOreBlock;
    public BlockItemSO CopperOreBlock;
    public BlockItemSO Log;
    public BlockItemSO Rock;
    #endregion

    public ItemSO GetItemByID(ItemsID id)
    {
        return items.Find(item => item.Id == id);
    }

    public void LoadResources()
    {
        items = new List<ItemSO>()
        {
            DirtBlock,
            StoneBlock,
            SandBlock,
            ClayBlock,
            IronOreBlock,
            CopperOreBlock,
            Log,
            Rock,
        };
    }
}
