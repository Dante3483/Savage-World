using SavageWorld.Runtime.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectPreview : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ObjectPreview, UxmlTraits>
    {

    }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {

    }

    #region Private fields
    private static readonly string _styleName = "ObjectPreview";
    private static readonly string _stylePath = StaticParameters.StylesPath + _styleName + StaticParameters.StyleExtension;
    private static readonly string _ussObjectPreview = "object-preview";

    private Image _preview;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public ObjectPreview() : base()
    {
        styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(_stylePath));
        AddToClassList(_ussObjectPreview);

        _preview = new Image();
        Add(_preview);
    }

    public void SetSprite(Sprite preview)
    {
        _preview.sprite = preview;
    }

    public void SetTexture(Texture preview)
    {
        _preview.image = preview;
    }
    #endregion
}