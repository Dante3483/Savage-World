using SavageWorld.Runtime.Managers;
using SavageWorld.Runtime.MVP;
using SavageWorld.Runtime.Player.CraftStation.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SavageWorld.Runtime.Player.CraftStation
{
    public class CraftStationView : ViewBase
    {
        #region Fields
        [Header("Left page")]
        [SerializeField]
        private UIRecipe _recipePrefab;
        [SerializeField]
        private UICraftStationBookmark _bookmarkPrefab;
        [SerializeField]
        private UISearchRecipes _searchRecipesInput;
        [SerializeField]
        private TMP_Text _craftStationNameTxt;
        [SerializeField]
        private RectTransform _recipesContent;
        [SerializeField]
        private RectTransform _bookmarksContent;
        [SerializeField]
        private RectTransform _freeObjectsContent;

        [Header("Right page")]
        [SerializeField]
        private UIRecipeMaterial _materialPrefab;
        [SerializeField]
        private UIQuantityInput _quantityInput;
        [SerializeField]
        private Image _bigImage;
        [SerializeField]
        private TMP_Text _rarityTxt;
        [SerializeField]
        private TMP_Text _descriptionTxt;
        [SerializeField]
        private Button _createItemButton;
        [SerializeField]
        private RectTransform _materialsContent;

        private List<UICraftStationBookmark> _listOfBookmarks;
        private List<UICraftStationBookmark> _listOfFreeBookmarks;
        private List<UIRecipe> _listOfRecipes;
        private List<UIRecipe> _listOfFreeRecipes;
        private List<UIRecipeMaterial> _listOfMaterials;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates
        public Action<int> SelectSetRequested;
        public Action<int> SelectRecipeRequested;
        public Action<int> CreateItemRequested;
        public Action<string> SearchingRecipes;
        #endregion

        #region Monobehaviour Methods

        #endregion

        #region Public Methods
        public override void Initialize()
        {
            _listOfBookmarks = new();
            _listOfFreeBookmarks = new();
            _listOfRecipes = new();
            _listOfFreeRecipes = new();

            InitializeMaterials();
            _quantityInput.Initialize();

            _searchRecipesInput.OnNeedSearch += OnSearchingRecipes;
            _createItemButton.onClick.AddListener(OnCreateItemRequested);
        }

        public override void Show()
        {
            UIManager.Instance.CraftStationUI.IsActive = true;
        }

        public override void Hide()
        {
            UIManager.Instance.CraftStationUI.IsActive = false;
        }

        public void UpdateName(string name)
        {
            _craftStationNameTxt.text = name;
        }

        public void UpdateBigImage(Sprite sprite)
        {
            _bigImage.sprite = sprite;
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

        public void UpdateMaterial(int index, Sprite icon, string name, int quantity)
        {
            _listOfMaterials[index].SetData(icon, name, quantity);
        }

        public void UpdateQuantityInput(int quantity)
        {
            _createItemButton.interactable = quantity > 0;
            _quantityInput.UpdateMaxQuantity(quantity);
        }

        public void AddBookmark(Sprite icon)
        {
            UICraftStationBookmark bookmark = GetObject(_listOfFreeBookmarks);
            if (bookmark is null)
            {
                bookmark = CreateBookmark();
            }
            bookmark.SetData(icon);
            bookmark.transform.SetParent(_bookmarksContent);
            _listOfBookmarks.Add(bookmark);
        }

        public void RemoveBookmarks()
        {
            Releaseobjects(_listOfBookmarks, _listOfFreeBookmarks);
        }

        public void SelectBookmark(int index)
        {
            DeselectAllBookmarks();
            _listOfBookmarks[index].Select();
            SelectSetRequested?.Invoke(index);
        }

        public void AddRecipe(Sprite icon, string name)
        {
            UIRecipe recipe = GetObject(_listOfFreeRecipes);
            if (recipe is null)
            {
                recipe = CreateRecipe();
            }
            recipe.SetData(icon, name);
            recipe.transform.SetParent(_recipesContent);
            _listOfRecipes.Add(recipe);
        }

        public void RemoveRecipes()
        {
            Releaseobjects(_listOfRecipes, _listOfFreeRecipes);
        }

        public void SelectRecipe(int index)
        {
            if (_listOfRecipes.Count == 0)
            {
                return;
            }
            DeselectAllRecipes();
            _listOfRecipes[index].Select();
            SelectRecipeRequested?.Invoke(index);
        }

        public void ResetSearching()
        {
            _searchRecipesInput.ResetData();
        }

        public void ResetMaterials()
        {
            foreach (UIRecipeMaterial material in _listOfMaterials)
            {
                material.ResetData();
            }
        }
        #endregion

        #region Private Methods
        //HARDCODE
        private void InitializeMaterials()
        {
            _listOfMaterials = new List<UIRecipeMaterial>();
            for (int i = 0; i < 6; i++)
            {
                UIRecipeMaterial material = Instantiate(_materialPrefab, Vector3.zero, Quaternion.identity);
                material.transform.SetParent(_materialsContent, false);
                material.name = "Material";
                _listOfMaterials.Add(material);
            }
        }

        private void DeselectAllBookmarks()
        {
            foreach (UICraftStationBookmark bookmark in _listOfBookmarks)
            {
                bookmark.Deselect();
            }
        }

        private void DeselectAllRecipes()
        {
            foreach (UIRecipe recipe in _listOfRecipes)
            {
                recipe.Deselect();
            }
        }

        private void Releaseobjects<T>(List<T> from, List<T> to) where T : MonoBehaviour
        {
            foreach (T obj in from)
            {
                obj.transform.SetParent(_freeObjectsContent);
                to.Add(obj);
            }
            from.Clear();
        }

        private T GetObject<T>(List<T> from) where T : MonoBehaviour
        {
            T obj = from.FirstOrDefault();
            if (obj != null)
            {
                from.Remove(obj);
            }
            return obj;
        }

        private UICraftStationBookmark CreateBookmark()
        {
            UICraftStationBookmark bookmark = Instantiate(_bookmarkPrefab, Vector3.zero, Quaternion.identity);
            bookmark.transform.SetParent(_bookmarksContent, false);
            bookmark.name = "Bookmark";
            bookmark.OnLeftButtonClick += BookmarkLeftButtonClickedEventHandler;
            return bookmark;
        }

        private UIRecipe CreateRecipe()
        {
            UIRecipe recipe = Instantiate(_recipePrefab, Vector3.zero, Quaternion.identity);
            recipe.transform.SetParent(_recipesContent, false);
            recipe.name = "Recipe";
            recipe.OnLeftButtonClick += RecipeLeftButtonClickedEventHandler;
            return recipe;
        }

        private void BookmarkLeftButtonClickedEventHandler(UICraftStationBookmark bookmark)
        {
            int index = _listOfBookmarks.IndexOf(bookmark);
            SelectBookmark(index);
        }

        private void RecipeLeftButtonClickedEventHandler(UIRecipe recipe)
        {
            int index = _listOfRecipes.IndexOf(recipe);
            SelectRecipe(index);
        }

        private void OnSearchingRecipes(string filter)
        {
            SearchingRecipes?.Invoke(filter);
        }

        private void OnCreateItemRequested()
        {
            CreateItemRequested?.Invoke(_quantityInput.Quantity);
        }
        #endregion
    }
}