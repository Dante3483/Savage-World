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
        private static FSMEditorWindow _window;
        private static VisualElement _root;
        private static FSMGraphView _fsmGraphView;
        private static FSMInspectorView _fsmInspectorView;
        private static FSMDataSO _fsm;
        #endregion

        #region Properties

        #endregion

        #region Events / Delegates

        #endregion

        #region Public Methods
        [MenuItem("SavageWorld/FSM")]
        public static void OpenWindow()
        {
            _fsm = null;
            _window = GetWindow<FSMEditorWindow>("FSM");
        }

        public static void OpenWindow(FSMDataSO fsm)
        {
            if (fsm == null)
            {
                return;
            }
            _fsm = fsm;
            _window = GetWindow<FSMEditorWindow>("FSM");
            _window.ResetWindow();
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is FSMDataSO fsm)
            {
                OpenWindow(fsm);
                return true;
            }
            return false;
        }

        public void CreateGUI()
        {
            _root = rootVisualElement;
            _visualTreeAsset.CloneTree(_root);
            ResetContent();
        }
        #endregion

        #region Private Methods
        private void ResetWindow()
        {
            _window.titleContent = new(_fsm.name);
            ResetContent();
        }

        private void ResetContent()
        {
            SetUpGraphView();
            SetUpInspectorView();
        }

        private void SetUpGraphView()
        {
            _fsmGraphView = _root.Q<FSMGraphView>();
            _fsmGraphView.PopulateView(_fsm);
            _fsmGraphView.StateSelected = OnStateSelectionChanged;
        }

        private void SetUpInspectorView()
        {
            _fsmInspectorView = _root.Q<FSMInspectorView>();
        }

        private void OnStateSelectionChanged(FSMStateNode stateView)
        {
            _fsmInspectorView.UpdateSelection(stateView);
        }
        #endregion
    }
}