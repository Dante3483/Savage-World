using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class FolderPathField : ObjectPathField
{
    public new class UxmlFactory : UxmlFactory<FolderPathField, UxmlTraits>
    {

    }

    public new class UxmlTraits : ObjectPathField.UxmlTraits
    {

    }

    #region Private fields

    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public FolderPathField() : this(null)
    {

    }

    public FolderPathField(SerializedProperty property) : this(property, null)
    {
    }


    public FolderPathField(SerializedProperty property, string label) : base(property, label)
    {
        Texture icon = EditorGUIUtility.IconContent("d_Folder Icon").image;
        _openPanelButton.style.backgroundImage = (StyleBackground)icon;
        _openPanelButton.clicked += () =>
        {
            string path = EditorUtility.OpenFolderPanel("", "", "");
            _pathTextField.value = path;
        };
    }
    #endregion
}