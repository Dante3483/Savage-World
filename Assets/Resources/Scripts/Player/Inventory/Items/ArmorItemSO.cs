using System.Text;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "Armor", menuName = "Items/Armor")]
    public class ArmorItemSO : ItemSO
    {
        #region Private fields
        [SerializeField] private ArmorTypes _armorType;
        [SerializeField] private Sprite _playerView;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public ArmorTypes ArmorType
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
        #endregion

        #region Methods
        public ArmorItemSO()
        {
            ItemType = ItemTypes.Armor;
            IsStackable = false;
            MaxStackSize = 1;
            Using = "Can be equipped";
        }

        public override StringBuilder GetFullDescription(int quantity)
        {
            _fullDescriptionStringBuilder.Clear();
            _fullDescriptionStringBuilder.Append("<size=25>").Append(ColoredName).Append("</size>").AppendLine();
            _fullDescriptionStringBuilder.Append(ItemRarity.Name).AppendLine();
            _fullDescriptionStringBuilder.Append(Using).AppendLine();
            _fullDescriptionStringBuilder.Append(Description).AppendLine();
            return _fullDescriptionStringBuilder;
        }
        #endregion
    }
}