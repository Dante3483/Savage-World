using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public class AtlasEditorWindow : TwoPaneEditorWindow
{
    private enum SearchMode
    {
        Global = 0,
        Collections = 1,
        Collection = 2,
    }

    private class AtlasInfo
    {
        #region Private fields
        private AtlasSO _atlas;
        private List<CollectionInfo> _collections;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public AtlasSO Atlas
        {
            get
            {
                return _atlas;
            }
        }

        public List<CollectionInfo> Collections
        {
            get
            {
                return _collections;
            }
        }
        #endregion

        #region Methods
        public AtlasInfo(AtlasSO atlas)
        {
            _atlas = atlas;
            SetCollections();
        }

        /// <summary>
        /// Loads data about all collections in the atlas.
        /// </summary>
        private void SetCollections()
        {
            _collections = new List<CollectionInfo>();
            SerializedProperty iterator = new SerializedObject(_atlas).GetIterator();
            iterator.NextVisible(true);
            while (iterator.NextVisible(false))
            {
                if (iterator.isArray && iterator.propertyType == SerializedPropertyType.Generic)
                {
                    _collections.Add(new CollectionInfo(iterator.Copy()));
                }
            }
        }

        /// <summary>
        /// Iterates through all collections attempting to delete an object.
        /// </summary>
        /// <param name="value">The object to be deleted.</param>
        /// <returns>True if the object is successfully deleted, false if not found or not deleted.</returns>
        public bool Delete(ObjectInfo value)
        {
            foreach (CollectionInfo collection in _collections)
            {
                if (collection.Delete(_atlas, value))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }

    private class CollectionInfo
    {
        #region Private fields
        private SerializedProperty _property;
        private List<ObjectInfo> _objects;
        private Type _elementType;
        private bool _isScriptableObject;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public SerializedProperty Property
        {
            get
            {
                return _property;
            }
        }

        public Type ElementType
        {
            get
            {
                return _elementType;
            }
        }

        public bool IsScriptableObject
        {
            get
            {
                return _isScriptableObject;
            }
        }

        public List<ObjectInfo> Objects
        {
            get
            {
                return _objects;
            }
        }
        #endregion

        #region Methods
        public CollectionInfo(SerializedProperty property)
        {
            _property = property;
            SetObjects();
            SetElementType();
            _isScriptableObject = _elementType.IsSubclassOf(typeof(ScriptableObject));
        }

        /// <summary>
        /// Loads data about all objects in the collection.
        /// </summary>
        private void SetObjects()
        {
            _objects = new List<ObjectInfo>();
            for (int i = 0; i < _property.arraySize; i++)
            {
                Object collectionObject = _property.GetArrayElementAtIndex(i).objectReferenceValue;
                if (collectionObject == null)
                {
                    _property.DeleteArrayElementAtIndex(i--);
                    _property.serializedObject.ApplyModifiedProperties();
                    continue;
                }
                _objects.Add(new ObjectInfo(collectionObject, i));
            }
            SortObjectsByName();
        }

        /// <summary>
        /// Sets the type of the collection item.
        /// </summary>
        private void SetElementType()
        {
            Type parentType = _property.serializedObject.targetObject.GetType();
            BindingFlags fieldFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            FieldInfo field = parentType.GetField(_property.propertyPath, fieldFlags);
            _elementType = field.FieldType.GetElementType();
        }

        /// <summary>
        /// Sorts the collection of objects alphabetically.
        /// </summary>
        private void SortObjectsByName()
        {
            _objects.Sort((obj1, obj2) => obj1.Data.name.CompareTo(obj2.Data.name));
        }

        /// <summary>
        /// Sorts the collection of objects alphabetically.
        /// </summary>
        /// <param name="value">The object to be added.</param>
        public void AddObject(Object value)
        {
            int index = _property.arraySize;
            _property.InsertArrayElementAtIndex(index);
            _property.GetArrayElementAtIndex(index).objectReferenceValue = value;
            _property.serializedObject.ApplyModifiedProperties();
            _objects.Add(new ObjectInfo(value, index));
            SortObjectsByName();
        }

        /// <summary>
        /// Searches for an object in the collection. 
        /// If found, removes it and shifts the indices of all objects above it.
        /// </summary>
        /// <param name="atlas">The atlas in which to delete.</param>
        /// <param name="value">The object to delete.</param>
        /// <returns>True if the object is found and removed, false otherwise.</returns>
        public bool Delete(AtlasSO atlas, ObjectInfo value)
        {
            int index = _objects.IndexOf(value);
            if (index == -1)
            {
                return false;
            }
            else
            {
                string fileExtension = _isScriptableObject ? ".asset" : ".prefab";
                string path = atlas.AtlasDataPath + value.Data.name + fileExtension;
                string assetGUID = AssetDatabase.AssetPathToGUID(path);

                if (!string.IsNullOrEmpty(assetGUID))
                {
                    AssetDatabase.DeleteAsset(path);
                    _property.DeleteArrayElementAtIndex(value.Index);
                    _property.serializedObject.ApplyModifiedProperties();
                    FixIndices(value.Index);
                    _objects.Remove(value);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Decreases the indices of each object that is located above the specified one.
        /// </summary>
        /// <param name="startIndex">The starting index for decreasing.</param>
        public void FixIndices(int startIndex)
        {
            foreach (ObjectInfo objectInfo in _objects)
            {
                if (objectInfo.Index > startIndex)
                {
                    objectInfo.DecreaseIndex();
                }
            }
        }
        #endregion
    }

    private class ObjectInfo
    {
        #region Private fields
        private Object _data;
        private int _index;
        private Type _type;
        #endregion

        #region Public fields

        #endregion

        #region Properties
        public Object Data
        {
            get
            {
                return _data;
            }
        }

        public int Index
        {
            get
            {
                return _index;
            }
        }

        public Type Type
        {
            get
            {
                return _type;
            }
        }
        #endregion

        #region Methods
        public ObjectInfo(Object data, int index)
        {
            _data = data;
            _index = index;
            _type = data.GetType();
        }

        /// <summary>
        /// Decreases the index of the object by 1.
        /// </summary>
        public void DecreaseIndex()
        {
            _index--;
        }
        #endregion
    }

    #region Private fields
    [SerializeField] private VisualTreeAsset _createObjectTreeAsset;
    private List<AtlasInfo> _atlases;
    private List<ObjectInfo> _objectsAfterSearch;

    private AtlasInfo _currentAtlas;
    private CollectionInfo _currentCollection;
    private ObjectInfo _currentObject;
    private int _currentIndex;

    private Object _newObject;
    private string _newObjectName;
    private Type _newObjectType;
    private bool _isTypeScriptableObject;
    private string _pathToSaveNewObject;

    private SearchMode _currentSearchMode;
    private bool _isSearching;
    
    private ToolbarSearchField _searchField;
    private ListView _listView;
    private Button _deleteObjectButton;
    private Button _createObjectButton;
    private VisualElement _searchToolbar;
    private VisualElement _backToolbar;
    private VisualElement _deleteObjectToolbar;

    private event Action OnDisplayAtlases;
    private event Action OnDisplayAtlas;
    private event Action OnDisplayCollection;

    private static readonly string _styleResource = StaticInfo.StyleSheetsDirectory + "AtlasEditorWindowStyleSheet";
    private static readonly string _ussAtlasWindow = "atlas-window";
    private static readonly string _ussToolbar = _ussAtlasWindow + "__toolbar";
    private static readonly string _ussSearchToolbar = _ussAtlasWindow + "__toolbar-search";
    private static readonly string _ussBackToolbar = _ussAtlasWindow + "__toolbar-back";
    private static readonly string _ussDeleteObjectToolbar = _ussAtlasWindow + "__toolbar-delete-object";
    private static readonly string _ussLeftPane = _ussAtlasWindow + "__left-pane";
    private static readonly string _ussRightPane = _ussAtlasWindow + "__right-pane";
    private static readonly string _ussSearchField = _ussAtlasWindow + "__search-field";
    private static readonly string _ussListView = _ussAtlasWindow + "__list-view";
    private static readonly string _ussToolbarButton = _ussAtlasWindow + "__toolbar-button";
    private static readonly string _ussContentButton = _ussAtlasWindow + "__content-button";
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    private void OnDestroy()
    {
        if (_newObject != null)
        {
            DestroyImmediate(_newObject);
        }
    }

    private void Update()
    {
        _searchToolbar.style.width = _leftPane.style.width;
    }

    [MenuItem("Utils/Atlases")]
    public static void OpenWindow()
    {
        AtlasEditorWindow window = GetWindow<AtlasEditorWindow>();
        window.titleContent = new GUIContent("Atlases");
        window.minSize = new Vector2(700, 500);
        window.Show();
    }

    public override void InitializeEditorWindow()
    {
        base.InitializeEditorWindow();
        _searchField = new ToolbarSearchField();
        _listView = new ListView();
        _searchToolbar = new VisualElement();
        _backToolbar = new VisualElement();
        _deleteObjectToolbar = new VisualElement();

        _root.styleSheets.Add(Resources.Load<StyleSheet>(_styleResource));
        _toolbar.AddToClassList(_ussToolbar);
        _searchToolbar.AddToClassList(_ussSearchToolbar);
        _backToolbar.AddToClassList(_ussBackToolbar);
        _deleteObjectToolbar.AddToClassList(_ussDeleteObjectToolbar);
        _splitView.AddToClassList(_ussAtlasWindow);
        _leftPane.AddToClassList(_ussLeftPane);
        _rightPane.AddToClassList(_ussRightPane);
        _searchField.AddToClassList(_ussSearchField);
        _listView.AddToClassList(_ussListView);

        _atlases = new List<AtlasInfo>();
        _objectsAfterSearch = new List<ObjectInfo>();

        OnDisplayAtlases = () =>
        {
            ClearContent();
            ResetSearch();
            CancelObjectCreation();
            _backToolbar.Clear();
            _deleteObjectButton.SetEnabled(false);
            _currentSearchMode = SearchMode.Global;
        };
        OnDisplayAtlas = () =>
        {
            ClearContent();
            ResetSearch();
            CancelObjectCreation();
            _deleteObjectButton.SetEnabled(false);
            _currentSearchMode = SearchMode.Collections;
        };
        OnDisplayCollection = () =>
        {
            ClearContent();
            CancelObjectCreation();
            _currentSearchMode = SearchMode.Collection;
        };

        GetAllData();
    }

    public override void ComposeToolbar()
    {
        AddSearchObjectsField();
        _toolbar.Add(_backToolbar);
        AddDeleteObjectButton();
    }

    public override void ComposeLeftPane()
    {
        DisplayAllAtlases();
    }

    /// <summary>
    /// Loads all data from all atlases in the project.
    /// </summary>
    private void GetAllData()
    {
        string[] allAtlasesGuids = AssetDatabase.FindAssets("t:AtlasSO");
        foreach (var atlasGuid in allAtlasesGuids)
        {
            AtlasSO atlas = AssetDatabase.LoadAssetAtPath<AtlasSO>(AssetDatabase.GUIDToAssetPath(atlasGuid));
            _atlases.Add(new AtlasInfo(atlas));
        }
    }

    /// <summary>
    /// Resets the search and, if possible, sets the current collection as the source for the ListView.
    /// </summary>
    private void ResetSearch()
    {
        _isSearching = false;
        _searchField.SetValueWithoutNotify("");
        _listView.itemsSource = _currentCollection?.Objects;
        _listView.RefreshItems();
    }

    /// <summary>
    /// Cancels the creation and removes the new item from memory.
    /// </summary>
    private void CancelObjectCreation()
    {
        _createObjectButton?.SetEnabled(true);
        DestroyImmediate(_newObject);
    }

    /// <summary>
    /// Displays all atlases as a list of buttons.
    /// </summary>
    private void DisplayAllAtlases()
    {
        OnDisplayAtlases?.Invoke();
        foreach (AtlasInfo atlasInfo in _atlases)
        {
            AddAtlasButton(atlasInfo);
        }
    }

    /// <summary>
    /// Displays all collections as buttons.
    /// </summary>
    private void DisplayAtlas()
    {
        OnDisplayAtlas?.Invoke();
        foreach (CollectionInfo collectionInfo in _currentAtlas.Collections)
        {
            AddCollectionButton(collectionInfo);
        }

        ScrollView atlasScrollView = new ScrollView(ScrollViewMode.Vertical);
        atlasScrollView.Add(new InspectorElement(_currentAtlas.Atlas));
        _rightPane.Add(atlasScrollView);
    }

    /// <summary>
    /// Displays all objects in the collection as a ListView.
    /// </summary>
    private void DisplayCollection()
    {
        OnDisplayCollection?.Invoke();
        AddListViewForCollection(_currentCollection.Objects);
        AddCreateObjectButton();
    }

    /// <summary>
    /// Displays all found objects.
    /// </summary>
    private void DisplaySearchResult()
    {
        ClearContent();
        AddListViewForCollection(_objectsAfterSearch);
    }

    /// <summary>
    /// Displays all unique types and their occurrences.
    /// </summary>
    /// <param name="types">Dictionary where the key is the type and the value is the count of occurrences.</param>
    private void DisplaySelectedTypes(Dictionary<Type, int> types)
    {
        _rightPane.Clear();
        foreach (Type type in types.Keys)
        {
            Button button = CreateButton($"{types[type]} {type.Name}", _ussContentButton);
            button.clicked += () => HandleSelectType(type);
            _rightPane.Add(button);
        }
    }

    /// <summary>
    /// Displays a new object on the right panel and 
    /// adds an input field for entering a new object name 
    /// and a button to create the object.
    /// </summary>
    private void DisplayNewObject()
    {
        _newObject = _isTypeScriptableObject ? CreateInstance(_newObjectType) : new GameObject("newPrefab", _newObjectType);

        _rightPane.Clear();

        InspectorElement inspectorElement = new InspectorElement(_isTypeScriptableObject ? _newObject : (_newObject as GameObject).GetComponent(_newObjectType));
        _rightPane.Add(inspectorElement);

        _createObjectTreeAsset.CloneTree(inspectorElement);
        VisualElement createObjectElement = inspectorElement.Q("create-object");
        createObjectElement.style.marginTop = 5;

        TextField objectNameTextField = createObjectElement.Q<TextField>();
        objectNameTextField.RegisterValueChangedCallback(evt => _newObjectName = evt.newValue);

        Button createObject = createObjectElement.Q<Button>();
        createObject.clicked += HandleCreateObject;
    }

    /// <summary>
    /// Adds a button to the left panel. When clicked, displays a specific atlas.
    /// </summary>
    /// <param name="atlasInfo">Information about the atlas.</param>
    private void AddAtlasButton(AtlasInfo atlasInfo)
    {
        Button atlasButton = CreateButton(GetCorrectName(atlasInfo.Atlas), _ussContentButton);
        atlasButton.clicked += () =>
        {
            _currentAtlas = atlasInfo;
            AddBackToAtlasesButton();
            DisplayAtlas();
        };
        _leftPane.Add(atlasButton);
    }

    /// <summary>
    /// Adds a button to the left panel. When clicked, displays a specific collection.
    /// </summary>
    /// <param name="collectionInfo">Information about the collection.</param>
    private void AddCollectionButton(CollectionInfo collectionInfo)
    {
        Button collectionButton = CreateButton(GetCorrectName(collectionInfo.Property.displayName), _ussContentButton);
        collectionButton.clicked += () =>
        {
            _currentCollection = collectionInfo;
            AddBackToAtlasButton();
            DisplayCollection();
        };
        _leftPane.Add(collectionButton);
    }

    /// <summary>
    /// Adds a button to the toolbar. When clicked, returns to all atlases.
    /// </summary>
    private void AddBackToAtlasesButton()
    {
        Button backToAtlasesButton = CreateButton("Back to atlases", _ussToolbarButton);
        backToAtlasesButton.clicked += DisplayAllAtlases;
        _backToolbar.Add(backToAtlasesButton);
    }

    /// <summary>
    /// Adds a button to the toolbar. When clicked, returns to all collections.
    /// </summary>
    private void AddBackToAtlasButton()
    {
        Button backToAtlasButton = CreateButton($"Back to {GetCorrectName(_currentAtlas.Atlas).ToLower()}", _ussToolbarButton);
        backToAtlasButton.clicked += () =>
        {
            DisplayAtlas();
            _backToolbar.Remove(backToAtlasButton);
        };
        _backToolbar.Add(backToAtlasButton);
    }

    /// <summary>
    /// Adds a field for searching objects to the toolbar.
    /// </summary>
    private void AddSearchObjectsField()
    {
        _searchField.RegisterValueChangedCallback(evt => HandleSearchFieldChanged(evt));
        _searchField.RegisterCallback<FocusInEvent>(evt => HandleStartSearch());
        _toolbar.Add(_searchToolbar);
        _searchToolbar.Add(_searchField);
    }

    /// <summary>
    /// Adds a button to the toolbar for deleting one or more objects.
    /// </summary>
    private void AddDeleteObjectButton()
    {
        _deleteObjectButton = CreateButton("Delete", _ussToolbarButton);
        _deleteObjectButton.clicked += HandleDeleteObject;
        _toolbar.Add(_deleteObjectToolbar);
        _deleteObjectToolbar.Add(_deleteObjectButton);
    }

    /// <summary>
    /// Sets the list to a ListView and configures it.
    /// </summary>
    /// <param name="itemSource">The list for ListView.</param>
    private void AddListViewForCollection(List<ObjectInfo> itemSource)
    {
        _listView.itemsSource = itemSource;
        _listView.fixedItemHeight = 30;
        _listView.makeItem += HandleMakeObject;
        _listView.bindItem += HandleBindObject;
        _listView.selectionType = SelectionType.Multiple;
        _listView.selectionChanged += HandleObjectSelection;
        _listView.selectedIndex = _currentIndex;
        _listView.ClearSelection();
        _listView.AddToSelection(0);
        _leftPane.Add(_listView);
    }

    /// <summary>
    /// Adds a button to the left panel for creating an object.
    /// </summary>
    private void AddCreateObjectButton()
    {
        _createObjectButton = CreateButton("Create new object", _ussContentButton);
        _createObjectButton.clicked += HandleStartCreateObject;
        _leftPane.Add(_createObjectButton);
    }

    /// <summary>
    /// Creates a new button.
    /// </summary>
    /// <param name="name">The name of the new button.</param>
    /// <param name="className">The class name for button styles.</param>
    /// <returns>A new button with a name and style.</returns>
    private Button CreateButton(string name, string className)
    {
        Button button = new Button();
        button.AddToClassList(className);
        button.text = name;
        return button;
    }

    /// <summary>
    /// Takes the name from the object and returns the corrected one.
    /// </summary>
    /// <param name="obj">The object whose name needs to be returned.</param>
    /// <returns>The name of the object with spaces between words.</returns>
    private string GetCorrectName(Object obj)
    {
        return GetCorrectName(obj.name);
    }

    /// <summary>
    /// Takes the name and returns the corrected one.
    /// </summary>
    /// <param name="name">The name to be processed.</param>
    /// <returns>The name of the object with spaces between words.</returns>
    private string GetCorrectName(string name)
    {
        name = Regex.Replace(name, @"(\p{Ll}) (\P{Ll})", m => m.Groups[1].Value + ' ' + m.Groups[2].Value.ToLower());
        return Regex.Replace(name, @"(\p{Ll})(\P{Ll})", m => m.Groups[1].Value + ' ' + m.Groups[2].Value.ToLower());
    }

    /// <summary>
    /// Retrieves the sprite based on the object's type.
    /// </summary>
    /// <param name="obj">The object whose sprite needs to be returned.</param>
    /// <returns>An icon based on the object's type.</returns>
    private Sprite GetSpriteByObject(Object obj)
    {
        return obj switch
        {
            BlockSO block => block.Sprites.Count != 0 ? block.Sprites[0] : null,
            Tree tree => tree.GetComponent<SpriteRenderer>()?.sprite,
            PickUpItem pickUpItem => pickUpItem.GetComponent<SpriteRenderer>()?.sprite,
            _ => null
        };
    }

    /// <summary>
    /// Event handler for ListView's MakeItem event.
    /// </summary>
    /// <returns>A new item for the ListView.</returns>
    private VisualElement HandleMakeObject()
    {
        return new ListItem();
    }

    /// <summary>
    /// Event handler for ListView's BindItem event. Sets the icon and name of the item.
    /// </summary>
    /// <param name="item">Visual element for binding with an item.</param>
    /// <param name="index">Index of the item.</param>
    private void HandleBindObject(VisualElement item, int index)
    {
        ObjectInfo obj = _listView.itemsSource[index] as ObjectInfo;
        if (obj == null)
        {
            return;
        }

        ListItem cell = item as ListItem;
        cell.SetName(GetCorrectName(obj.Data));
        cell.SetIcon(GetSpriteByObject(obj.Data));
    }

    /// <summary>
    /// Event handler for ListView's SelectionChanged event. 
    /// If multiple items are selected, finds all unique types. 
    /// If there are more than one, displays them for further processing. 
    /// Otherwise, displays the Editor for one or multiple objects.
    /// </summary>
    /// <param name="selectedItems"></param>
    private void HandleObjectSelection(IEnumerable<object> selectedItems)
    {
        _currentIndex = _listView.selectedIndex;
        _currentObject = selectedItems.FirstOrDefault() as ObjectInfo;
        int count = selectedItems.Count();
        bool isAnyItemSelected = _currentObject != null;
        _deleteObjectButton.SetEnabled(isAnyItemSelected);

        if (isAnyItemSelected)
        {
            Editor editor;
            CancelObjectCreation();
            if (count > 1)
            {
                Dictionary<Type, int> types = selectedItems
                    .Cast<ObjectInfo>()
                    .GroupBy(obj => obj.Type)
                    .OrderByDescending(group => group.Count())
                    .ToDictionary(group => group.Key, group => group.Count());
                if (types.Keys.Count > 1)
                {
                    DisplaySelectedTypes(types);
                    return;
                }

                Object[] objects = selectedItems
                    .Cast<ObjectInfo>()
                    .Select(obj => obj.Data)
                    .ToArray();
                editor = Editor.CreateEditor(objects);
            }
            else
            {
                editor = Editor.CreateEditor(_currentObject.Data);
            }

            _rightPane.Clear();
            InspectorElement inspectorElement = new InspectorElement(editor);
            inspectorElement.RegisterCallback<SerializedPropertyChangeEvent>(evt => HandleUpdatePreview(evt));
            _rightPane.Add(inspectorElement);
        }
    }

    /// <summary>
    /// Event handler for the button click event to start creating an object.
    /// Initiates the actual creation.
    /// </summary>
    private void HandleStartCreateObject()
    {
        _pathToSaveNewObject = _currentAtlas.Atlas.AtlasDataPath;
        _newObjectType = _currentCollection.ElementType;
        _isTypeScriptableObject = _currentCollection.IsScriptableObject;

        _createObjectButton.SetEnabled(false);
        _deleteObjectButton.SetEnabled(false);
        _listView.ClearSelection();

        ResetSearch();
        DisplayNewObject();
    }

    /// <summary>
    /// Event handler for the button click event to create an object, 
    /// save it to disk, and add it to the collection.
    /// </summary>
    private void HandleCreateObject()
    {
        if (string.IsNullOrEmpty(_newObjectName))
        {
            return;
        }

        if (SaveObject())
        {
            AddObjectToCurrentCollection();
            DisplayNewObject();
        }
    }

    /// <summary>
    /// Event handler for the button click event to delete all selected items.
    /// </summary>
    private void HandleDeleteObject()
    {

        foreach (ObjectInfo selectedObject in _listView.selectedItems)
        {
            foreach (AtlasInfo atlas in _atlases)
            {
                if (atlas.Delete(selectedObject))
                {
                    if (_isSearching)
                    {
                        _objectsAfterSearch.Remove(selectedObject);
                    }
                    break;
                }
            }
        }
        _listView.ClearSelection();
        _listView.RefreshItems();
    }

    /// <summary>
    /// Event handler for the FocusInEvent of the search field, if the search field is empty.
    /// </summary>
    private void HandleStartSearch()
    {
        if (_searchField.value == "")
        {
            _objectsAfterSearch.Clear();
            switch (_currentSearchMode)
            {
                case SearchMode.Global:
                    {
                        SearchGloabal("");
                        if (!_isSearching)
                        {
                            _isSearching = true;
                            DisplaySearchResult();
                            AddBackToAtlasesButton();
                        }
                    }
                    break;
                case SearchMode.Collections:
                    {
                        SearchCollections(_currentAtlas, "");
                        if (!_isSearching)
                        {
                            _isSearching = true;
                            DisplaySearchResult();
                            AddBackToAtlasButton();
                        }
                    }
                    break;
                case SearchMode.Collection:
                    {
                        SearchCollection(_currentCollection, "");
                        if (!_isSearching)
                        {
                            _isSearching = true;
                            _listView.itemsSource = _objectsAfterSearch;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        SortSearchResultByName();
        _listView.RefreshItems();
    }

    /// <summary>
    /// Event handler for the value change event in the search string. 
    /// Performs global, collection-based, or local search.
    /// </summary>
    /// <param name="evt">The new value from the search string.</param>
    private void HandleSearchFieldChanged(ChangeEvent<string> evt)
    {
        string searchString = evt.newValue.ToLower();
        _objectsAfterSearch.Clear();
        switch (_currentSearchMode)
        {
            case SearchMode.Global:
                {
                    SearchGloabal(searchString);
                }
                break;
            case SearchMode.Collections:
                {
                    SearchCollections(_currentAtlas, searchString);
                }
                break;
            case SearchMode.Collection:
                {
                    SearchCollection(_currentCollection,searchString);
                }
                break;
            default:
                break;
        }
        SortSearchResultByName();
        _listView.ClearSelection();
        _listView.AddToSelection(0);
        _listView.RefreshItems();
    }

    /// <summary>
    /// Event handler for the button click event to select a specific type of multiple selected objects.
    /// </summary>
    /// <param name="type">The type of the object.</param>
    private void HandleSelectType(Type type)
    {
        List<int> newSelectedIndices = new List<int>();
        IEnumerable<int> selectedIndices = _listView.selectedIndices;
        IEnumerable<object> selectedItems = _listView.selectedItems;
        for (int i = 0; i < selectedItems.Count(); i++)
        {
            object obj = (selectedItems.ElementAt(i) as ObjectInfo).Data;
            if (obj.GetType() == type)
            {
                newSelectedIndices.Add(selectedIndices.ElementAt(i));
            }
        }
        _listView.SetSelection(newSelectedIndices);
    }

    /// <summary>
    /// Event handler for the SerializedPropertyChangeEvent to update the object preview.
    /// </summary>
    /// <param name="evt">The event data.</param>
    private void HandleUpdatePreview(SerializedPropertyChangeEvent evt)
    {
        SerializedProperty changedProperty = evt.changedProperty;
        if (!changedProperty.isArray && changedProperty.boxedValue is Sprite)
        {
            _listView.RefreshItem(_currentIndex);
        }
    }

    /// <summary>
    /// Saves the new object to disk based on its type.
    /// </summary>
    /// <returns>True if the object is successfully saved, false if an error occurs.</returns>
    private bool SaveObject()
    {
        string assetName = _newObjectName.Replace(" ", "");
        string fileExtension = _isTypeScriptableObject ? ".asset" : ".prefab";
        string path = _pathToSaveNewObject + assetName + fileExtension;
        Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
        if (asset != null)
        {
            return false;
        }
        if (_isTypeScriptableObject)
        {
            AssetDatabase.CreateAsset(_newObject, path);
            return true;
        }
        else
        {
            GameObject oldObject = _newObject as GameObject;
            _newObject = PrefabUtility.SaveAsPrefabAssetAndConnect(_newObject as GameObject, path, InteractionMode.UserAction).GetComponent(_newObjectType);
            DestroyImmediate(oldObject);
            return true;
        }
    }

    /// <summary>
    /// Adds the object to the current collection. Refresh ListView
    /// </summary>
    private void AddObjectToCurrentCollection()
    {
        _currentCollection.AddObject(_newObject); 
        _newObject = null;
        _listView.RefreshItems();
    }

    /// <summary>
    /// Performs a global search across all available objects.
    /// </summary>
    /// <param name="searchString">The string to search for.</param>
    private void SearchGloabal(string searchString)
    {
        foreach (AtlasInfo atlas in _atlases)
        {
            SearchCollections(atlas, searchString);
        }
    }

    /// <summary>
    /// Searches through all collections of a specific atlas.
    /// </summary>
    /// <param name="atlas">The selected atlas.</param>
    /// <param name="searchString">The string to search for.</param>
    private void SearchCollections(AtlasInfo atlas, string searchString)
    {
        foreach (CollectionInfo collection in atlas.Collections)
        {
            SearchCollection(collection, searchString);
        }
    }

    /// <summary>
    /// Searches through all objects in a specific collection.
    /// </summary>
    /// <param name="collection">The selected collection.</param>
    /// <param name="searchString">The string to search for.</param>
    private void SearchCollection(CollectionInfo collection, string searchString)
    {
        foreach (ObjectInfo value in collection.Objects)
        {
            if (GetCorrectName(value.Data).ToLower().Contains(searchString))
            {
                _objectsAfterSearch.Add(value);
            }
        }
    }

    /// <summary>
    /// Sorts the search result alphabetically.
    /// </summary>
    private void SortSearchResultByName()
    {
        _objectsAfterSearch.Sort((obj1, obj2) => obj1.Data.name.CompareTo(obj2.Data.name));
    }
    #endregion
}