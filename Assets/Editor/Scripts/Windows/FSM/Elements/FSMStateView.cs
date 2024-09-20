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
        _state = state;
        CreateInputPorts();
        CreateOutputPorts();
        SetTitleOfNode(_state.Name);
        SetGuidOfNode(_state.Guid);
        SetPositionOfNode(_state.Position);
    }

    public void SetTitleOfNode(string value)
    {
        title = value;
    }

    public void SetGuidOfNode(string value)
    {
        viewDataKey = value;
    }

    public void SetPositionOfNode(Vector2 value)
    {
        style.left = value.x;
        style.top = value.y;
    }

    public void SetPositionOfState(Vector2 value)
    {
        _state.Position = value;
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        SetPositionOfState(newPos.min);
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
        SetPositionOfNode(position);
    }
    #endregion
}