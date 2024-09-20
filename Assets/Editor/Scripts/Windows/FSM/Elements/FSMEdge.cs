using SavageWorld.Runtime.Utilities.FSM;
using SavageWorld.Runtime.Utilities.FSM.Conditions;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class FSMEdge : Edge
{
    public new class UxmlFactory : UxmlFactory<FSMEdge, UxmlTraits>
    {

    }

    public new class UxmlTraits : Edge.UxmlTraits
    {

    }

    #region Fields
    private FSMStateSO _parent;
    private FSMStateSO _child;
    private FSMConditionBase _condition;
    private FSMGraphSearchProvider _searchProvider;
    #endregion

    #region Properties
    public FSMConditionBase Condition
    {
        get
        {
            return _condition;
        }

        set
        {
            _condition = value;
        }
    }
    #endregion

    #region Events / Delegates
    public Action<FSMEdge> EdgeSelected;
    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public FSMEdge() : base()
    {
        _searchProvider = ScriptableObject.CreateInstance<FSMGraphSearchProvider>();
        _searchProvider.Name = "Conditions";
        _searchProvider.Type = typeof(FSMConditionBase);
        _searchProvider.OnSelect = CreateCondition;

        RegisterCallback<MouseDownEvent>(ShowSearchWindow);
    }

    public override void OnSelected()
    {
        base.OnSelected();
        EdgeSelected?.Invoke(this);
    }
    #endregion

    #region Private Methods
    private void ShowSearchWindow(MouseDownEvent evt)
    {
        if (evt.button == (int)MouseButton.RightMouse)
        {
            SearchWindow.Open(new(GUIUtility.GUIToScreenPoint(evt.mousePosition)), _searchProvider);
        }
    }

    private void CreateCondition(Type type)
    {
        FSMStateView parentView = output.node as FSMStateView;
        FSMStateView childView = input.node as FSMStateView;
        parentView.State.ConditionByChild[childView.State] = (FSMConditionBase)Activator.CreateInstance(type);
    }
    #endregion
}