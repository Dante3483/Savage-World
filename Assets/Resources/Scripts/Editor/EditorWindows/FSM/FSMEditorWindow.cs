using SavageWorld.Runtime.Utilities.FSM;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class FSMEditorWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset _visualTreeAsset;
    private FSMGraphView _fsmGraphView;
    private FSMInspectorView _fsmInspectorView;

    [MenuItem("Utils/FSM")]
    public static void OpenWindow()
    {
        FSMEditorWindow wnd = GetWindow<FSMEditorWindow>();
        wnd.titleContent = new GUIContent("FSMEditorWindow");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        _visualTreeAsset.CloneTree(root);
        _fsmGraphView = root.Q<FSMGraphView>();
        _fsmInspectorView = root.Q<FSMInspectorView>();
        _fsmGraphView.StateSelected = OnStateSelectionChanged;
        OnSelectionChange();
    }

    private void OnSelectionChange()
    {
        FiniteStateMachineSO finiteStateMachine = Selection.activeObject as FiniteStateMachineSO;
        if (finiteStateMachine)
        {
            _fsmGraphView.PopulateView(finiteStateMachine);
        }
    }

    private void OnStateSelectionChanged(StateView stateView)
    {
        _fsmInspectorView.UpdateSelection(stateView);
    }
}
