using System.Text;
using UnityEngine;

namespace Items
{
    public abstract class ItemSO : ScriptableObject
    {
        #region Private fields
        [SerializeField] private ItemsID _id;
        [SerializeField] private ItemTypes _itemType;
        [SerializeField] private ItemRaritySO _itemRarity;
        [SerializeField] private bool _isStackable;
        [SerializeField] private int _maxStackSize = 1;
        [SerializeField] private string _name;
        [SerializeField] private string _using;
        [SerializeField][TextArea] private string _description;
        [SerializeField] private Sprite _smallItemImage;
        [SerializeField] private Sprite _bigItemImage;
        protected StringBuilder _fullDescriptionStringBuilder;
        #endregion

        #region Public fields

        #endregion

        #region Properties
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
                return _maxStackSize;
            }

            set
            {
                _maxStackSize = value;
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

        public string ColoredName
        {
            get
            {
                return $"<color=#{ColorUtility.ToHtmlStringRGB(ItemRarity.RarityColor)}>{_name}</color>";
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

        #region Methods
        public ItemSO()
        {
            _fullDescriptionStringBuilder = new StringBuilder();
        }

        public abstract StringBuilder GetFullDescription(int quantity);
        #endregion
    }
}
