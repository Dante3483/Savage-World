using SavageWorld.Runtime.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SavageWorld.Runtime.DependencyInjection
{
    [DefaultExecutionOrder(-1000)]
    public class Injector : MonoBehaviour
    {
        #region Private fields
        [SerializeField]
        private Provider _provider;
        private const BindingFlags _bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private readonly Dictionary<Type, object> _registry = new();
        #endregion

        #region Public fields

        #endregion

        #region Properties

        #endregion

        #region Methods
        private void Awake()
        {
            RegisterProvider();
            Inject();
        }

        private void RegisterProvider()
        {
            if (_provider == null)
            {
                throw new("Provider is null");
            }
            var fields = _provider.GetType().GetFields(_bindingFlags);
            foreach (var field in fields)
            {
                var fieldValue = field.GetValue(_provider);
                if (fieldValue is null)
                {
                    throw new("Field value is null");
                }
                else
                {
                    _registry.Add(field.FieldType, fieldValue);
                }
            }
        }

        private void Inject()
        {
            var injectObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID).Where(IsObjectHasInjectAttribute);
            foreach (var injectObject in injectObjects)
            {
                var type = injectObject.GetType();
                var injectFields = type.GetFields(_bindingFlags).Where(IsFieldHasInjectAttribute);
                foreach (var injectField in injectFields)
                {
                    var fieldType = injectField.FieldType;
                    var fieldInstance = GetObjectFromRegistry(fieldType);
                    if (fieldInstance is null)
                    {
                        throw new("Item in registry is null");
                    }
                    injectField.SetValue(injectObject, fieldInstance);
                }
            }
        }

        private bool IsFieldHasInjectAttribute(FieldInfo info)
        {
            return Attribute.IsDefined(info, typeof(InjectAttribute));
        }

        private object GetObjectFromRegistry(Type type)
        {
            _registry.TryGetValue(type, out object value);
            return value;
        }

        private bool IsObjectHasInjectAttribute(MonoBehaviour behaviour)
        {
            var members = behaviour.GetType().GetMembers(_bindingFlags);
            return members.Any(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
        }
        #endregion
    }
}