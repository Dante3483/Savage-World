using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class FiniteStateMachineInspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<FiniteStateMachineInspectorView, UxmlTraits>
    {

    }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {

    }

    #region Fields
    private Editor _editor;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public FiniteStateMachineInspectorView() : base()
    {

    }

    public void UpdateSelection(FiniteStateMachineNodeView node)
    {
        Clear();
        if (node != null)
        {
            SerializedObject serializedObject = new(node.Data);
            Add(new FiniteStateMachineActionsView(serializedObject.FindProperty("_listOfEnterActions"), node.Data.ListOfEnterActions));
            Add(new FiniteStateMachineActionsView(serializedObject.FindProperty("_listOfExitActions"), node.Data.ListOfExitActions));
            Add(new FiniteStateMachineActionsView(serializedObject.FindProperty("_listOfFixedUpdateActions"), node.Data.ListOfFixedUpdateActions));
            Add(new FiniteStateMachineActionsView(serializedObject.FindProperty("_listOfUpdateActions"), node.Data.ListOfUpdateActions));
            this.Bind(new(node.Data));
        }
    }
    #endregion

    #region Private Methods

    #endregion
}