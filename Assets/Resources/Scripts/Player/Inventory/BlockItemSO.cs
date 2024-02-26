using System.Text;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "Block", menuName = "Items/Block")]
    public class BlockItemSO : ItemSO
    {
        #region Private fields
        [SerializeField] private BlockSO _blockToPlace;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public BlockSO BlockToPlace
        {
            get
            {
                return _blockToPlace;
            }

            set
            {
                _blockToPlace = value;
            }
        }
        #endregion

        #region Methods
        public BlockItemSO()
        {
            ItemType = ItemTypes.Block;
            IsStackable = true;
            MaxStackSize = 9999;
            Using = "Can be placed";
        }

        public override StringBuilder GetFullDescription(int quantity)
        {
            _fullDescriptionStringBuilder.Clear();
            _fullDescriptionStringBuilder.Append("<size=25>").Append(ColoredName).Append($" ({quantity})").Append("</size>").AppendLine();
            _fullDescriptionStringBuilder.Append(ItemRarity.Name).AppendLine();
            _fullDescriptionStringBuilder.Append(Using).AppendLine();
            _fullDescriptionStringBuilder.Append(Description).AppendLine();
            return _fullDescriptionStringBuilder;
        }
        #endregion
    }
}
