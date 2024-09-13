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
        Add(new PropertyField(serializedObject.FindProperty("_stateName")));
        Add(new PropertyField(serializedObject.FindProperty("_guid")));
        Add(new FSMActionView(serializedObject.FindProperty("_listOfActionOnEnter"), stateView.State.ListOfActionOnEnter));
        Add(new FSMActionView(serializedObject.FindProperty("_listOfActionOnExit"), stateView.State.ListOfActionOnExit));
        Add(new FSMActionView(serializedObject.FindProperty("_listOfActionOnFixedUpdate"), stateView.State.ListOfActionOnFixedUpdate));
        Add(new FSMActionView(serializedObject.FindProperty("_listOfActionOnUpdate"), stateView.State.ListOfActionOnUpdate));
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