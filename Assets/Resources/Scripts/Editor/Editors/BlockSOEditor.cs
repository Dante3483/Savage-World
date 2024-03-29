using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(BlockSO), true)]
[CanEditMultipleObjects]
public class BlockSOEditor : NewEditor
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
        _sprites = serializedObject.FindProperty("_sprites");
        _colorOnMap = serializedObject.FindProperty("_colorOnMap");
    }

    public override void InitializeEditor()
    {
        _root = new VisualElement();

        _spritesPropertyField = new PropertyField(_sprites);
        _colorOnMapPropertyField = new PropertyField(_colorOnMap);
    }
    #endregion
}
