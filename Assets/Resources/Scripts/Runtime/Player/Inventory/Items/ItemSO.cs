using System.Text;
using UnityEngine;

namespace Items
{
    public abstract class ItemSO : ScriptableObject
    {
        #region Fields
        [SerializeField]
        private ItemsID _id;
        protected ItemTypes _itemType;
        [SerializeField]
        protected ItemRaritySO _itemRarity;
        protected bool _isStackable;
        [SerializeField]
        protected int _stackSize;
        [SerializeField]
        private string _name;
        [SerializeField]
        protected string _using;
        [SerializeField]
        [TextArea]
        protected string _description;
        [SerializeField]
        private Sprite _smallItemImage;
        [SerializeField]
        private Sprite _bigItemImage;
        protected StringBuilder _fullDescriptionStringBuilder;
        #endregion

        #region Properties
        public string ColoredName => $"<color=#{ColorUtility.ToHtmlStringRGB(_itemRarity.RarityColor)}>{_name}</color>";

        public ItemTypes ItemType
        {
            get
            {
                return _itemType;
            }

            set
            {
                _itemType = value;
            }
        }

        public bool IsStackable
        {
            get
            {
                return _isStackable;
            }

            set
            {
                _isStackable = value;
            }
        }

        public int MaxStackSize
        {
            get
            {
                return _stackSize;
            }

            set
            {
                _stackSize = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                _description = value;
            }
        }

        public Sprite SmallItemImage
        {
            get
            {
                return _smallItemImage;
            }

            set
            {
                _smallItemImage = value;
            }
        }

        public ItemRaritySO ItemRarity
        {
            get
            {
                return _itemRarity;
            }

            set
            {
                _itemRarity = value;
            }
        }

        public string Using
        {
            get
            {
                return _using;
            }

            set
            {
                _using = value;
            }
        }

        public ItemsID Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public Sprite BigItemImage
        {
            get
            {
                return _bigItemImage;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
        public ItemSO()
        {
            _fullDescriptionStringBuilder = new StringBuilder();
        }

        public abstract StringBuilder GetFullDescription(int quantity);
        #endregion

        #region Private Methods

        #endregion
    }
}
