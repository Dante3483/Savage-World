using SavageWorld.Runtime.Utilities.FiniteStateMachine;
using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class FiniteStateMachineGraphView : CustomGraphView
{
    public new class UxmlFactory : UxmlFactory<FiniteStateMachineGraphView, UxmlTraits>
    {

    }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {

    }

    #region Fields
    private FiniteStateMachine _finiteStateMachine;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates
    public Action<FiniteStateMachineNodeView> StateSelected;
    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public FiniteStateMachineGraphView() : base()
    {
        Undo.undoRedoPerformed += UndoRedoPerformedHandle;
    }

    public void PopulateView(FiniteStateMachine finiteStateMachine)
    {
        ResetView();
        _finiteStateMachine = finiteStateMachine;
        if (_finiteStateMachine != null)
        {
            graphViewChanged += GraphViewChangedHandler;
            CreateNodes();
            CreateEdges();
            CreateStarterState();
        }
    }
    #endregion

    #region Private Methods
    private void ResetView()
    {
        graphViewChanged -= GraphViewChangedHandler;
        DeleteElements(graphElements);
    }

    private void CreateNodes()
    {
        foreach (State state in _finiteStateMachine.ListOfStates)
        {
            CreateNode(state);
        }
    }

    private void CreateEdges()
    {
        foreach (State state in _finiteStateMachine.ListOfStates)
        {
            foreach (StateTransition transition in state.ListOfTransitions)
            {
                CreateEdge(state, transition);
            }
        }

    }

    private void CreateStarterState()
    {
        if (_finiteStateMachine.StarterState == null)
        {
            _finiteStateMachine.StarterState = CreateState(new(0, 0), "Start");
        }
        FiniteStateMachineNodeView node = FindNodeByState(_finiteStateMachine.StarterState);
        node.AddToClassList("starter-node");
    }

    private FiniteStateMachineNodeView CreateNode(State state)
    {
        FiniteStateMachineNodeView node = new(state);
        node.NodeSelected = OnStateSelected;
        EnableNodeVisualization(state);
        AddElement(node);
        return node;
    }

    private Edge CreateEdge(State state, StateTransition transition)
    {
        FiniteStateMachineNodeView parent = FindNodeByState(state);
        FiniteStateMachineNodeView child = FindNodeByState(transition.State);
        Edge edge = parent.OutputPort.ConnectTo(child.InputPort);
        AddElement(edge);
        return edge;
    }

    private State CreateState(Vector2 position, string name = "")
    {
        State state = _finiteStateMachine.AddState(name);
        state.Position = position;
        CreateNode(state);
        ClearSelection();
        return state;
    }

    private void EnableNodeVisualization(State state)
    {
        state.Entered -= StateEnteredHandle;
        state.Exited -= StateExitedHandle;
        state.Entered += StateEnteredHandle;
        state.Exited += StateExitedHandle;
    }

    private FiniteStateMachineNodeView FindNodeByState(State state)
    {
        return GetNodeByGuid(state.Guid) as FiniteStateMachineNodeView;
    }

    protected override void RegisterCallbacks()
    {
        base.RegisterCallbacks();
        RegisterCallback<MouseDownEvent>(MouseDownEventHandler);
    }

    private void UndoRedoPerformedHandle()
    {
        PopulateView(_finiteStateMachine);
        AssetDatabase.SaveAssets();
    }

    private void MouseDownEventHandler(MouseDownEvent evt)
    {
        if (evt.button == (int)MouseButton.LeftMouse && evt.clickCount == 2)
        {
            CreateState(contentViewContainer.WorldToLocal(evt.mousePosition));
            evt.StopPropagation();
        }
    }

    private GraphViewChange GraphViewChangedHandler(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach(element =>
            {
                if (element is FiniteStateMachineNodeView node)
                {
                    if (node.Data == _finiteStateMachine.StarterState)
                    {
                        CreateNode(node.Data);
                    }
                    else
                    {
                        _finiteStateMachine.RemoveState(node.Data);
                    }
                }

                if (element is Edge edge)
                {
                    FiniteStateMachineNodeView parent = edge.output.node as FiniteStateMachineNodeView;
                    FiniteStateMachineNodeView child = edge.input.node as FiniteStateMachineNodeView;
                    parent.Data.RemoveTransition(child.Data);
                    parent.UpdateExtensionContainer();
                }
            });
        }

        if (graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(edge =>
            {
                FiniteStateMachineNodeView parent = edge.output.node as FiniteStateMachineNodeView;
                FiniteStateMachineNodeView child = edge.input.node as FiniteStateMachineNodeView;
                parent.Data.AddTransition(child.Data);
                parent.UpdateExtensionContainer();
            });
        }
        return graphViewChange;
    }

    private void StateEnteredHandle(State state)
    {
        FindNodeByState(state).AddToClassList("current-node");
        state.ListOfTransitions.ForEach(t => FindNodeByState(t.State).AddToClassList("next-node"));
    }

    private void StateExitedHandle(State state)
    {
        FindNodeByState(state).RemoveFromClassList("current-node");
        state.ListOfTransitions.ForEach(t => FindNodeByState(t.State).RemoveFromClassList("next-node"));
    }

    private void OnStateSelected(CustomNode<State> node)
    {
        StateSelected?.Invoke(node as FiniteStateMachineNodeView);
    }
    #endregion
}