using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "Armor", menuName = "Items/Armor")]
    public class ArmorItemSO : NonStackableItemSO
    {
        #region Fields
        [SerializeField]
        private ArmorTypes _armorType;
        [SerializeField]
        private Sprite _playerView;
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

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
        public ArmorItemSO()
        {
            _itemType = ItemTypes.Armor;
            _using = "Can be equipped";
        }
        #endregion

        #region Private Methods

        #endregion
    }
}