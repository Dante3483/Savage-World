using SavageWorld.Runtime.Utilities.FSM;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
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
    //private static readonly string _styleResource = StaticInfo.StyleSheetsDirectory + "";
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
        //styleSheets.Add(Resources.Load<StyleSheet>(_styleResource));
        GridBackground gridBackground = new();
        gridBackground.name = "grid-background";
        Insert(0, gridBackground);

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
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
            CreateNode();
        }
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        if (evt.target is FSMGraphView)
        {
            evt.menu.AppendAction("Add state", (action) => CreateNode());
        }
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
    }
    #endregion

    #region Private Methods
    private void CreateNode()
    {
        FSMStateSO state = _finiteStateMachine.CreateState();
        CreateStateView(state);
    }

    private void CreateStateView(FSMStateSO state)
    {
        FSMStateView stateView = new(state);
        stateView.StateSelected = StateSelected;
        AddElement(stateView);
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
                FSMStateView stateView = element as FSMStateView;
                if (stateView != null)
                {
                    _finiteStateMachine.DeleteState(stateView.State);
                }

                Edge edge = element as Edge;
                if (edge != null)
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