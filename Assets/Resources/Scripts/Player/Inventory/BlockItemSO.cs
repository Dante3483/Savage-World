using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "newItem", menuName = "UI/Inventory/Block")]
public class BlockItemSO : ItemSO
{
    [SerializeField] private BlockSO _blockToPlace;

    public BlockItemSO()
    {
        ItemType = ItemType.Block;
        IsStackable = true;
        MaxStackSize = 999;
        Using = "Can be placed";
    }

    public BlockSO BlockToPlace
    {
        get
        {
            return _blockToPlace;
        }

        set
        {
            Debug.Log("asasasd");
            _blockToPlace = value;
        }
    }

    public override string GetDescription()
    {
        StringBuilder builder = new StringBuilder();

        builder.Append("<size=30>").Append(ColoredName).Append("</size>").AppendLine();
        builder.Append(ItemRarity.Name).AppendLine();
        builder.Append(Using).AppendLine();
        builder.Append(Description).AppendLine();

        return builder.ToString();
    }
}
