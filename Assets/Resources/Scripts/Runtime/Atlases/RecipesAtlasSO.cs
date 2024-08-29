using SavageWorld.Runtime.Entities.Player.CraftStation;
using UnityEngine;

namespace SavageWorld.Runtime.Atlases
{
    [CreateAssetMenu(fileName = "RecipesAtlas", menuName = "Atlases/RecipesAtlas")]
    public class RecipesAtlasSO : AtlasSO
    {
        #region Fields
        [SerializeField]
        private RecipeSO[] _recipes;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
        public override void InitializeAtlas()
        {
            ResetRecipes();
        }

        public void ResetRecipes()
        {
            foreach (RecipeSO recipe in _recipes)
            {
                recipe.IsUnlocked = recipe.IsUnlockedByDefault;
            }
        }
        #endregion

        #region Private Methods

        #endregion
    }
}