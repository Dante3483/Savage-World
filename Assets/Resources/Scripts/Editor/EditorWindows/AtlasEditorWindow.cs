using System;
using System.Collections.Generic;
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
                _objects.Add(new ObjectInfo(collectionObject));
            }
        }

        private void SetElementType()
        {
            Type parentType = _property.serializedObject.targetObject.GetType();
            BindingFlags fieldFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            FieldInfo field = parentType.GetField(_property.propertyPath, fieldFlags);
            _elementType = field.FieldType.GetElementType();
        }

        public void AddObject(Object value)
        {
            int arraySize = _property.arraySize;
            _property.InsertArrayElementAtIndex(arraySize);
            _property.GetArrayElementAtIndex(arraySize).objectReferenceValue = value;
            _property.serializedObject.ApplyModifiedProperties();
            _objects.Add(new ObjectInfo(value));
        }

        public ObjectInfo FindByName(string name)
        {
            return _objects.Find(obj => obj.Data.name == name);
        }

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
                    _property.DeleteArrayElementAtIndex(index);
                    _property.serializedObject.ApplyModifiedProperties();
                    _objects.RemoveAt(index);
                    return true;
                }
                return false;
            }
        }
        #endregion
    }

    private class ObjectInfo
    {
        #region Private fields
        private Object _data;
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
        #endregion

        #region Methods
        public ObjectInfo(Object data)
        {
            _data = data;
        }
        #endregion
    }

    #region Private fields
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
        _rightPane = new ScrollView(ScrollViewMode.Vertical);
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

    private void GetAllData()
    {
        string[] allAtlasesGuids = AssetDatabase.FindAssets("t:AtlasSO");
        foreach (var atlasGuid in allAtlasesGuids)
        {
            AtlasSO atlas = AssetDatabase.LoadAssetAtPath<AtlasSO>(AssetDatabase.GUIDToAssetPath(atlasGuid));
            _atlases.Add(new AtlasInfo(atlas));
        }
    }

    private void ClearContent()
    {
        _leftPane.Clear();
        _rightPane.Clear();
    }

    private void ResetSearch()
    {
        _isSearching = false;
        _searchField.SetValueWithoutNotify("");
        _listView.itemsSource = _currentCollection?.Objects;
        _listView.RefreshItems();
    }

    private void CancelObjectCreation()
    {
        _createObjectButton?.SetEnabled(true);
        DestroyImmediate(_newObject);
    }

    private void DisplayAllAtlases()
    {
        OnDisplayAtlases?.Invoke();
        foreach (AtlasInfo atlasInfo in _atlases)
        {
            AddAtlasButton(atlasInfo);
        }
    }

    private void DisplayAtlas()
    {
        OnDisplayAtlas?.Invoke();
        foreach (CollectionInfo collectionInfo in _currentAtlas.Collections)
        {
            AddCollectionButton(collectionInfo);
        }

        InspectorElement atlasInspectorElement = new InspectorElement(_currentAtlas.Atlas);
        _rightPane.Add(atlasInspectorElement);
    }

    private void DisplayCollection()
    {
        OnDisplayCollection?.Invoke();
        AddListViewForCollection(_currentCollection.Objects);
        AddCreateObjectButton();
    }

    private void DisplaySearchResult()
    {
        ClearContent();
        AddListViewForCollection(_objectsAfterSearch);
    }

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

    private void AddBackToAtlasesButton()
    {
        Button backToAtlasesButton = CreateButton("Back to atlases", _ussToolbarButton);
        backToAtlasesButton.clicked += DisplayAllAtlases;
        _backToolbar.Add(backToAtlasesButton);
    }

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

    private void AddSearchObjectsField()
    {
        _searchField.RegisterValueChangedCallback(evt => HandleSearchFieldChanged(evt));
        _searchField.RegisterCallback<FocusInEvent>(evt => HandleStartSearch());
        _toolbar.Add(_searchToolbar);
        _searchToolbar.Add(_searchField);
    }

    private void AddDeleteObjectButton()
    {
        _deleteObjectButton = CreateButton("Delete object", _ussToolbarButton);
        _deleteObjectButton.clicked += HandleDeleteObject;
        _toolbar.Add(_deleteObjectToolbar);
        _deleteObjectToolbar.Add(_deleteObjectButton);
    }

    private void AddListViewForCollection(List<ObjectInfo> itemSource)
    {
        _listView.itemsSource = itemSource;
        _listView.fixedItemHeight = 30;
        _listView.makeItem += HandleMakeObject;
        _listView.bindItem += HandleBindObject;
        _listView.selectionChanged += HandleObjectSelection;
        _listView.selectedIndex = _currentIndex;
        _listView.ClearSelection();
        _listView.AddToSelection(0);
        _leftPane.Add(_listView);
    }

    private void AddCreateObjectButton()
    {
        _createObjectButton = CreateButton("Create new object", _ussContentButton);
        _createObjectButton.clicked += HandleStartCreateObject;
        _leftPane.Add(_createObjectButton);
    }

    private Button CreateButton(string name, string className)
    {
        Button button = new Button();
        button.AddToClassList(className);
        button.text = name;
        return button;
    }

    private string GetCorrectName(Object obj)
    {
        return GetCorrectName(obj.name);
    }

    private string GetCorrectName(string name)
    {
        name = Regex.Replace(name, @"(\p{Ll}) (\P{Ll})", m => m.Groups[1].Value + ' ' + m.Groups[2].Value.ToLower());
        return Regex.Replace(name, @"(\p{Ll})(\P{Ll})", m => m.Groups[1].Value + ' ' + m.Groups[2].Value.ToLower());
    }

    private Sprite GetIconByObject(Object obj)
    {
        return obj switch
        {
            BlockSO block => block.Sprites.Count != 0 ? block.Sprites[0] : null,
            Tree tree => tree.GetComponent<SpriteRenderer>()?.sprite,
            PickUpItem pickUpItem => pickUpItem.GetComponent<SpriteRenderer>()?.sprite,
            _ => null
        };
    }

    private VisualElement HandleMakeObject()
    {
        return new ListItem();
    }

    private void HandleBindObject(VisualElement item, int index)
    {
        ObjectInfo obj = _listView.itemsSource[index] as ObjectInfo;
        if (obj == null)
        {
            return;
        }

        ListItem cell = item as ListItem;
        cell.SetName(GetCorrectName(obj.Data));
        cell.SetIcon(GetIconByObject(obj.Data));
    }

    private void HandleObjectSelection(IEnumerable<object> selectedItems)
    {
        _currentIndex = _listView.selectedIndex;
        _currentObject = selectedItems.FirstOrDefault() as ObjectInfo;
        bool isItemSelected = _currentObject != null;

        _rightPane.Clear();
        _deleteObjectButton.SetEnabled(isItemSelected);

        if (isItemSelected)
        {
            CancelObjectCreation();

            InspectorElement inspectorElement = new InspectorElement(_currentObject.Data);
            inspectorElement.RegisterCallback<SerializedPropertyChangeEvent>(evt => _listView.RefreshItem(_currentIndex));
            _rightPane.Add(inspectorElement);
        }
    }

    private void HandleStartCreateObject()
    {
        _pathToSaveNewObject = _currentAtlas.Atlas.AtlasDataPath;
        _newObjectType = _currentCollection.ElementType;
        _isTypeScriptableObject = _currentCollection.IsScriptableObject;

        _createObjectButton.SetEnabled(false);
        _deleteObjectButton.SetEnabled(false);
        _listView.ClearSelection();

        ResetSearch();
        StartCreateObject();
    }

    private void HandleCreateObject()
    {
        if (string.IsNullOrEmpty(_newObjectName))
        {
            return;
        }

        if (SaveObject())
        {
            AddObjectToCurrentCollection();
            StartCreateObject();
        }
    }

    private void HandleDeleteObject()
    {
        foreach (AtlasInfo atlas in _atlases)
        {
            if (atlas.Delete(_currentObject))
            {
                if (_isSearching)
                {
                    _objectsAfterSearch.Remove(_currentObject);
                }
                _listView.ClearSelection();
                _listView.AddToSelection(_currentIndex - 1);
                _listView.RefreshItems();
                break;
            }
        }
    }

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
    }

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
        _listView.ClearSelection();
        _listView.AddToSelection(0);
    }

    private void StartCreateObject()
    {
        _newObject = _isTypeScriptableObject ? CreateInstance(_newObjectType) : new GameObject("newPrefab", _newObjectType);

        _rightPane.Clear();

        TextField objectNameTextField = new TextField();
        objectNameTextField.RegisterValueChangedCallback(evt =>
        {
            _newObjectName = evt.newValue;
        });
        _rightPane.Add(objectNameTextField);

        Button createObject = new Button();
        createObject.text = "Create";
        createObject.clicked += HandleCreateObject;
        _rightPane.Add(createObject);

        InspectorElement inspectorElement = new InspectorElement(_isTypeScriptableObject ? _newObject : (_newObject as GameObject).GetComponent(_newObjectType));
        _rightPane.Add(inspectorElement);
    }

    private bool SaveObject()
    {
        string assetName = _newObjectName.Replace(" ", "");
        string path = _pathToSaveNewObject + assetName;
        if (_currentCollection.FindByName(assetName) != null)
        {
            return false;
        }
        if (_isTypeScriptableObject)
        {
            path += ".asset";
            AssetDatabase.CreateAsset(_newObject, path);
            AssetDatabase.SaveAssets();
            return true;
        }
        else
        {
            path += ".prefab";
            GameObject oldObject = _newObject as GameObject;
            _newObject = PrefabUtility.SaveAsPrefabAssetAndConnect(_newObject as GameObject, path, InteractionMode.UserAction).GetComponent(_newObjectType);
            DestroyImmediate(oldObject);
            return true;
        }
    }

    private void AddObjectToCurrentCollection()
    {
        _currentCollection.AddObject(_newObject); 
        _newObject = null;
        _listView.RefreshItems();
    }

    private void SearchGloabal(string searchString)
    {
        foreach (AtlasInfo atlas in _atlases)
        {
            SearchCollections(atlas, searchString);
        }
        _listView.RefreshItems();
    }

    private void SearchCollections(AtlasInfo atlas, string searchString)
    {
        foreach (CollectionInfo collection in atlas.Collections)
        {
            SearchCollection(collection, searchString);
        }
        _listView.RefreshItems();
    }

    private void SearchCollection(CollectionInfo collection, string searchString)
    {
        foreach (ObjectInfo value in collection.Objects)
        {
            if (GetCorrectName(value.Data).ToLower().Contains(searchString))
            {
                _objectsAfterSearch.Add(value);
            }
        }
        _listView.RefreshItems();
    }
    #endregion
}