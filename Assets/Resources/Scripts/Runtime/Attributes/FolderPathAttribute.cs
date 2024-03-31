using System;
using UnityEngine;

[System.AttributeUsage(AttributeTargets.Field)]
public class FolderPathAttribute : PropertyAttribute
{
    #region Private fields
    private string _label;
    private string _title;
    private string _startFolder;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    public string Label
    {
        get
        {
            return _label;
        }
    }

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
    }
    #endregion

    #region Methods
    public FolderPathAttribute(string label = "", string title = "", string startFolder = "Assets/")
    {
        _label = label;
        _title = title;
        _startFolder = startFolder;
    }
    #endregion
}
