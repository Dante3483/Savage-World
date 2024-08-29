using SavageWorld.Runtime.Entities.Player.Book;
using UnityEngine;

namespace SavageWorld.Runtime.Entities.Player.CraftStation
{
    [CreateAssetMenu(fileName = "Recipe", menuName = "Player/CraftStation/Recipe")]
    public class RecipeSO : ScriptableObject
    {
        #region Fields
        [SerializeField]
        private ItemQuantity[] _materials;
        [SerializeField]
        private ItemQuantity _result;
        [SerializeField]
        private bool _isUnlocked;
        [SerializeField]
        private bool _isUnlockedByDefault;

        public static RecipeSO SelectedRecipe;
        #endregion

        #region Properties
        public ItemQuantity[] Materials
        {
            get
            {
                return _materials;
            }
        }

        public ItemQuantity Result
        {
            get
            {
                return _result;
            }
        }

        public bool IsUnlocked { get => _isUnlocked; set => _isUnlocked = value; }

        public bool IsUnlockedByDefault
        {
            get
            {
                return _isUnlockedByDefault;
            }
        }
        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
        public void SelectRecipe()
        {
            SelectedRecipe = this;
        }
        #endregion

        #region Private Methods

        #endregion
    }
}