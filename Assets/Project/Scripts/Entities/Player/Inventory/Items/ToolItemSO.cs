using SavageWorld.Runtime.Enums.Types;

namespace SavageWorld.Runtime.Entities.Player.Inventory.Items
{
    public abstract class ToolItemSO : NonStackableItemSO
    {
        #region Fields
        protected ToolTypes _toolType;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
        public ToolItemSO()
        {
            _itemType = ItemTypes.Tool;
            _using = "Can be used";
        }
        #endregion

        #region Private Methods

        #endregion
    }
}