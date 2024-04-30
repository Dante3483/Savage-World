using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipes", menuName = "Player/CraftStation/CraftStation")]
public class CraftStationSO : ScriptableObject
{
    #region Private fields
    [SerializeField] private string _name;
    [SerializeField] private List<RecipesSet> _recipesSets;
    [SerializeField] private int _indexOfCurrentSet;

    private List<RecipeSO> _currentRecipes;
    #endregion

    #region Public fields
    public static CraftStationSO CurrentCraftStation;
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

    public List<RecipesSet> RecipesSets
    {
        get
        {
            return _recipesSets;
        }
    }
    #endregion

    #region Methods
    public void SelectCraftStation()
    {
        CurrentCraftStation = this;
    }

    public void ChangeSet(int index)
    {
        _indexOfCurrentSet = index;
    }

    public RecipeSO GetRecipe(int index)
    {
        return _currentRecipes[index];
    }

    public List<RecipeSO> GetRecipes()
    {
        _currentRecipes = _recipesSets[_indexOfCurrentSet].Recipes;
        return _currentRecipes;
    }

    public List<RecipeSO> GetRecipes(string name)
    {
        _currentRecipes = _recipesSets[_indexOfCurrentSet].Recipes.FindAll(r => r.Result.Item.Name.ToLower().Contains(name));
        return _currentRecipes;
    }
    #endregion
}
