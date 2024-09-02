using UnityEditor;
using UnityEngine.UIElements;

public class FSMInspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<FSMInspectorView, UxmlTraits>
    {

    }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {

    }

    #region Fields
    //private static readonly string _styleResource = StaticInfo.StyleSheetsDirectory + "";
    private Editor _editor;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public FSMInspectorView() : base()
    {
        //styleSheets.Add(Resources.Load<StyleSheet>(_styleResource));
    }

    public void UpdateSelection(StateView stateView)
    {
        Clear();
        UnityEngine.Object.DestroyImmediate(_editor);
        _editor = Editor.CreateEditor(stateView.State);
        IMGUIContainer container = new(() => _editor.OnInspectorGUI());
        Add(container);
    }
    #endregion

    #region Private Methods

    #endregion
}