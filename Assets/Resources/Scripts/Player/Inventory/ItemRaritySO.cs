using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "newRarity", menuName = "Items/Rarity")]
    public class ItemRaritySO : ScriptableObject
    {
        #region Private fields
        [SerializeField] private string _name;
        [SerializeField] private Color _rarityColor;
        #endregion

        #region Public fields

        #endregion

        #region Properties
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

        public Color RarityColor
        {
            get
            {
                return _rarityColor;
            }

            set
            {
                _rarityColor = value;
            }
        }
        #endregion

        #region Methods

        #endregion
    }
}
