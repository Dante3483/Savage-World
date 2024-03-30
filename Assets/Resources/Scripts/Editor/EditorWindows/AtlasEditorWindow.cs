using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class AtlasEditorWindow : TwoPaneEditorWindow
{
    #region Private fields
    private List<AtlasSO> _atlases;
    private Dictionary<AtlasSO, List<SerializedProperty>> _collectionsByAtlas;
    private Dictionary<string, SerializedProperty> _collectionByName;
    private Dictionary<string, List<Object>> _objectsByCollectionName;

    private List<Object> _currentObjectsCollection;
    private SolidBlockSO _newObject;
    private int _selectedIndex;

    private readonly string _styleResource = StaticInfo.StyleSheetsDirectory + "AtlasEditorWindowStyleSheet";
    private readonly string _ussContentButton = "content-button";
    private readonly string _ussToolbarButton = "toolbar-button";
    private readonly string _ussSearch = "search";
    private readonly string _ussSearchPlaceholder = "search-placeholder";
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

        _root.styleSheets.Add(Resources.Load<StyleSheet>(_styleResource));
        _atlases = new List<AtlasSO>();
        _collectionsByAtlas = new Dictionary<AtlasSO, List<SerializedProperty>>();
        _collectionByName = new Dictionary<string, SerializedProperty>();
        _objectsByCollectionName = new Dictionary<string, List<Object>>();
    }

    public override void ComposeLeftPane()
    {
        Button button = new Button(() =>
        {
            _rightPane.Clear();
            string path = AssetDatabase.GetAssetPath(_currentObjectsCollection[0]);
            path = Path.GetDirectoryName(path) + '\\';
            Debug.Log(path);
            _newObject = CreateInstance<SolidBlockSO>();
            _newObject.name = "NewBlock";
            InspectorElement inspectorElement = new InspectorElement(_newObject);
            _rightPane.Add(inspectorElement);
            Button create = new Button(() =>
            {
                string[] existingAssets = AssetDatabase.FindAssets(_newObject.name, new[] { path });
                if (existingAssets.Length > 0)
                {
                    _newObject.name += existingAssets.Length;
                }
                AssetDatabase.CreateAsset(_newObject, path + _newObject.name + ".asset");
                AssetDatabase.SaveAssets();
                //_leftPane.Q<ListView>()?.Rebuild();

                SerializedObject serializedObject = new SerializedObject(_atlases[0]);
                SerializedProperty collection = serializedObject.FindProperty("_solidBlocks");
                collection.InsertArrayElementAtIndex(collection.arraySize);
                collection.GetArrayElementAtIndex(collection.arraySize - 1).objectReferenceValue = _newObject;
                serializedObject.ApplyModifiedProperties();
                _currentObjectsCollection.Add(_newObject);
                //_leftPane.Q<ListView>()?.Rebuild();
                _newObject = CreateInstance<SolidBlockSO>();
                _newObject.name = "NewBlock";
            });
            create.text = "Create";
            _rightPane.Add(create);
        });
        button.text = "Create";
        _root.Add(button);
        GetAllData();
        DisplayAllAtlases();
    }

    private void GetAllData()
    {
        string[] allAtlasesGuids = AssetDatabase.FindAssets("t:AtlasSO");
        foreach (var atlasGuid in allAtlasesGuids)
        {
            AtlasSO atlas = AssetDatabase.LoadAssetAtPath<AtlasSO>(AssetDatabase.GUIDToAssetPath(atlasGuid));
            _atlases.Add(atlas);
            GetAllAtlasCollections(atlas);
        }
    }

    private void GetAllAtlasCollections(AtlasSO atlas)
    {
        _collectionsByAtlas.Add(atlas, new List<SerializedProperty>());
        SerializedProperty property = new SerializedObject(atlas).GetIterator();
        property.NextVisible(true);
        while (property.NextVisible(false))
        {
            if (property.isArray && property.propertyType == SerializedPropertyType.Generic)
            {
                SerializedProperty collection = property.Copy();
                _collectionsByAtlas[atlas].Add(collection);
                _collectionByName.Add(property.name, collection);
                GetAllCollectionData(property);
            }
        }
    }

    private void GetAllCollectionData(SerializedProperty collection)
    {
        string collectionName = collection.displayName;
        _objectsByCollectionName.Add(collectionName, new List<Object>());
        for (int i = 0; i < collection.arraySize; i++)
        {
            Object element = collection.GetArrayElementAtIndex(i).objectReferenceValue;
            if (element == null)
            {
                collection.DeleteArrayElementAtIndex(i--);
                collection.serializedObject.ApplyModifiedProperties();
                continue;
            }
            _objectsByCollectionName[collectionName].Add(collection.GetArrayElementAtIndex(i).objectReferenceValue);
        }
    }
    
    private void DisplayAllAtlases()
    {
        _leftPane.Clear();
        _rightPane.Clear();
        _selectedIndex = 0;
        foreach (AtlasSO atlas in _atlases)
        {
            Button atlasButton = CreateContentButton(GetCorrectNameByObject(atlas));
            atlasButton.RegisterCallback<ClickEvent>(evt =>
            {
                Button toolbarButton = CreateToolbarButton("Back to atlases");
                toolbarButton.RegisterCallback<ClickEvent>(evt =>
                {
                    DisplayAllAtlases();
                    _toolbar.Clear();
                });
                _toolbar.Add(toolbarButton);
                DisplayAtlas(atlas);
            });
            _leftPane.Add(atlasButton);
        }
    }

    private void DisplayAtlas(AtlasSO atlas)
    {
        _leftPane.Clear();
        _rightPane.Clear();
        _selectedIndex = 0;
        List<SerializedProperty> collections = _collectionsByAtlas[atlas];
        if (collections.Count > 1)
        {
            foreach (SerializedProperty collection in collections)
            {
                Button collectionButton = CreateContentButton(collection.displayName);
                collectionButton.RegisterCallback<ClickEvent>(evt =>
                {
                    Button toolbarButton = CreateToolbarButton($"Back to {GetCorrectNameByObject(atlas).ToLower()}");
                    toolbarButton.RegisterCallback<ClickEvent>(evt =>
                    {
                        DisplayAtlas(atlas);
                        _toolbar.Remove(toolbarButton);
                    });
                    _toolbar.Add(toolbarButton);
                    DisplayCollection(collection.displayName);
                });
                _leftPane.Add(collectionButton);
            }
        }
        else
        {
            DisplayCollection(collections[0].displayName);
        }
    }

    private void DisplayCollection(string collectionName)
    {
        _leftPane.Clear();
        List<Object> objects = _objectsByCollectionName[collectionName];
        _currentObjectsCollection = objects;

        ListView listView = new ListView();
        listView.makeItem = () => new ListItem();
        listView.bindItem = OnObjectBinding;
        listView.itemsSource = _currentObjectsCollection;
        listView.selectionChanged += OnObjectSelectionChange;
        listView.selectionChanged += (items) => { _selectedIndex = listView.selectedIndex; };
        listView.selectedIndex = _selectedIndex;
        listView.horizontalScrollingEnabled = false;

        TextField searchField = CreateSearchField("Search");
        searchField.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            if (!searchField.ClassListContains(_ussSearchPlaceholder))
            {
                _currentObjectsCollection = objects.FindAll(obj => GetCorrectNameByObject(obj).ToLower().Contains(searchField.text.ToLower()));
                listView.itemsSource = _currentObjectsCollection;
            }
        });
        _leftPane.Add(searchField);
        _leftPane.Add(listView);
    }

    private Button CreateContentButton(string name)
    {
        Button button = new Button();
        button.AddToClassList(_ussContentButton);
        button.text = name;
        return button;
    }

    private Button CreateToolbarButton(string name)
    {
        Button button = new Button();
        button.AddToClassList(_ussToolbarButton);
        button.text = name;
        return button;
    }

    private TextField CreateSearchField(string placeholder)
    {
        TextField textField = new TextField();
        textField.AddToClassList(_ussSearch);
        textField.AddToClassList(_ussSearchPlaceholder);
        textField.RegisterCallback<FocusInEvent>(evt =>
        {
            
            if (textField.ClassListContains(_ussSearchPlaceholder))
            {
                textField.SetValueWithoutNotify("");
                textField.RemoveFromClassList(_ussSearchPlaceholder);
            }
        });
        textField.RegisterCallback<FocusOutEvent>(evt =>
        {
            if (textField.value == string.Empty)
            {
                textField.SetValueWithoutNotify(placeholder);
                textField.AddToClassList(_ussSearchPlaceholder);
            }
        });
        textField.value = placeholder;
        textField.multiline = false;
        return textField;
    }

    private string GetCorrectNameByObject(Object obj)
    {
        return Regex.Replace(obj.name, @"(\p{Ll})(\P{Ll})", m => m.Groups[1].Value + ' ' + m.Groups[2].Value.ToLower());
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

    private void OnObjectSelectionChange(IEnumerable<object> selectedItems)
    {
        _rightPane.Clear();
        Object obj = selectedItems.First() as Object;
        if (obj == null)
        {
            return;
        }
        InspectorElement inspectorElement = new InspectorElement(obj);
        inspectorElement.RegisterCallback<SerializedPropertyChangeEvent>(evt =>
        {
            _leftPane.Q<ListView>()?.Rebuild();
        });
        _rightPane.Add(inspectorElement);
    }

    private void OnObjectBinding(VisualElement item, int index)
    {
        ListItem cell = item as ListItem;
        Object obj = _currentObjectsCollection[index];
        cell.SetName(GetCorrectNameByObject(obj));
        cell.SetIcon(GetIconByObject(obj));
    }
    #endregion
}