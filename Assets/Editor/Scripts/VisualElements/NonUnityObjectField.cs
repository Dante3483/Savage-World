using SavageWorld.Runtime.Utilities;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class NonUnityObjectField : VisualElement
{
    public new class UxmlFactory : UxmlFactory<NonUnityObjectField, UxmlTraits>
    {

    }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        private UxmlStringAttributeDescription _label = new()
        {
            name = "label",
            defaultValue = "Object Field"
        };

        private UxmlBoolAttributeDescription _isReadonly = new()
        {
            name = "readonly",
            defaultValue = false
        };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            if (ve is NonUnityObjectField objectField)
            {
                objectField.Label = _label.GetValueFromBag(bag, cc);
                objectField.IsReadonly = _isReadonly.GetValueFromBag(bag, cc);
            }
        }
    }

    #region Fields
    private static readonly string _styleName = "NonUnityObjectField";
    private static readonly string _stylePath = StaticParameters.StylesPath + _styleName + StaticParameters.StyleExtension;
    private static readonly string _ussClassName = "non-unity-object-field";
    private static readonly string _labelUssClassName = _ussClassName + "__label";
    private static readonly string _inputUssClassName = _ussClassName + "__input";
    private static readonly string _objectNameUssClassName = _ussClassName + "__object-name";
    private bool _isReadonly;
    private object _value;
    private Label _labelElement;
    private VisualElement _inputElement;
    private Label _objectName;
    #endregion

    #region Properties
    public string Label
    {
        get
        {
            return _labelElement.text;
        }

        set
        {
            if (_labelElement.text != value)
            {
                _labelElement.text = value;
                if (string.IsNullOrEmpty(_labelElement.text))
                {
                    _labelElement.RemoveFromHierarchy();
                }
                else if (!Contains(_labelElement))
                {
                    Insert(0, _labelElement);
                }
            }
        }
    }

    public bool IsReadonly
    {
        get
        {
            return _isReadonly;
        }

        set
        {
            _isReadonly = value;
        }
    }

    public object Value
    {
        get
        {
            return _value;
        }

        set
        {
            if (_value != value)
            {
                _objectName.text = value.GetType().Name;
            }
            _value = value;
        }
    }
    #endregion

    #region Events / Delegates
    public Action<MouseDownEvent> SelectingObject;
    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public NonUnityObjectField() : this("object")
    {

    }

    public NonUnityObjectField(string label) : base()
    {
        styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(_stylePath));
        AddToClassList(_ussClassName);

        _labelElement = new();
        _labelElement.name = "label";

        _inputElement = new();
        _inputElement.name = "input";
        _inputElement.AddToClassList(_inputUssClassName);
        _inputElement.RegisterCallback<MouseDownEvent>(OnInputClicked);

        _objectName = new("None (Object)");
        _objectName.name = "object-name";
        _objectName.AddToClassList(_objectNameUssClassName);
        _inputElement.Add(_objectName);

        Label = label;
        Add(_inputElement);
    }

    public void SetObjectName(string name)
    {
        _objectName.text = name;
    }

    public void BindLabel(string bindingPath, SerializedObject serializedObject)
    {
        _labelElement.bindingPath = bindingPath;
        _labelElement.Bind(serializedObject);
    }
    #endregion

    #region Private Methods
    private void OnInputClicked(MouseDownEvent evt)
    {
        if (evt.button == (int)MouseButton.LeftMouse)
        {
            SelectingObject?.Invoke(evt);
        }
    }
    #endregion
}