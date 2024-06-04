using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RecipesSet
{
    #region Private fields
    [SerializeField]
    private string _name;
    [SerializeField]
    private Sprite _icon;
    [SerializeField]
    private List<RecipeSO> _recipes;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public List<RecipeSO> Recipes
    {
        get
        {
            return _recipes;
        }
    }

    public Sprite Icon
    {
        get
        {
            return _icon;
        }
    }
    #endregion

    #region Methods

    #endregion
}
