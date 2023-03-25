using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "newItem", menuName = "UI/Inventory/Armor")]
public class ArmorItemSO : ItemSO
{
    [SerializeField] private ArmorType _armorType;
    [SerializeField] private Sprite _playerView;

    public ArmorType ArmorType
    {
        get
        {
            return _armorType;
        }

        set
        {
            _armorType = value;
        }
    }

    public Sprite PlayerView
    {
        get
        {
            return _playerView;
        }

        set
        {
            _playerView = value;
        }
    }

    public ArmorItemSO()
    {
        ItemType = ItemType.Armor;
        IsStackable = false;
        MaxStackSize = 1;
        Using = "Can be equipped";
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
