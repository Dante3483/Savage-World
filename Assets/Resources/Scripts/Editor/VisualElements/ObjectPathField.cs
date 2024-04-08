using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class ObjectPathField : BaseField<DefaultAsset>
{
    public new class UxmlTraits : BaseField<DefaultAsset>.UxmlTraits
    {

    }

    #region Private fields
    private static readonly string _styleResource = StaticInfo.StyleSheetsDirectory + "ObjectPathField";
    private static readonly string _ussObjectPathField = "object-path-field";
    private static readonly string _ussInput = _ussObjectPathField + "__input";
    private static readonly string _ussOpenPanel = _ussObjectPathField + "__open-panel";
    private static readonly string _ussPath = _ussObjectPathField + "__path";

    protected VisualElement _visualInput;
    protected TextField _pathTextField;
    protected Button _openPanelButton;
    private SerializedProperty _property;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public ObjectPathField(SerializedProperty property, string label) : base(label, null)
    {
        styleSheets.Add(Resources.Load<StyleSheet>(_styleResource));
        AddToClassList(_ussObjectPathField);

        _pathTextField = new TextField();
        _pathTextField.AddToClassList(_ussPath);

        _openPanelButton = new Button();
        _openPanelButton.AddToClassList(_ussOpenPanel);

        _visualInput = this.Q<VisualElement>(className: inputUssClassName);
        _visualInput.AddToClassList(_ussInput);
        _visualInput.Add(_pathTextField);
        _visualInput.Add(_openPanelButton);

        if (property != null)
        {
            _pathTextField.bindingPath = property.propertyPath;
        }
    }
    #endregion
}