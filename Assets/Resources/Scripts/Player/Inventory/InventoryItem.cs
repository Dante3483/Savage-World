using Items;

namespace Inventory
{
    public struct InventoryItem
    {
        #region Private fields

        #endregion

        #region Public fields
        public int Quantity;
        public ItemSO Item;
        public bool IsEmpty => Item == null;
        #endregion

        #region Properties

        #endregion

        #region Methods
        public InventoryItem ChangeQuantity(int newQuantity)
        {
            return new InventoryItem()
            {
                Item = this.Item,
                Quantity = newQuantity,
            };
        }

        public static InventoryItem GetEmptyItem()
        {
            return new InventoryItem()
            {
                Item = null,
                Quantity = 0,
            };
        }
        #endregion
    }
}
