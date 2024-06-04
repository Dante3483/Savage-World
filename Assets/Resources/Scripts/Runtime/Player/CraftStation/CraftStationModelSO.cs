using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftStation", menuName = "Player/CraftStation/CraftStation")]
public class CraftStationModelSO : ModelBaseSO
{
    #region Fields
    [SerializeField]
    private string _name;
    [SerializeField]
    private List<RecipesSet> _listOfSets;
    [SerializeField]
    private int _indexOfSelectedSet;
    [SerializeField]
    private int _indexOfSelectedRecipe;
    private List<RecipeSO> _listOfRecipes;
    private RecipeSO _selectedRecipe;
    private static CraftStationModelSO _selectedCraftStation;
    #endregion

    #region Properties
    public string Name
    {
        get
        {
            return _name;
        }
    }

    public List<RecipesSet> ListOfSets
    {
        get
        {
            return _listOfSets;
        }
    }

    public RecipeSO SelectedRecipe
    {
        get
        {
            return _selectedRecipe;
        }

        set
        {
            _selectedRecipe = value;
        }
    }
    #endregion

    #region Events / Delegates
    public static event Action<CraftStationModelSO> CraftStationSelected;
    public static event Action<List<RecipeSO>> SetSelected;
    public static event Action<RecipeSO> RecipeSelected;
    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public override void Initialize()
    {

    }

    public void ResetSelection()
    {
        _indexOfSelectedSet = -1;
        _indexOfSelectedRecipe = -1;
    }

    public void SelectCraftStation()
    {
        if (_selectedCraftStation != this)
        {
            _selectedCraftStation = this;
            _selectedCraftStation.ResetSelection();
            CraftStationSelected?.Invoke(_selectedCraftStation);
        }
    }

    public void SelectSet(int index)
    {
        if (_indexOfSelectedSet != index)
        {
            _indexOfSelectedSet = index;
            _listOfRecipes = _listOfSets[index].Recipes;
            SetSelected?.Invoke(_listOfRecipes);
        }
    }

    public void SelectRecipe(int index)
    {
        if (_indexOfSelectedRecipe != index)
        {
            _indexOfSelectedRecipe = index;
            _selectedRecipe = _listOfRecipes[index];
            RecipeSelected?.Invoke(_selectedRecipe);
        }
    }

    public RecipeSO GetRecipe(int index)
    {
        return _listOfRecipes[index];
    }

    public List<RecipeSO> GetRecipesByFilter(string filter)
    {
        return _listOfSets[_indexOfSelectedSet].Recipes.FindAll(r => r.Result.Item.Name.ToLower().Contains(filter));
    }
    #endregion

    #region Private Methods

    #endregion
}