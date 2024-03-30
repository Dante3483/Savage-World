using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class CustomEditor : Editor
{
    #region Private fields
    protected VisualElement _root;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public override VisualElement CreateInspectorGUI()
    {

        _root = new VisualElement();
        FindSerializedProperties();
        InitializeEditorElements();
        Compose();
        return _root;
    }

    public abstract void Compose();

    public abstract void FindSerializedProperties();

    public abstract void InitializeEditorElements();
    #endregion
}
