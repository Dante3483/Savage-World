using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICraftStationPage : MonoBehaviour
{
    #region Private fields
    [Header("Left page")]
    [SerializeField] private UIRecipe _recipePrefab;
    [SerializeField] private UICraftStationBookmark _bookmarkPrefab;
    [SerializeField] private UISearchRecipes _searchRecipesInput;
    [SerializeField] private TMP_Text _craftStationNameTxt;
    [SerializeField] private RectTransform _recipesContent;
    [SerializeField] private RectTransform _bookmarksContent;

    [Header("Right page")]
    [SerializeField] private UIRecipeMaterial _recipeMaterialPrefab;
    [SerializeField] private UIQuantityInput _quantityInput;
    [SerializeField] private Image _bigImage;
    [SerializeField] private TMP_Text _rarityTxt;
    [SerializeField] private TMP_Text _descriptionTxt;
    [SerializeField] private Button _createItemButton;
    [SerializeField] private RectTransform _recipeMaterialsContent;

    private List<UICraftStationBookmark> _listOfBookmarks;
    private List<UIRecipe> _listOfRecipes;
    private List<UIRecipeMaterial> _listOfRecipeMaterials;
    #endregion

    #region Public fields
    public Action<int> OnRecipeSelected, OnBookmarkSelected, OnItemCreate;
    public Action<string> OnSearchRecipes;
    #endregion

    #region Properties

    #endregion

    #region Methods
    private void Awake()
    {
        _listOfRecipes = new List<UIRecipe>();
    }

    public void InitializePage(int bookmarksCount, int recipeMaterialsCount)
    {
        InitializeBookmarks(bookmarksCount);
        InitializeRecipeMaterials(recipeMaterialsCount);
        _searchRecipesInput.OnNeedSearch += HandleSearchRecipes;
    }

    private void InitializeBookmarks(int bookmarksCount)
    {
        _listOfBookmarks = new List<UICraftStationBookmark>();
        for (int i = 0; i < bookmarksCount; i++)
        {
            UICraftStationBookmark uiItem = Instantiate(_bookmarkPrefab, Vector3.zero, Quaternion.identity);
            uiItem.transform.SetParent(_bookmarksContent, false);
            uiItem.name = "Bookmark";
            uiItem.OnLeftButtonClick += HandleSelectBookmark;
            _listOfBookmarks.Add(uiItem);
        }
    }

    private void InitializeRecipeMaterials(int recipeMaterialsCount)
    {
        _listOfRecipeMaterials = new List<UIRecipeMaterial>();
        for (int i = 0; i < recipeMaterialsCount; i++)
        {
            UIRecipeMaterial uiItem = Instantiate(_recipeMaterialPrefab, Vector3.zero, Quaternion.identity);
            uiItem.transform.SetParent(_recipeMaterialsContent, false);
            uiItem.name = "RecipeMaterial";
            _listOfRecipeMaterials.Add(uiItem);
        }
    }

    public void UpdateRecipe(Sprite sprite, string name)
    {
        UIRecipe uiItem = Instantiate(_recipePrefab, Vector3.zero, Quaternion.identity);
        uiItem.transform.SetParent(_recipesContent, false);
        uiItem.name = "Recipe";
        uiItem.OnLeftButtonClick += HandleSelectRecipe;
        uiItem.SetData(sprite, name);
        _listOfRecipes.Add(uiItem);
    }

    public void UpdateRecipeMaterial(int index, Sprite sprite, string name, int quantity)
    {
        _listOfRecipeMaterials[index].SetData(sprite, name, quantity);
    }

    public void UpdateBigImage(Sprite sprite)
    {
        _bigImage.sprite = sprite;
    }

    public void UpdateQuantityInput(int quantity)
    {
        _createItemButton.interactable = quantity > 0;
        _quantityInput.UpdateMaxQuantity(quantity);
    }

    public void UpdateRarityText(string name, Color color)
    {
        _rarityTxt.text = name;
        _rarityTxt.color = color;
    }

    public void UpdateDescriptionText(string description)
    {
        _descriptionTxt.text = description;
    }

    public void UpdateCraftStationName(string name)
    {
        _craftStationNameTxt.text = name;
    }

    public void UpdateBookmark(int index, Sprite sprite)
    {
        _listOfBookmarks[index].SetData(sprite);
    }

    public void ResetAllRecipes()
    {
        foreach (UIRecipe uiItem in _listOfRecipes)
        {
            Destroy(uiItem.gameObject);
        }
        _listOfRecipes.Clear();
    }

    public void ResetAllMaterials()
    {
        foreach (UIRecipeMaterial uiItem in _listOfRecipeMaterials)
        {
            uiItem.ResetData();
        }
    }

    public void ResetAllBookmarks()
    {
        foreach (UICraftStationBookmark uiItem in _listOfBookmarks)
        {
            uiItem.ResetData();
        }
    }

    public void ResetSearchInput()
    {
        _searchRecipesInput.ResetData();
    }

    public void SelectRecipe(int index)
    {
        if (_listOfRecipes.Count == 0)
        {
            return;
        }
        DeselectAllRecipes();
        _listOfRecipes[index].Select();
        OnRecipeSelected?.Invoke(index);
    }

    public void SelectBookmark(int index)
    {
        DeselectAllBookmarks();
        _listOfBookmarks[index].Select();
        OnBookmarkSelected?.Invoke(index);
    }

    private void DeselectAllRecipes()
    {
        foreach (UIRecipe uiItem in _listOfRecipes)
        {
            uiItem.Deselect();
        }
    }

    private void DeselectAllBookmarks()
    {
        foreach (UICraftStationBookmark uiItem in _listOfBookmarks)
        {
            uiItem.Deselect();
        }
    }

    private void HandleSelectRecipe(UIRecipe recipe)
    {
        int index = _listOfRecipes.IndexOf(recipe);
        SelectRecipe(index);
    }

    private void HandleSelectBookmark(UICraftStationBookmark bookmark)
    {
        int index = _listOfBookmarks.IndexOf(bookmark);
        SelectBookmark(index);
    }

    public void HandleSearchRecipes(string name)
    {
        OnSearchRecipes?.Invoke(name);
    }

    public void HandleCreateItem()
    {
        OnItemCreate?.Invoke(_quantityInput.Quantity);
    }
    #endregion
}
