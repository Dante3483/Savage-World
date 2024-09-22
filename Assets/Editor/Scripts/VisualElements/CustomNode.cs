using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using Object = UnityEngine.Object;

public class CustomNode<T> : Node where T : Object
{
    #region Fields
    //private static readonly string _styleName = "";
    //private static readonly string _stylePath = StaticParameters.StylesPath + _styleName + StaticParameters.StyleExtension;
    protected T _data;
    protected Port _inputPort;
    protected Port _outputPort;
    private SerializedObject _dataSerializedObject;
    #endregion

    #region Properties
    public T Data
    {
        get
        {
            return _data;
        }
    }

    public Port InputPort
    {
        get
        {
            return _inputPort;
        }

        set
        {
            _inputPort = value;
        }
    }

    public Port OutputPort
    {
        get
        {
            return _outputPort;
        }

        set
        {
            _outputPort = value;
        }
    }

    public SerializedObject DataSerializedObject
    {
        get
        {
            if (_dataSerializedObject == null)
            {
                _dataSerializedObject = new(_data);
            }
            _dataSerializedObject.Update();
            return _dataSerializedObject;
        }
    }
    #endregion

    #region Events / Delegates
    public Action<CustomNode<T>> NodeSelected;
    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public CustomNode() : base()
    {

    }

    public CustomNode(T data) : base()
    {
        //styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(_stylePath));
        Initialize(data);
        Bind();
    }

    public void UpdateTitleContainer()
    {
        titleContainer.Clear();
        InitializeTitleContainer();
        Bind();
    }

    public void UpdateInputContainer()
    {
        inputContainer.Clear();
        InitializeOutputContainer();
        Bind();
    }

    public void UpdateOutputContainer()
    {
        outputContainer.Clear();
        InitializeOutputContainer();
        Bind();
    }

    public void UpdateExtensionContainer()
    {
        extensionContainer.Clear();
        InitializeExtensionContainer();
        Bind();
    }

    public override void OnSelected()
    {
        base.OnSelected();
        NodeSelected?.Invoke(this);
    }

    public override void OnUnselected()
    {
        base.OnUnselected();
        NodeSelected?.Invoke(null);
    }
    #endregion

    #region Private Methods
    protected virtual void Initialize(T data)
    {
        _data = data;
        _dataSerializedObject = new(_data);
        InitializeTitleContainer();
        InitializeInputContainer();
        InitializeOutputContainer();
        InitializeExtensionContainer();
    }

    protected virtual void InitializeTitleContainer()
    {

    }

    protected virtual void InitializeInputContainer()
    {
        _inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
        _inputPort.portName = "";
        inputContainer.Add(_inputPort);
    }

    protected virtual void InitializeOutputContainer()
    {
        _outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
        _outputPort.portName = "";
        outputContainer.Add(_outputPort);
    }

    protected virtual void InitializeExtensionContainer()
    {

    }

    protected void Bind()
    {
        this.Bind(_dataSerializedObject);
    }
    #endregion
}