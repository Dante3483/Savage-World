using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class TwoPaneEditorWindow : EditorWindow
{
    #region Private fields
    protected Toolbar _toolbar;
    protected TwoPaneSplitView _splitView;
    protected VisualElement _leftPane;
    protected VisualElement _rightPane;
    #endregion

    #region Public fields

    #endregion

    #region Properties
    protected VisualElement _root => rootVisualElement;
    #endregion

    #region Methods
    private void CreateGUI()
    {
        InitializeEditorWindow();
        Compose();
    }

    public virtual void InitializeEditorWindow()
    {
        _toolbar = new Toolbar();
        _splitView = new TwoPaneSplitView(0, 200, TwoPaneSplitViewOrientation.Horizontal);
        _leftPane = new VisualElement();
        _rightPane = new VisualElement();
    }

    private void Compose()
    {
        ComposeToolbar();
        ComposeSplitView();
        _root.Add(_toolbar);
        _root.Add(_splitView);
    }

    private void ComposeSplitView()
    {
        ComposeLeftPane();
        ComposeRightPane();
        _splitView.Add(_leftPane);
        _splitView.Add(_rightPane);
    }

    protected void HideToolbar()
    {
        _toolbar.style.display = DisplayStyle.None;
    }

    protected void ShowToolbar()
    {
        _toolbar.style.display = DisplayStyle.Flex;
    }

    protected void HideLeftPane()
    {
        _leftPane.style.display = DisplayStyle.None;
    }

    protected void ShowLeftPane()
    {
        _leftPane.style.display = DisplayStyle.Flex;
    }

    protected void HideRightPane()
    {
        _rightPane.style.display = DisplayStyle.None;
    }

    protected void ShowRightPane()
    {
        _rightPane.style.display = DisplayStyle.Flex;
    }

    public virtual void ComposeToolbar()
    {

    }

    public virtual void ComposeLeftPane()
    {

    }

    public virtual void ComposeRightPane()
    {

    }
    #endregion
}
