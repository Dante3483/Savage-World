using SavageWorld.Runtime.Utilities;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class TabGroup : VisualElement
{
    public new class UxmlFactory : UxmlFactory<TabGroup, UxmlTraits>
    {

    }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {

    }

    #region Private fields
    private static readonly string _styleResource = StaticInfo.StyleSheetsDirectory + "TabGroup";
    private static readonly string _ussTabGroup = "tab-group";
    private static readonly string _ussTabs = _ussTabGroup + "__tabs";
    private static readonly string _ussTab = _ussTabGroup + "__tab";
    private static readonly string _ussTabSelected = _ussTabGroup + "__tab-selected";
    private static readonly string _ussContent = _ussTabGroup + "__content";

    private SerializedObject _serializedObject;

    private Dictionary<Button, VisualElement> _contentByTab;
    private List<Button> _tabsList;
    private VisualElement _tabs;
    private VisualElement _content;
    private Button _selectedTab;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public TabGroup() : base()
    {
        styleSheets.Add(Resources.Load<StyleSheet>(_styleResource));
        AddToClassList(_ussTabGroup);

        _tabs = new VisualElement();
        _tabs.AddToClassList(_ussTabs);
        _tabs.name = "tabs";
        hierarchy.Add(_tabs);

        _content = new VisualElement();
        _content.AddToClassList(_ussContent);
        _content.name = "content";
        hierarchy.Add(_content);

        _contentByTab = new Dictionary<Button, VisualElement>();
        _tabsList = new List<Button>();
    }

    public void AddTab(Button tab, VisualElement content, SerializedObject serializedObject, bool hideByDefault = false)
    {
        _tabsList.Add(tab);
        _contentByTab.TryAdd(tab, content);

        tab.AddToClassList(_ussTab);
        tab.clicked += () => SelectTab(tab);
        _tabs.Add(tab);

        content.Bind(serializedObject);

        if (hideByDefault)
        {
            HideTab(tab);
        }
        if (_contentByTab.Count == 1)
        {
            SelectTab(0);
        }
    }

    public void AddTab(string tabName, VisualElement content, SerializedObject serializedObject, bool hideByDefault = false)
    {
        Button tab = new();
        tab.text = tabName;
        AddTab(tab, content, serializedObject, hideByDefault);
    }

    public void SelectTab(Button tab)
    {
        if (_selectedTab == tab)
        {
            return;
        }
        if (_contentByTab.ContainsKey(tab))
        {
            tab.AddToClassList(_ussTabSelected);

            if (_selectedTab != null)
            {
                _selectedTab.RemoveFromClassList(_ussTabSelected);
                _contentByTab[_selectedTab].RemoveFromHierarchy();
            }
            _selectedTab = tab;
            _content.Add(_contentByTab[tab]);
        }
    }

    public void SelectTab(int tabIndex)
    {
        if (tabIndex < _tabsList.Count)
        {
            SelectTab(_tabsList[tabIndex]);
        }
    }

    public void HideTab(string tabName)
    {
        Button tab = GetTabByName(tabName);
        if (tab != null)
        {
            HideTab(tab);
        }
    }

    public void HideTab(int tabIndex)
    {
        HideTab(GetTabByIndex(tabIndex));
    }

    private void HideTab(Button tab)
    {
        tab.style.display = DisplayStyle.None;
    }

    public void ShowTab(string tabName)
    {
        Button tab = GetTabByName(tabName);
        if (tab != null)
        {
            ShowTab(tab);
        }
    }

    public void ShowTab(int tabIndex)
    {
        ShowTab(GetTabByIndex(tabIndex));
    }

    private void ShowTab(Button tab)
    {
        tab.style.display = DisplayStyle.Flex;
    }

    private Button GetTabByName(string tabName)
    {
        return _tabsList.Find(tab => tab.text == tabName);
    }

    private Button GetTabByIndex(int tabIndex)
    {
        if (tabIndex < _tabsList.Count)
        {
            return _tabsList[tabIndex];
        }
        return null;
    }
    #endregion
}