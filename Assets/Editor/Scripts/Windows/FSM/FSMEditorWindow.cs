using SavageWorld.Runtime.Utilities.FSM;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace SavageWorld.Editor.Windows.FSM
{
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

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is FSMDataSO)
            {
                OpenWindow();
                return true;
            }
            return false;
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            _visualTreeAsset.CloneTree(root);
            _fsmGraphView = root.Q<FSMGraphView>();
            _fsmInspectorView = root.Q<FSMInspectorView>();
            _fsmGraphView.StateSelected = OnStateSelectionChanged;
            _fsmGraphView.EdgeSelected = OnEdgeSelectionChanged;
            OnSelectionChange();
        }

        private void OnSelectionChange()
        {
            FSMDataSO finiteStateMachine = Selection.activeObject as FSMDataSO;
            if (finiteStateMachine)
            {
                _fsmGraphView.PopulateView(finiteStateMachine);
            }
        }

        private void OnStateSelectionChanged(FSMStateView stateView)
        {
            _fsmInspectorView.UpdateSelection(stateView);
        }

        private void OnEdgeSelectionChanged(FSMEdge edge)
        {
            _fsmInspectorView.UpdateSelection(edge);
        }
    }
}