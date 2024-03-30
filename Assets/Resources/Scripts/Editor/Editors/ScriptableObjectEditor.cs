using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[UnityEditor.CustomEditor(typeof(ScriptableObject), true, isFallback = false)]
public abstract class ScriptableObjectEditor : Editor
{
    #region Private fields

    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        InspectorElement.FillDefaultInspector(root, serializedObject, this);
        return root;
    }
    #endregion
}
