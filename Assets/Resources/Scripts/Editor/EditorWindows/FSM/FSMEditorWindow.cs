using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class FSMEditorWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset _visualTreeAsset;

    [MenuItem("Utils/FSM")]
    public static void ShowExample()
    {
        FSMEditorWindow wnd = GetWindow<FSMEditorWindow>();
        wnd.titleContent = new GUIContent("FSMEditorWindow");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        _visualTreeAsset.CloneTree(root);
    }
}
