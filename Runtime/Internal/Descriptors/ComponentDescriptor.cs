using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace InjectReady.YouInject.Internal
{
    internal class ComponentDescriptor
    {
        private readonly Type _componentType;
        private object? _binding;
        public MethodInfo? Initializer { get; private set; }

        public ComponentDescriptor(Type componentType)
        {
            if (componentType == null) throw new ArgumentNullException(nameof(componentType));
            
            if (!Utility.IsComponentType(componentType))
            {
                throw new ArgumentException(
                    $"Cannot create ComponentDescriptor. {componentType.FullName} is not derived from MonoBehaviour.",
                    nameof(componentType));
            }

            _componentType = componentType;
        }

        public void BindService(DynamicDescriptor serviceDescriptor)
        {
            if (serviceDescriptor == null) throw new ArgumentNullException(nameof(serviceDescriptor));

            switch (_binding)
            {
                case null:
                    _binding = serviceDescriptor;
                    return;
                case List<DynamicDescriptor> list:
                    list.Add(serviceDescriptor);
                    return;
                case DynamicDescriptor value:
                    _binding = new List<DynamicDescriptor> { value, serviceDescriptor };
                    return;
                default:
                    throw new Exception("Unexpected behaviour");
            }
        }

        public void AddInitializer(string methodName)
        {
            if (methodName == null) throw new ArgumentNullException(nameof(methodName));

            if (Initializer is not null)
            {
                throw new InvalidOperationException(
                    $"Cannot add the initializer with name '{methodName}' to the '{_componentType.FullName}' "
                    + $"component. The initialize already exists with name '{Initializer.Name}'.");
            }

            Initializer = _componentType.GetMethod(methodName);

            if (Initializer is null)
            {
                throw new InvalidOperationException($"Cannot get the method '{methodName}' in {_componentType.FullName}.");
            }
        }

        public bool TryGetSingleBinding([MaybeNullWhen(false)]out IServiceDescriptor serviceDescriptor)
        {
            if (_binding is IServiceDescriptor descriptor)
            {
                serviceDescriptor = descriptor;
                return true;
            }

            serviceDescriptor = null;
            return false;
        }
    }
}