using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipes", menuName = "Player/CraftStation/CraftStation")]
public class CraftStationSO : ScriptableObject
{
    #region Private fields
    [SerializeField] List<RecipeSO> _recipes;
    #endregion

    #region Public fields
    public static CraftStationSO CurrentCraftStation;
    #endregion

    #region Properties
    public List<RecipeSO> Recipes
    {
        get
        {
            return _recipes;
        }
    }
    #endregion

    #region Methods
    public void SelectCraftStation()
    {
        CurrentCraftStation = this;
    }
    #endregion
}
