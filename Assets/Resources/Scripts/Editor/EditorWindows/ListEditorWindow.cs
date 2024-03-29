using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class ListEditorWindow : TwoPaneEditorWindow
{
    #region Private fields
    protected new ListView _leftPane;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public override void InitializeEditorWindow()
    {
        base.InitializeEditorWindow();
        _leftPane = new ListView();
        _rightPane = new ListView();
    }
    #endregion
}
