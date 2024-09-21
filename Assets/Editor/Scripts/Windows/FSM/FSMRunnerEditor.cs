using SavageWorld.Editor.Editors;
using SavageWorld.Editor.Windows.FSM;
using SavageWorld.Runtime.Utilities.FSM;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(FSMRunner), true)]
[CanEditMultipleObjects]
public class FSMRunnerEditor : ObjectsEditor
{
    #region Fields
    [SerializeField]
    private VisualTreeAsset _editorTreeAsset;
    private Button _openBtn;
    private SerializedProperty _fsmProperty;
    private FSMDataSO _fsm;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public override void Compose()
    {
        _editorTreeAsset.CloneTree(_root);
        _openBtn = _root.Q<Button>("open-btn");
        _openBtn.clicked += () =>
        {
            _fsm = (FSMDataSO)_fsmProperty.objectReferenceValue;
            FSMEditorWindow.OpenWindow(_fsm);
        };
    }

    public override void FindSerializedProperties()
    {
        _fsmProperty = serializedObject.FindProperty("_fsm");
    }

    public override void InitializeEditorElements()
    {

    }
    #endregion

    #region Private Methods

    #endregion
}
