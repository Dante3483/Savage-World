using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "Block", menuName = "Items/Block")]
    public class BlockItemSO : StackableItemSO
    {
        #region Fields
        [SerializeField]
        private BlockSO _blockToPlace;
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

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
        public BlockItemSO()
        {
            _itemType = ItemTypes.Block;
            _using = "Can be placed";
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
