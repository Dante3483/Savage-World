using SavageWorld.Runtime.Utilities.FSM;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class FSMStateView : Node
{
    public new class UxmlFactory : UxmlFactory<FSMStateView, UxmlTraits>
    {

    }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {

    }

    #region Fields
    //private static readonly string _styleResource = StaticInfo.StyleSheetsDirectory + "";
    private FSMStateSO _state;
    private Port _input;
    private Port _output;
    #endregion

    #region Properties
    public FSMStateSO State
    {
        get
        {
            return _state;
        }
    }

    public Port Input
    {
        get
        {
            return _input;
        }

        set
        {
            _input = value;
        }
    }

    public Port Output
    {
        get
        {
            return _output;
        }

        set
        {
            _output = value;
        }
    }
    #endregion

    #region Events / Delegates
    public Action<FSMStateView> StateSelected;
    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public FSMStateView() : this(null)
    {

    }

    public FSMStateView(FSMStateSO state) : base()
    {
        //styleSheets.Add(Resources.Load<StyleSheet>(_styleResource));
        _state = state;
        title = state.name;
        viewDataKey = state.Guid;
        state.PositionChanged = OnPositionValueChanged;
        style.left = state.Position.x;
        style.top = state.Position.y;
        CreateInputPorts();
        CreateOutputPorts();
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        _state.Position.x = newPos.xMin;
        _state.Position.y = newPos.yMin;
    }

    public override void OnSelected()
    {
        base.OnSelected();
        StateSelected?.Invoke(this);
    }

    public override Port InstantiatePort(Orientation orientation, Direction direction, Port.Capacity capacity, Type type)
    {
        return Port.Create<FSMEdge>(orientation, direction, capacity, type);
    }
    #endregion

    #region Private Methods
    private void CreateInputPorts()
    {
        _input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
        if (_input != null)
        {
            _input.portName = "";
            inputContainer.Add(_input);
        }
    }

    private void CreateOutputPorts()
    {
        _output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
        if (_output != null)
        {
            _output.portName = "";
            outputContainer.Add(_output);
        }
    }

    private void OnPositionValueChanged(Vector2 position)
    {
        style.left = position.x;
        style.top = position.y;
    }
    #endregion
}