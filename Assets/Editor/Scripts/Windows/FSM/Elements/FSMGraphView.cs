using SavageWorld.Runtime.Utilities.FSM;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
        RegisterCallback<MouseDownEvent>(OnMouseDown);
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        Insert(0, new GridBackground { name = "grid-background" });
        Undo.undoRedoPerformed += OnUndoRedo;
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
            _fsm.ListOfStates.ForEach(state => EnableStateVisualization(state));
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
    private void CreateState()
    {
        CreateState(new(0, 0));
    }

    private void CreateState(Vector2 position)
    {
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

    private void EnableStateVisualization(FSMStateSO state)
    {
        state.Entered -= OnStateEntered;
        state.Exited -= OnStateExited;
        state.Entered += OnStateEntered;
        state.Exited += OnStateExited;
    }

    private void CreateEdges(FSMStateSO state)
    {
        state.ListOfChildren.ForEach(child =>
        {
            FSMStateNode parentNode = FindStateNode(state);
            FSMStateNode childNode = FindStateNode(child);
            FSMEdge edge = parentNode.OutputPort.ConnectTo<FSMEdge>(childNode.InputPort);
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
                    if (stateNode.State == _fsm.RootState)
                    {
                        CreateStateNode(stateNode.State);
                    }
                    else
                    {
                        _fsm.DeleteState(stateNode.State);
                    }
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

    private void OnMouseDown(MouseDownEvent evt)
    {
        if (evt.button == (int)MouseButton.LeftMouse && evt.clickCount == 2)
        {
            CreateState(contentViewContainer.WorldToLocal(evt.mousePosition));
            evt.StopPropagation();
        }
    }

    private void OnUndoRedo()
    {
        PopulateView(_fsm);
        AssetDatabase.SaveAssets();
    }

    private void OnStateEntered(FSMStateSO state)
    {
        FindStateNode(state).AddToClassList("current-node");
        state.ListOfChildren.ForEach(child => FindStateNode(child).AddToClassList("next-node"));
    }

    private void OnStateExited(FSMStateSO state)
    {
        FindStateNode(state).RemoveFromClassList("current-node");
        state.ListOfChildren.ForEach(child => FindStateNode(child).RemoveFromClassList("next-node"));
    }
    #endregion
}