using Items;
using UnityEngine;

public class CraftStationController : MonoBehaviour, IBookPageController
{
    #region Private fields
    [Header("Main")]
    [SerializeField]
    private Player _player;
    [SerializeField]
    private UICraftStationPage _craftStationUI;
    [SerializeField]
    private CraftStationSO _currentCraftStation;
    [SerializeField]
    private RecipeSO _currentRecipe;
    [SerializeField]
    private Inventory _inventoryData;
    [SerializeField]
    private int _bookmarksCount;
    [SerializeField]
    private int _recipeMaterialsCount;
    [SerializeField]
    private bool _isQuantityForRecipeRepeating;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public bool IsActive => UIManager.Instance.CraftStationUI.IsActive;
    #endregion

    #region Methods
    private void Awake()
    {
        if (_player is null)
        {
            _player = GetComponentInParent<Player>();
        }
        PrepareData();
        PrepareUI();
    }

    public void PrepareData()
    {
        _inventoryData = _player.Inventory;
        _inventoryData.OnItemsUpdate += HandleUpdateMaxRecipeQuantity;
    }

    public void PrepareUI()
    {
        _craftStationUI = UIManager.Instance.CraftStationUI.Content.GetComponentInChildren<UICraftStationPage>();
        _craftStationUI.InitializePage(_bookmarksCount, _recipeMaterialsCount);
        _craftStationUI.OnBookmarkSelected += HandleUpdateSet;
        _craftStationUI.OnRecipeSelected += HandleUpdateRecipeInfo;
        _craftStationUI.OnItemCreate += HandleCreateItem;
        _craftStationUI.OnSearchRecipes += UpdateRecipes;
    }

    public void ResetData()
    {
        UIManager.Instance.CraftStationUI.ReverseActivity();
        if (UIManager.Instance.CraftStationUI.IsActive)
        {
            UpdateCraftStation();
        }
    }

    public void UpdateCraftStation()
    {
        _currentCraftStation = CraftStationSO.CurrentCraftStation;
        UpdateLeftPage();
    }

    private void UpdateRecipes()
    {
        _craftStationUI.ResetSearchInput();
        _craftStationUI.ResetAllRecipes();
        foreach (RecipeSO recipe in _currentCraftStation.GetRecipes())
        {
            if (recipe.IsUnlocked)
            {
                _craftStationUI.UpdateRecipe(recipe.Result.Item.SmallItemImage, recipe.Result.Item.Name);
            }
        }
        _craftStationUI.SelectRecipe(0);
    }

    private void UpdateRecipes(string name)
    {
        _craftStationUI.ResetAllRecipes();
        foreach (RecipeSO recipe in _currentCraftStation.GetRecipes(name))
        {
            _craftStationUI.UpdateRecipe(recipe.Result.Item.SmallItemImage, recipe.Result.Item.Name);
        }
        _craftStationUI.SelectRecipe(0);
    }

    private void UpdateRecipeMaterials()
    {
        int i = 0;
        _craftStationUI.UpdateBigImage(_currentRecipe.Result.Item.BigItemImage);
        foreach (ItemQuantity material in _currentRecipe.Materials)
        {
            _craftStationUI.UpdateRecipeMaterial(i++, material.Item.SmallItemImage, material.Item.Name, material.Quantity);
        }
    }

    private void UpdateBookmarks()
    {
        _craftStationUI.ResetAllBookmarks();
        int i = 0;
        foreach (RecipesSet recipesSet in _currentCraftStation.RecipesSets)
        {
            _craftStationUI.UpdateBookmark(i++, recipesSet.SetSprite);
        }
        _craftStationUI.SelectBookmark(0);
    }

    private void UpdateMaxRecipeQuantity()
    {
        int itemQuantity;
        int currentRecipeQuantity;
        int minRecipeQuantity = int.MaxValue;
        foreach (ItemQuantity material in _currentRecipe.Materials)
        {
            itemQuantity = _inventoryData.GetItemQuantity(material.Item);
            currentRecipeQuantity = Mathf.FloorToInt(itemQuantity / material.Quantity);
            minRecipeQuantity = Mathf.Min(currentRecipeQuantity, minRecipeQuantity);
        }
        _craftStationUI.UpdateQuantityInput(minRecipeQuantity);
    }

    private void UpdateLeftPage()
    {
        _craftStationUI.UpdateCraftStationName(_currentCraftStation.Name);
        UpdateBookmarks();
    }

    private void UpdateRightPage()
    {
        ItemSO resultItem = _currentRecipe.Result.Item;
        ItemRaritySO resultRarity = resultItem.ItemRarity;
        _craftStationUI.ResetAllMaterials();
        _craftStationUI.UpdateRarityText(resultRarity.Name, resultRarity.RarityColor);
        _craftStationUI.UpdateDescriptionText(resultItem.Description);
        UpdateRecipeMaterials();
        UpdateMaxRecipeQuantity();
    }

    private void HandleUpdateSet(int index)
    {
        _currentCraftStation.ChangeSet(index);
        UpdateRecipes();
    }

    private void HandleUpdateRecipeInfo(int index)
    {
        _currentRecipe = _currentCraftStation.GetRecipe(index);
        _currentRecipe.SelectRecipe();
        UpdateRightPage();
    }

    private void HandleUpdateMaxRecipeQuantity()
    {
        if (UIManager.Instance.CraftStationUI.IsActive)
        {
            UpdateMaxRecipeQuantity();
        }
    }

    private void HandleCreateItem(int quantity)
    {
        foreach (ItemQuantity material in _currentRecipe.Materials)
        {
            _inventoryData.RemoveItemFromFirstSlot(material.Item, material.Quantity * quantity);
        }
        _inventoryData.AddItem(_currentRecipe.Result.Item, _currentRecipe.Result.Quantity * quantity);
        UpdateMaxRecipeQuantity();
    }
    #endregion
}
