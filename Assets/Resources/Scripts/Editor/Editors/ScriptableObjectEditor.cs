using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(ScriptableObject), true, isFallback = false)]
public class ScriptableObjectEditor : Editor
{
    #region Private fields
    private VisualElement _root;
    #endregion

    #region Public fields

    #endregion

    #region Properties

    #endregion

    #region Methods
    public override VisualElement CreateInspectorGUI()
    {
        _root = new VisualElement();
        AddFields();
        AddButtons();
        return _root;
    }

    private void AddFields()
    {
        InspectorElement.FillDefaultInspector(_root, serializedObject, this);
    }

    private void AddButtons()
    {
        BindingFlags methodFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly;
        for (Type type = target.GetType(); type != null; type = type.BaseType)
        {
            foreach (MethodInfo method in type.GetMethods(methodFlags))
            {
                ButtonAttribute buttonAttribute = method.GetCustomAttribute(typeof(ButtonAttribute), true) as ButtonAttribute;
                if (buttonAttribute != null)
                {
                    AddButton(buttonAttribute.ButtonName, method);
                }
            }
        }
    }

    private void AddButton(string buttonName, MethodInfo method)
    {
        Button button = new Button();
        button.text = buttonName;
        button.clicked += () => method.Invoke(target, null);
        _root.Add(button);
    }
    #endregion
}
