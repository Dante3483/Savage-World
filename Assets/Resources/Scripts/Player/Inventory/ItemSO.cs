using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private Sprite _itemImage;
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
                string hexColor = ColorUtility.ToHtmlStringRGB(ItemRarity.RarityColor);
                return $"<color=#{hexColor}>{_name}</color>";
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

        public Sprite ItemImage
        {
            get
            {
                return _itemImage;
            }

            set
            {
                _itemImage = value;
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
        #endregion

        #region Methods
        public abstract string GetDescription();
        #endregion
    }
}
