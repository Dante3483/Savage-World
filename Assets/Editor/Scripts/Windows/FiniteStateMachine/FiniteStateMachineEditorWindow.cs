using SavageWorld.Runtime.Utilities.FiniteStateMachine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

public class FiniteStateMachineEditorWindow : EditorWindow
{
    #region Fields
    [SerializeField]
    private VisualTreeAsset _visualTreeAsset;
    private VisualElement _root;
    private FiniteStateMachineInspectorView _inspectorView;
    private FiniteStateMachineGraphView _graphView;
    private FiniteStateMachineBase _finiteStateMachine;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Public Methods
    [MenuItem("SavageWorld/Finite State Machine")]
    public static void Open()
    {
        Open(null);
    }

    [OnOpenAsset]
    public static bool Open(int instanceId, int line)
    {
        if (Selection.activeObject is FiniteStateMachineBase finiteStateMachine)
        {
            Open(finiteStateMachine);
            return true;
        }
        return false;
    }

    public static void Open(FiniteStateMachineBase finiteStateMachine)
    {
        string windowName = finiteStateMachine != null ? finiteStateMachine.name : "FiniteStateMachine";
        FiniteStateMachineEditorWindow window = GetWindow<FiniteStateMachineEditorWindow>(windowName);
        window._finiteStateMachine = finiteStateMachine;
        window.PopulateWindow();
    }

    public void CreateGUI()
    {
        _root = rootVisualElement;
        _visualTreeAsset.CloneTree(_root);
        _inspectorView = _root.Q<FiniteStateMachineInspectorView>();
        _graphView = _root.Q<FiniteStateMachineGraphView>();
    }
    #endregion

    #region Private Methods
    private void PopulateWindow()
    {
        _graphView.PopulateView(_finiteStateMachine);
    }
    #endregion
}
