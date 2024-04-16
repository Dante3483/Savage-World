using Items;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Player/CraftStation/Recipe")]
public class RecipeSO : ScriptableObject
{
    #region Private fields
    [SerializeField] private ItemQuantity[] _materials;
    [SerializeField] private ItemQuantity _result;
    [SerializeField] private bool _isUnlocked;
    #endregion

    #region Public fields
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
    #endregion

    #region Methods
    public void SelectRecipe()
    {
        SelectedRecipe = this;
    }
    #endregion
}
