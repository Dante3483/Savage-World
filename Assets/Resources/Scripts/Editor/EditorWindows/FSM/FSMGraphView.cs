using SavageWorld.Runtime;
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
    private FiniteStateMachineSO _finiteStateMachine;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates
    public Action<StateView> StateSelected;
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

    public void PopulateView(FiniteStateMachineSO finiteStateMachine)
    {
        _finiteStateMachine = finiteStateMachine;
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;
        _finiteStateMachine.ListOfStates.ForEach(state => CreateStateView(state));
        _finiteStateMachine.ListOfStates.ForEach(state => CreateEdges(state));
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction("Add state", (action) => CreateNode());
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
    }
    #endregion

    #region Private Methods
    private void CreateNode()
    {
        StateSO state = _finiteStateMachine.CreateState();
        CreateStateView(state);
    }

    private void CreateStateView(StateSO state)
    {
        StateView stateView = new(state);
        stateView.StateSelected = StateSelected;
        AddElement(stateView);
    }

    private void CreateEdges(StateSO state)
    {
        state.ListOfChildren.ForEach(child =>
        {
            StateView parentView = FindStateView(state);
            StateView childView = FindStateView(child);
            Edge edge = parentView.Output.ConnectTo(childView.Input);
            AddElement(edge);
        });
    }

    private StateView FindStateView(StateSO state)
    {
        return GetNodeByGuid(state.Guid) as StateView;
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach(element =>
            {
                StateView stateView = element as StateView;
                if (stateView != null)
                {
                    _finiteStateMachine.DeleteState(stateView.State);
                }

                Edge edge = element as Edge;
                if (edge != null)
                {
                    StateView parentView = edge.output.node as StateView;
                    StateView childView = edge.input.node as StateView;
                    _finiteStateMachine.RemoveChild(parentView.State, childView.State);
                }
            });
        }

        if (graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(edge =>
            {
                StateView parentView = edge.output.node as StateView;
                StateView childView = edge.input.node as StateView;
                _finiteStateMachine.AddChild(parentView.State, childView.State);
            });
        }
        return graphViewChange;
    }
    #endregion
}