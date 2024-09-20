using SavageWorld.Runtime.Attributes;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace SavageWorld.Editor.Editors
{
    [CustomEditor(typeof(Object), true, isFallback = true)]
    [CanEditMultipleObjects]
    public class DefaultUIElementsEditor : UnityEditor.Editor
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
            //SerializedProperty iterator = serializedObject.GetIterator();
            //if (iterator.NextVisible(true))
            //{
            //    do
            //    {
            //        PropertyField propertyField = new(iterator.Copy()) { name = "PropertyField:" + iterator.propertyPath };

            //        if (iterator.propertyPath == "m_Script" && serializedObject.targetObject != null)
            //            propertyField.SetEnabled(value: false);

            //        _root.Add(propertyField);
            //    }
            //    while (iterator.NextVisible(false));
            //}
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
            Button button = new();
            button.text = buttonName;
            button.clicked += () => method.Invoke(target, null);
            _root.Add(button);
        }
        #endregion
    }
}