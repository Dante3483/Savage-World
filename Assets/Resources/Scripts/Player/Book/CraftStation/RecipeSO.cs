using Items;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Player/CraftStation/Recipe")]
public class RecipeSO : ScriptableObject
{
    #region Private fields
    [SerializeField] private RecipeItem[] _meterials;
    [SerializeField] private RecipeItem _result;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public RecipeItem[] Meterials
    {
        get
        {
            return _meterials;
        }
    }

    public RecipeItem Result
    {
        get
        {
            return _result;
        }
    }

    public static RecipeSO SelectedRecipe;
    #endregion

    #region Methods
    public void SelectRecipe()
    {
        SelectedRecipe = this;
    }
    #endregion
}
