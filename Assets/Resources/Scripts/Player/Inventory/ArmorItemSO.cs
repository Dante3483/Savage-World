using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "newArmor", menuName = "Items/Armor")]
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

        public override string GetDescription()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("<size=30>").Append(ColoredName).Append("</size>").AppendLine();
            builder.Append(ItemRarity.Name).AppendLine();
            builder.Append(Using).AppendLine();
            builder.Append(Description).AppendLine();

            return builder.ToString();
        }
        #endregion
    }
}