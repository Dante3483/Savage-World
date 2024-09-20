using SavageWorld.Runtime.Utilities.FSM;
using SavageWorld.Runtime.Utilities.FSM.Conditions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class FSMInspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<FSMInspectorView, UxmlTraits>
    {

    }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {

    }

    #region Fields

    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public FSMInspectorView() : base()
    {

    }

    public void UpdateSelection(FSMStateView stateView)
    {
        Clear();
        SerializedObject serializedObject = new(stateView.State);

        PropertyField namePropertyField = new(serializedObject.FindProperty("_name"));
        PropertyField positionPropertyField = new(serializedObject.FindProperty("_position"));
        PropertyField guidPropertyField = new(serializedObject.FindProperty("_guid"));

        namePropertyField.RegisterCallback<SerializedPropertyChangeEvent>(evt => stateView.SetTitleOfNode(evt.changedProperty.stringValue));
        positionPropertyField.RegisterCallback<SerializedPropertyChangeEvent>(evt => stateView.SetPositionOfNode(evt.changedProperty.vector2Value));
        guidPropertyField.RegisterCallback<SerializedPropertyChangeEvent>(evt => stateView.SetGuidOfNode(evt.changedProperty.stringValue));

        Add(namePropertyField);
        Add(positionPropertyField);
        Add(guidPropertyField);
        Add(new FSMActionView(serializedObject.FindProperty("_listOfActionsOnEnter"), stateView.State.ListOfActionsOnEnter));
        Add(new FSMActionView(serializedObject.FindProperty("_listOfActionsOnExit"), stateView.State.ListOfActionsOnExit));
        Add(new FSMActionView(serializedObject.FindProperty("_listOfActionsOnFixedUpdate"), stateView.State.ListOfActionsOnFixedUpdate));
        Add(new FSMActionView(serializedObject.FindProperty("_listOfActionsOnUpdate"), stateView.State.ListOfActionsOnUpdate));
        this.Bind(new(stateView.State));
    }

    public void UpdateSelection(FSMEdge edge)
    {
        Clear();
        FSMStateView parentStateView = edge.output.node as FSMStateView;
        FSMStateView childStateView = edge.input.node as FSMStateView;
        FSMStateSO state = parentStateView.State;
        state.ConditionByChild.TryGetValue(childStateView.State, out FSMConditionBase condition);
        string labelText = condition == null ? "No condition" : condition.GetType().Name;
        Label label = new(labelText);
        Add(label);
    }
    #endregion

    #region Private Methods

    #endregion
}