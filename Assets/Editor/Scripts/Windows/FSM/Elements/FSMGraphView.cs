using SavageWorld.Runtime.Utilities.FSM;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class FSMGraphView : GraphView
{
    public new class UxmlFactory : UxmlFactory<FSMGraphView, UxmlTraits>
    {

    }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {

    }

    #region Fields
    private FSMDataSO _finiteStateMachine;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates
    public Action<FSMStateView> StateSelected;
    public Action<FSMEdge> EdgeSelected;
    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public FSMGraphView() : base()
    {
        GridBackground gridBackground = new()
        {
            name = "grid-background"
        };

        RegisterCallback<MouseDownEvent>(CheckDoubleClick);
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        Insert(0, gridBackground);
    }

    public void PopulateView(FSMDataSO finiteStateMachine)
    {
        _finiteStateMachine = finiteStateMachine;
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;
        _finiteStateMachine.ListOfStates.ForEach(state => CreateStateView(state));
        _finiteStateMachine.ListOfStates.ForEach(state => CreateEdges(state));
        if (_finiteStateMachine.RootState == null)
        {
            CreateState();
        }
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {

    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
    }
    #endregion

    #region Private Methods
    private void CheckDoubleClick(MouseDownEvent evt)
    {
        if (evt.button == (int)MouseButton.LeftMouse && evt.clickCount == 2)
        {
            CreateState(contentViewContainer.WorldToLocal(evt.mousePosition));
            evt.StopPropagation();
        }
    }

    private void CreateState()
    {
        CreateState(new(0, 0));
    }

    private void CreateState(Vector2 position)
    {
        FSMStateSO state = _finiteStateMachine.CreateState();
        state.Position = position;
        ClearSelection();
        AddToSelection(CreateStateView(state));
    }

    private FSMStateView CreateStateView(FSMStateSO state)
    {
        FSMStateView stateView = new(state);
        stateView.StateSelected = StateSelected;
        AddElement(stateView);
        return stateView;
    }

    private void CreateEdges(FSMStateSO state)
    {
        state.ListOfChildren.ForEach(child =>
        {
            FSMStateView parentView = FindStateView(state);
            FSMStateView childView = FindStateView(child);
            FSMEdge edge = parentView.Output.ConnectTo<FSMEdge>(childView.Input);
            edge.EdgeSelected = EdgeSelected;
            AddElement(edge);
        });
    }

    private FSMStateView FindStateView(FSMStateSO state)
    {
        return GetNodeByGuid(state.Guid) as FSMStateView;
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach(element =>
            {
                if (element is FSMStateView stateView)
                {
                    _finiteStateMachine.DeleteState(stateView.State);
                }

                if (element is Edge edge)
                {
                    FSMStateView parentView = edge.output.node as FSMStateView;
                    FSMStateView childView = edge.input.node as FSMStateView;
                    _finiteStateMachine.RemoveChild(parentView.State, childView.State);
                }
            });
        }

        if (graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(edge =>
            {
                FSMEdge fsmEdge = edge as FSMEdge;
                fsmEdge.EdgeSelected = EdgeSelected;
                FSMStateView parentView = edge.output.node as FSMStateView;
                FSMStateView childView = edge.input.node as FSMStateView;
                _finiteStateMachine.AddChild(parentView.State, childView.State);
            });
        }
        return graphViewChange;
    }
    #endregion
}