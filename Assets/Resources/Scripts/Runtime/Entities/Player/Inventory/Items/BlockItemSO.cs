using SavageWorld.Runtime.Enums.Types;
using SavageWorld.Runtime.Terrain.Tiles;
using UnityEngine;

namespace SavageWorld.Runtime.Entities.Player.Inventory.Items
{
    [CreateAssetMenu(fileName = "Block", menuName = "Items/Block")]
    public class BlockItemSO : StackableItemSO
    {
        #region Fields
        [SerializeField]
        private TileBaseSO _blockToPlace;
        #endregion

        #region Properties
        public TileBaseSO BlockToPlace
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
