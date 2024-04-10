using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ResearchEditorWindow : EditorWindow
{
    #region Private fields
    [SerializeField] private VisualTreeAsset _visualTreeAsset;

    private VisualElement _root;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    [MenuItem("Utils/Research")]
    public static void ShowWindow()
    {
        ResearchEditorWindow wnd = GetWindow<ResearchEditorWindow>();
        wnd.titleContent = new GUIContent("Research");
        wnd.minSize = new Vector2(700, 500);
    }

    public void CreateGUI()
    {
        _root = rootVisualElement;
        _visualTreeAsset.CloneTree(_root);
    }
    #endregion
}
