using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(FolderPathAttribute))]
public class FolderPathAttributeDrawer : PropertyDrawer
{
    #region Private fields

    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        return new FolderPathField(property);
    }
    #endregion
}
