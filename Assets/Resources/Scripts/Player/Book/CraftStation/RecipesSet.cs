using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class RecipesSet
{
    #region Private fields
    [SerializeField] private string _setName;
    [SerializeField] private Sprite _setSprite;
    [SerializeField] private List<RecipeSO> _recipes;
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

    public Sprite SetSprite
    {
        get
        {
            return _setSprite;
        }
    }
    #endregion

    #region Methods

    #endregion
}
