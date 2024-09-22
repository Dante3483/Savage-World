using SavageWorld.Runtime.Utilities.FiniteStateMachine.Actions;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class FiniteStateMachineActionsView : VisualElement
{
    #region Fields
    private ListView _listView;
    private List<StateActionBase> _listOfActions;
    private FiniteStateMachineSearchProvider _searchProvider;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public FiniteStateMachineActionsView() : base()
    {

    }

    public FiniteStateMachineActionsView(SerializedProperty serializedProperty, List<StateActionBase> listOfActions) : base()
    {
        _searchProvider = ScriptableObject.CreateInstance<FiniteStateMachineSearchProvider>();
        _searchProvider.Name = "Actions";
        _searchProvider.Type = typeof(StateActionBase);
        _searchProvider.OnSelect = AddAction;

        _listOfActions = listOfActions;
        _listView = new(listOfActions);
        _listView.headerTitle = serializedProperty.displayName;
        _listView.bindingPath = serializedProperty.propertyPath;
        _listView.reorderable = true;
        _listView.showBoundCollectionSize = true;
        _listView.showFoldoutHeader = true;
        _listView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
        _listView.reorderMode = ListViewReorderMode.Animated;
        _listView.Q<Toggle>().AddManipulator(new ContextualMenuManipulator(ShowSearchWindow));
        Add(_listView);
    }
    #endregion

    #region Private Methods
    private void AddAction(Type type)
    {
        _listOfActions.Add((StateActionBase)Activator.CreateInstance(type));
    }

    private void ShowSearchWindow(ContextualMenuPopulateEvent evt)
    {
        SearchWindow.Open(new(GUIUtility.GUIToScreenPoint(evt.mousePosition)), _searchProvider);
    }
    #endregion
}