using SavageWorld.Runtime.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FiniteStateMachineSearchProvider : ScriptableObject, ISearchWindowProvider
{
    #region Fields
    private string _name;
    private Type _type;
    #endregion

    #region Properties
    public string Name
    {
        get
        {
            return _name;
        }

        set
        {
            _name = value;
        }
    }

    public Type Type
    {
        get
        {
            return _type;
        }

        set
        {
            _type = value;
        }
    }
    #endregion

    #region Events / Delegates
    public Action<Type> OnSelect;
    #endregion

    #region Monobehaviour Methods

    #endregion

    #region Public Methods
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> tree = new()
        {
            new SearchTreeGroupEntry(new GUIContent(_name), 0)
        };
        foreach (Type type in TypeCache.GetTypesDerivedFrom(_type))
        {
            if (!type.IsAbstract)
            {
                GUIContent content = new(type.Name);
                if (type.CustomAttributes != null)
                {
                    FSMComponentAttribute attribute = type.GetCustomAttribute<FSMComponentAttribute>();
                    if (attribute != null)
                    {
                        content.text = attribute.Name;
                    }
                }
                SearchTreeEntry entry = new(content);
                entry.level = 1;
                entry.userData = type;
                tree.Add(entry);
            }
        }
        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        OnSelect?.Invoke((Type)SearchTreeEntry.userData);
        return true;
    }
    #endregion

    #region Private Methods

    #endregion
}