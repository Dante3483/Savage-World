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
    private static FSMDataSO _fsm;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates
    public Action<FSMStateNode> StateSelected;
    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public FSMGraphView() : base()
    {
        RegisterCallback<MouseDownEvent>(CheckDoubleClick);
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        Insert(0, new GridBackground { name = "grid-background" });
    }

    public void PopulateView(FSMDataSO fsm)
    {
        _fsm = fsm;
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        if (_fsm != null)
        {
            graphViewChanged += OnGraphViewChanged;
            _fsm.ListOfStates.ForEach(state => CreateStateNode(state));
            _fsm.ListOfStates.ForEach(state => CreateEdges(state));
            if (_fsm.RootState == null)
            {
                CreateState();
            }
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
        if (_fsm == null)
        {
            return;
        }
        FSMStateSO state = _fsm.CreateState();
        state.Position = position;
        ClearSelection();
        AddToSelection(CreateStateNode(state));
    }

    private FSMStateNode CreateStateNode(FSMStateSO state)
    {
        FSMStateNode stateNode = new(state);
        stateNode.StateSelected = StateSelected;
        AddElement(stateNode);
        return stateNode;
    }

    private void CreateEdges(FSMStateSO state)
    {
        state.ListOfChildren.ForEach(child =>
        {
            FSMStateNode parentView = FindStateNode(state);
            FSMStateNode childView = FindStateNode(child);
            FSMEdge edge = parentView.OutputPort.ConnectTo<FSMEdge>(childView.InputPort);
            AddElement(edge);
        });
    }

    private FSMStateNode FindStateNode(FSMStateSO state)
    {
        return GetNodeByGuid(state.Guid) as FSMStateNode;
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach(element =>
            {
                if (element is FSMStateNode stateNode)
                {
                    _fsm.DeleteState(stateNode.State);
                }

                if (element is Edge edge)
                {
                    FSMStateNode parent = edge.output.node as FSMStateNode;
                    FSMStateNode child = edge.input.node as FSMStateNode;
                    _fsm.RemoveChild(parent.State, child.State);
                    parent.UpdateExtensionContainer();
                }
            });
        }

        if (graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(edge =>
            {
                FSMEdge fsmEdge = edge as FSMEdge;
                FSMStateNode parent = edge.output.node as FSMStateNode;
                FSMStateNode child = edge.input.node as FSMStateNode;
                _fsm.AddChild(parent.State, child.State);
                parent.UpdateExtensionContainer();
            });
        }
        return graphViewChange;
    }
    #endregion
}