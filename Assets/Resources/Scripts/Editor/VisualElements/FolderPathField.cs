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
    private string _title;
    private string _startFolder;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public string Title
    {
        get
        {
            return _title;
        }

        set
        {
            _title = value;
        }
    }

    public string StartFolder
    {
        get
        {
            return _startFolder;
        }

        set
        {
            _startFolder = value;
        }
    }
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
            string startFolder = string.IsNullOrEmpty(_pathTextField.value) ? _startFolder : _pathTextField.value;
            string absolutePath = EditorUtility.OpenFolderPanel(_title, startFolder, "");
            if (absolutePath.Contains(Application.dataPath))
            {
                string relativePath = "Assets" + absolutePath.Substring(Application.dataPath.Length) + '/';
                _pathTextField.value = relativePath;
            }
        };
    }
    #endregion
}