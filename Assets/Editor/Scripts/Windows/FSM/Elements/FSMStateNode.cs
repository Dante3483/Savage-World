using SavageWorld.Runtime.Attributes;
using SavageWorld.Runtime.Utilities;
using SavageWorld.Runtime.Utilities.FSM;
using SavageWorld.Runtime.Utilities.FSM.Conditions;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class FSMStateNode : Node
{
    #region Fields
    private static readonly string _styleName = "FSMStateNode";
    private static readonly string _stylePath = StaticParameters.StylesPath + _styleName + StaticParameters.StyleExtension;

    private FSMSearchProvider _searchProvider;
    private FSMStateSO _state;
    private FSMStateSO _stateInEdit;
    private Port _inputPort;
    private Port _outputPort;
    #endregion

    #region Properties
    public FSMStateSO State
    {
        get
        {
            return _state;
        }
    }

    public Port InputPort
    {
        get
        {
            return _inputPort;
        }

        set
        {
            _inputPort = value;
        }
    }

    public Port OutputPort
    {
        get
        {
            return _outputPort;
        }

        set
        {
            _outputPort = value;
        }
    }
    #endregion

    #region Events / Delegates
    public Action<FSMStateNode> StateSelected;
    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public FSMStateNode() : this(null)
    {

    }

    public FSMStateNode(FSMStateSO state) : base()
    {
        styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(_stylePath));
        Initialize(state);
        this.Bind(new(_state));
    }

    public void UpdateTitleContainer()
    {
        titleContainer.Clear();
        InitializeTitleContainer();
        this.Bind(new(_state));
    }

    public void UpdateInputContainer()
    {
        inputContainer.Clear();
        InitializeOutputContainer();
        this.Bind(new(_state));
    }

    public void UpdateOutputContainer()
    {
        outputContainer.Clear();
        InitializeOutputContainer();
        this.Bind(new(_state));
    }

    public void UpdateExtensionContainer()
    {
        extensionContainer.Clear();
        InitializeExtensionContainer();
        this.Bind(new(_state));
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Undo.RecordObject(_state, "FSM (Set Position)");
        _state.Position = newPos.min;
        EditorUtility.SetDirty(_state);
    }

    public override void OnSelected()
    {
        base.OnSelected();
        StateSelected?.Invoke(this);
    }

    public override void OnUnselected()
    {
        base.OnUnselected();
        StateSelected?.Invoke(null);
    }

    public override Port InstantiatePort(Orientation orientation, Direction direction, Port.Capacity capacity, Type type)
    {
        return Port.Create<FSMEdge>(orientation, direction, capacity, type);
    }
    #endregion

    #region Private Methods
    private void Initialize(FSMStateSO state)
    {
        _state = state;
        viewDataKey = _state.Guid;
        style.left = _state.Position.x;
        style.top = _state.Position.y;
        InitializeSearchWindow();
        InitializeTitleContainer();
        InitializeInputContainer();
        InitializeOutputContainer();
        InitializeExtensionContainer();
    }

    private void InitializeSearchWindow()
    {
        _searchProvider = ScriptableObject.CreateInstance<FSMSearchProvider>();
        _searchProvider.Name = "Conditions";
        _searchProvider.Type = typeof(FSMConditionBase);
        _searchProvider.OnSelect = CreateCondition;
    }

    private void InitializeTitleContainer()
    {
        TextField titleTextField = new()
        {
            bindingPath = "_name",
            name = "state-name"
        };
        titleContainer.Insert(0, titleTextField);
    }

    private void InitializeInputContainer()
    {
        _inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
        _inputPort.portName = "";
        inputContainer.Add(_inputPort);
    }

    private void InitializeOutputContainer()
    {
        _outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
        _outputPort.portName = "";
        outputContainer.Add(_outputPort);
    }

    private void InitializeExtensionContainer()
    {
        if (_state == null)
        {
            return;
        }
        VisualElement container = new();

        if (_state.ListOfChildren.Count > 0)
        {
            Foldout transitionsFoldout = new()
            {
                text = "Transitions",
                value = false
            };
            SerializedProperty conditionsByChild = new SerializedObject(_state).FindProperty("_conditionByChild");
            SerializedProperty listOfKeyValuePair = conditionsByChild.FindPropertyRelative("_listOfKeyValuePair");
            for (int i = 0; i < listOfKeyValuePair.arraySize; i++)
            {
                SerializedProperty arrayElement = listOfKeyValuePair.GetArrayElementAtIndex(i);
                SerializedProperty keyProperty = arrayElement.FindPropertyRelative("_key");
                SerializedProperty valueProperty = arrayElement.FindPropertyRelative("_value");
                FSMStateSO child = (FSMStateSO)keyProperty.objectReferenceValue;
                FSMConditionBase condition = (FSMConditionBase)valueProperty.managedReferenceValue;


                NonUnityObjectField conditionObjectField = new();
                conditionObjectField.BindLabel("_name", new(child));
                conditionObjectField.SelectingObject += (evt) => OnConditionObjectFieldSelectingObject(evt, child);
                FSMComponentAttribute fsmAttribute = condition.GetType().GetCustomAttribute<FSMComponentAttribute>();
                conditionObjectField.SetObjectName(fsmAttribute == null ? condition.GetType().Name : fsmAttribute.Name);

                PropertyField valuePropertyField = new(valueProperty);
                valuePropertyField.RegisterCallback<GeometryChangedEvent>(OnTransitionPropertyFieldGeometryChanged);

                VisualElement transitionContainer = new() { name = "transition-container" };
                transitionContainer.Add(conditionObjectField);
                transitionContainer.Add(valuePropertyField);
                transitionsFoldout.Add(transitionContainer);
            }
            container.Add(transitionsFoldout);
        }

        extensionContainer.Add(container);
        RefreshExpandedState();
    }

    private void OpenSearchWindow(MouseDownEvent evt)
    {
        SearchWindow.Open(new(GUIUtility.GUIToScreenPoint(evt.mousePosition)), _searchProvider);
    }

    private void CreateCondition(Type type)
    {
        _state.AddCondition(_stateInEdit, (FSMConditionBase)Activator.CreateInstance(type));
        UpdateExtensionContainer();
    }

    private void OnConditionObjectFieldSelectingObject(MouseDownEvent evt, FSMStateSO child)
    {
        _stateInEdit = child;
        OpenSearchWindow(evt);
    }

    private void OnTransitionPropertyFieldGeometryChanged(GeometryChangedEvent evt)
    {
        PropertyField propertyField = (evt.currentTarget as PropertyField);
        if (propertyField.Q<Foldout>() == null)
        {
            propertyField.RemoveFromHierarchy();
        }
    }
    #endregion
}