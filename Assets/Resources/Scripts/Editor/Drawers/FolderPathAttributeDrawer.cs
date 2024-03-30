using UnityEditor;
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
        VisualElement _root = new VisualElement();

        FolderPathField folderPathField = new FolderPathField(property);

        _root.Add(folderPathField);
        return _root;
    }
    #endregion
}
