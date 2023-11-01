using Inventory;
using System.Text;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "newBlock", menuName = "Items/Block")]
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
            MaxStackSize = 999;
            Using = "Can be placed";
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
