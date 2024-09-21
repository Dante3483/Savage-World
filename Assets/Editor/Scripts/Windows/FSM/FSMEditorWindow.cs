using SavageWorld.Runtime.Utilities.FSM;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace SavageWorld.Editor.Windows.FSM
{
    public class FSMEditorWindow : EditorWindow
    {
        #region Fields
        [SerializeField]
        private VisualTreeAsset _visualTreeAsset;
        private static FSMGraphView _fsmGraphView;
        private static FSMInspectorView _fsmInspectorView;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        [MenuItem("SavageWorld/FSM")]
        public static FSMEditorWindow OpenWindow()
        {
            return GetWindow<FSMEditorWindow>("FSM");
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is FSMDataSO fsm)
            {
                FSMEditorWindow window = OpenWindow();
                window.titleContent = new(fsm.name);
                _fsmGraphView.PopulateView(fsm);
                return true;
            }
            return false;
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            _visualTreeAsset.CloneTree(root);
            SetUpGraphView(root);
            SetUpInspectorView(root);
        }
        #endregion

        #region Private Methods
        private void SetUpGraphView(VisualElement root)
        {
            _fsmGraphView = root.Q<FSMGraphView>();
            _fsmGraphView.StateSelected = OnStateSelectionChanged;
            _fsmGraphView.EdgeSelected = OnEdgeSelectionChanged;
        }

        private void SetUpInspectorView(VisualElement root)
        {
            _fsmInspectorView = root.Q<FSMInspectorView>();
        }

        private void OnStateSelectionChanged(FSMStateView stateView)
        {
            _fsmInspectorView.UpdateSelection(stateView);
        }

        private void OnEdgeSelectionChanged(FSMEdge edge)
        {
            _fsmInspectorView.UpdateSelection(edge);
        }
        #endregion
    }
}