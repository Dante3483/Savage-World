using SavageWorld.Runtime.Attributes;
using SavageWorld.Runtime.Utilities.FiniteStateMachine;
using SavageWorld.Runtime.Utilities.FiniteStateMachine.Conditions;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class FiniteStateMachineNodeView : CustomNode<State>
{
    #region Fields
    private FiniteStateMachineSearchProvider _searchProvider;
    private State _stateInSearchWindow;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public FiniteStateMachineNodeView() : base()
    {

    }

    public FiniteStateMachineNodeView(State state) : base(state)
    {

    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Undo.RecordObject(_data, "SetPosition");
        _data.Position = newPos.min;
        EditorUtility.SetDirty(_data);
    }
    #endregion

    #region Private Methods
    private void InitializeSearchWindow()
    {
        _searchProvider = ScriptableObject.CreateInstance<FiniteStateMachineSearchProvider>();
        _searchProvider.Name = "Conditions";
        _searchProvider.Type = typeof(TransitionConditionBase);
        _searchProvider.OnSelect = ChangeCondition;
    }

    private void ChangeCondition(Type type)
    {
        _data.ChangeCondition(_stateInSearchWindow, (TransitionConditionBase)Activator.CreateInstance(type));
        UpdateExtensionContainer();
    }

    private void OpenSearchWindow(MouseDownEvent evt)
    {
        SearchWindow.Open(new(GUIUtility.GUIToScreenPoint(evt.mousePosition)), _searchProvider);
    }

    protected override void Initialize(State data)
    {
        base.Initialize(data);
        InitializeSearchWindow();
        viewDataKey = data.Guid;
        style.left = data.Position.x;
        style.top = data.Position.y;
    }

    protected override void InitializeTitleContainer()
    {
        base.InitializeTitleContainer();
        TextField titleTextField = new()
        {
            bindingPath = "_name",
            name = "state-name"
        };
        titleContainer.Insert(0, titleTextField);
    }

    protected override void InitializeExtensionContainer()
    {
        base.InitializeExtensionContainer();
        if (_data == null)
        {
            return;
        }
        VisualElement container = new();
        if (_data.ListOfTransitions.Count > 0)
        {
            Foldout transitionsFoldout = new()
            {
                text = "Transitions",
                value = false
            };
            SerializedProperty listOfTransitionsProperty = DataSerializedObject.FindProperty("_listOfTransitions");
            for (int i = 0; i < listOfTransitionsProperty.arraySize; i++)
            {
                SerializedProperty arrayElement = listOfTransitionsProperty.GetArrayElementAtIndex(i);
                SerializedProperty stateProperty = arrayElement.FindPropertyRelative("_state");
                SerializedProperty conditionProperty = arrayElement.FindPropertyRelative("_condition");
                State state = (State)stateProperty.objectReferenceValue;
                TransitionConditionBase condition = (TransitionConditionBase)conditionProperty.managedReferenceValue;

                NonUnityObjectField conditionObjectField = new();
                conditionObjectField.BindLabel("_name", new(state));
                conditionObjectField.SelectingObject += (evt) => ConditionObjectFieldSelectingObjectHandle(evt, state);
                FSMComponentAttribute fsmAttribute = condition.GetType().GetCustomAttribute<FSMComponentAttribute>();
                conditionObjectField.SetObjectName(fsmAttribute == null ? condition.GetType().Name : fsmAttribute.Name);

                PropertyField conditionPropertyField = new(conditionProperty);
                conditionPropertyField.RegisterCallback<GeometryChangedEvent>(ConditionPropertyFieldGeometryChangedEventHandle);

                VisualElement transitionContainer = new() { name = "transition-container" };
                transitionContainer.Add(conditionObjectField);
                transitionContainer.Add(conditionPropertyField);
                transitionsFoldout.Add(transitionContainer);
            }
            container.Add(transitionsFoldout);
        }
        extensionContainer.Add(container);
        RefreshExpandedState();
    }

    private void ConditionObjectFieldSelectingObjectHandle(MouseDownEvent evt, State state)
    {
        _stateInSearchWindow = state;
        OpenSearchWindow(evt);
    }

    private void ConditionPropertyFieldGeometryChangedEventHandle(GeometryChangedEvent evt)
    {
        PropertyField propertyField = (evt.currentTarget as PropertyField);
        if (propertyField.Q<Foldout>() == null)
        {
            propertyField.RemoveFromHierarchy();
        }
    }
    #endregion
}