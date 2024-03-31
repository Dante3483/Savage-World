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
        FolderPathAttribute folderPathAttribute = attribute as FolderPathAttribute;
        FolderPathField folderPathField = new FolderPathField(property);
        folderPathField.label = string.IsNullOrEmpty(folderPathAttribute.Label) ? property.displayName : folderPathAttribute.Label;
        folderPathField.Title = folderPathAttribute.Title;
        folderPathField.StartFolder = folderPathAttribute.StartFolder;
        return folderPathField;
    }
    #endregion
}
