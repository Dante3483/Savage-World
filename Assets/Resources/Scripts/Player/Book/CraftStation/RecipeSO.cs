using Items;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Player/CraftStation/Recipe")]
public class RecipeSO : ScriptableObject
{
    #region Private fields
    [SerializeField] private RecipeItem[] _materials;
    [SerializeField] private RecipeItem _result;
    [SerializeField] private bool _isEnoughMaterials;
    #endregion

    #region Public fields
    public static RecipeSO SelectedRecipe;
    #endregion

    #region Properties
    public RecipeItem[] Materials
    {
        get
        {
            return _materials;
        }
    }

    public RecipeItem Result
    {
        get
        {
            return _result;
        }
    }

    public bool IsEnoughMaterials
    {
        get
        {
            return _isEnoughMaterials;
        }

        set
        {
            _isEnoughMaterials = value;
        }
    }
    #endregion

    #region Methods
    public void SelectRecipe()
    {
        SelectedRecipe = this;
    }
    #endregion
}
