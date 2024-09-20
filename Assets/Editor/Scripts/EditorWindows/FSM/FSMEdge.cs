using SavageWorld.Runtime.Utilities.FSM;
using SavageWorld.Runtime.Utilities.FSM.Conditions;
using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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
        this.AddManipulator(new ContextualMenuManipulator(evt =>
        {
            if (evt.target is FSMEdge)
            {
                evt.menu.ClearItems();
                TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<FSMConditionBase>();
                foreach (Type type in types)
                {
                    if (!type.IsAbstract)
                    {
                        evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (ation) => CreateCondition(type), DropdownMenuAction.AlwaysEnabled);
                    }
                }
            }
        }));
    }

    public override void OnSelected()
    {
        base.OnSelected();
        EdgeSelected?.Invoke(this);
    }
    #endregion

    #region Private Methods
    private void CreateCondition(Type type)
    {
        FSMStateView parentView = output.node as FSMStateView;
        FSMStateView childView = input.node as FSMStateView;
        parentView.State.ConditionByChild[childView.State] = (FSMConditionBase)Activator.CreateInstance(type);
    }
    #endregion
}