using SavageWorld.Runtime.Attributes;
using SavageWorld.Runtime.Utilities;
using SavageWorld.Runtime.Utilities.FSM;
using SavageWorld.Runtime.Utilities.FSM.Conditions;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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
        _state = state;
        _searchProvider = ScriptableObject.CreateInstance<FSMSearchProvider>();
        _searchProvider.Name = "Conditions";
        _searchProvider.Type = typeof(FSMConditionBase);
        _searchProvider.OnSelect = CreateCondition;
        FillContent();
        SetTitleOfNode(_state.Name);
        SetGuidOfNode(_state.Guid);
        SetPositionOfNode(_state.Position);
    }

    public void UpdateTitleContainer()
    {
        titleContainer.Clear();
        FillTitleContainer();
    }

    public void UpdateInputContainer()
    {
        inputContainer.Clear();
        FillOutputContainer();
    }

    public void UpdateOutputContainer()
    {
        outputContainer.Clear();
        FillOutputContainer();
    }

    public void UpdateExtensionContainer()
    {
        extensionContainer.Clear();
        FillExtensionContainer();
    }

    public void SetTitleOfNode(string value)
    {
        if (title != value)
        {
            foreach (FSMEdge edge in _inputPort.connections)
            {
                FSMStateNode parent = edge.output.node as FSMStateNode;
                parent.UpdateExtensionContainer();
            }
        }
        title = value;
    }

    public void SetGuidOfNode(string value)
    {
        viewDataKey = value;
    }

    public void SetPositionOfNode(Vector2 value)
    {
        Undo.RecordObject(_state, "FSM (Set Position)");
        style.left = value.x;
        style.top = value.y;
        EditorUtility.SetDirty(_state);
    }

    public void SetPositionOfState(Vector2 value)
    {
        Undo.RecordObject(_state, "FSM (Set Position)");
        _state.Position = value;
        EditorUtility.SetDirty(_state);
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        SetPositionOfState(newPos.min);
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
    private void FillContent()
    {
        FillTitleContainer();
        FillInputContainer();
        FillOutputContainer();
        FillExtensionContainer();
    }

    private void FillTitleContainer()
    {

    }

    private void FillInputContainer()
    {
        _inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
        _inputPort.portName = "";
        inputContainer.Add(_inputPort);
    }

    private void FillOutputContainer()
    {
        _outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
        _outputPort.portName = "";
        outputContainer.Add(_outputPort);
    }

    private void FillExtensionContainer()
    {
        if (_state == null)
        {
            return;
        }
        VisualElement container = new();
        foreach (var kvp in _state.ConditionByChild)
        {
            FSMStateSO child = kvp.Key;
            FSMConditionBase condition = kvp.Value;
            NonUnityObjectField conditionObjectField = new(child.Name);
            conditionObjectField.SelectingObject += (evt) =>
            {
                _stateInEdit = child;
                ShowSearchWindow(evt);
            };

            Type conditionType = condition.GetType();
            FSMComponentAttribute fsmAttribute = conditionType.GetCustomAttribute<FSMComponentAttribute>();
            if (fsmAttribute != null)
            {
                conditionObjectField.SetObjectName(fsmAttribute.Name);
            }
            container.Add(conditionObjectField);
        }
        extensionContainer.Add(container);
        RefreshExpandedState();
    }

    private void ShowSearchWindow(MouseDownEvent evt)
    {
        SearchWindow.Open(new(GUIUtility.GUIToScreenPoint(evt.mousePosition)), _searchProvider);
    }

    private void CreateCondition(Type type)
    {
        _state.AddCondition(_stateInEdit, (FSMConditionBase)Activator.CreateInstance(type));
        UpdateExtensionContainer();
    }
    #endregion
}