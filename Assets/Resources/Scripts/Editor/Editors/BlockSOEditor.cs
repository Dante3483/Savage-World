using UnityEditor;
using UnityEditor.UIElements;

//[CustomEditor(typeof(BlockSO), true)]
[CanEditMultipleObjects]
public class BlockSOEditor : ObjectsCustomEditor
{
    #region Private fields
    private SerializedProperty _sprites;
    private SerializedProperty _colorOnMap;

    private PropertyField _spritesPropertyField;
    private PropertyField _colorOnMapPropertyField;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public override void Compose()
    {
        //_root = base.CreateInspectorGUI();
        _root.Add(_spritesPropertyField);
        _root.Add(_colorOnMapPropertyField);
    }

    public override void FindSerializedProperties()
    {
        _sprites = serializedObject.FindProperty("_path");
        _colorOnMap = serializedObject.FindProperty("_colorOnMap");
    }

    public override void InitializeEditorElements()
    {
        _spritesPropertyField = new PropertyField(_sprites);
        _colorOnMapPropertyField = new PropertyField(_colorOnMap);
    }
    #endregion
}
