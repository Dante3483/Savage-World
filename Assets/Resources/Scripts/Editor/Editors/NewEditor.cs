using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class NewEditor : Editor
{
    #region Private fields
    protected VisualElement _root;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("asdamsldkasmkasdmad");
        EditorGUILayout.EndHorizontal();
        base.OnInspectorGUI();
    }

    public override VisualElement CreateInspectorGUI()
    {
        FindSerializedProperties();
        InitializeEditor();
        Compose();
        return _root;
    }

    public abstract void Compose();

    public abstract void FindSerializedProperties();

    public abstract void InitializeEditor();
    #endregion
}
