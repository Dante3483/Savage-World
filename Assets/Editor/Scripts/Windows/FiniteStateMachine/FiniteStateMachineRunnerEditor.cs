using SavageWorld.Editor.Editors;
using SavageWorld.Runtime.Utilities.FiniteStateMachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(FiniteStateMachineRunner), true)]
public class FiniteStateMachineRunnerEditor : ObjectsEditor
{
    #region Fields
    [SerializeField]
    private VisualTreeAsset _visualTreeAsset;
    private SerializedProperty _finiteStateMachineProperty;
    private FiniteStateMachine _finiteStateMachine;
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
        _visualTreeAsset.CloneTree(_root);
        Button openBtn = _root.Q<Button>("open-btn");
        openBtn.clicked += () =>
        {
            _finiteStateMachine = (FiniteStateMachine)_finiteStateMachineProperty.objectReferenceValue;
            FiniteStateMachineEditorWindow.Open(_finiteStateMachine);
        };
    }

    public override void FindSerializedProperties()
    {
        _finiteStateMachineProperty = serializedObject.FindProperty("_finiteStateMachine");
    }

    public override void InitializeEditorElements()
    {

    }
    #endregion

    #region Private Methods

    #endregion
}
