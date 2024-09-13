using SavageWorld.Runtime.Utilities.FSM.Actions;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

public class FSMActionView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<FSMActionView, UxmlTraits>
    {

    }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {

    }

    #region Fields
    private ListView _listView;
    private List<FSMActionBase> _listOfActions;
    #endregion

    #region Properties

    #endregion

    #region Events / Delegates

    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public FSMActionView()
    {

    }

    public FSMActionView(SerializedProperty serializedProperty, List<FSMActionBase> listOfActions) : base()
    {
        _listOfActions = listOfActions;
        _listView = new(listOfActions);
        _listView.headerTitle = serializedProperty.displayName;
        _listView.bindingPath = serializedProperty.propertyPath;
        _listView.reorderable = true;
        _listView.showBoundCollectionSize = true;
        _listView.showFoldoutHeader = true;
        _listView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
        _listView.reorderMode = ListViewReorderMode.Animated;
        _listView.Q<Toggle>().AddManipulator(new ContextualMenuManipulator((evt) =>
        {
            evt.menu.ClearItems();
            TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<FSMActionBase>();
            foreach (Type type in types)
            {
                if (!type.IsAbstract)
                {
                    evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (ation) => CreateAction(type), DropdownMenuAction.AlwaysEnabled);
                }
            }
        }));
        Add(_listView);
    }

    public void Update(List<FSMActionBase> listOfActions)
    {

    }
    #endregion

    #region Private Methods
    private void CreateAction(Type type)
    {
        if (_listOfActions != null)
        {
            _listOfActions.Add((FSMActionBase)Activator.CreateInstance(type));
        }
    }
    #endregion
}