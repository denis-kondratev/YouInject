using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace YouInject.Internal
{
    internal class DynamicComponentDescriptor : DynamicDescriptor
    {
        private readonly string? _initializingMethodName;
        private MethodInfo? _initializingMethod;
        private Type? _implementationType; 

        public DynamicComponentDescriptor(Type serviceType, string? initializingMethodName) 
            : base(serviceType)
        {
            if (!DescriptorUtility.IsMonoBehavior(serviceType))
            {
                throw new ArgumentException(
                    $"Cannot register the dynamic component service '{serviceType.FullName}'. It is not derived " +
                    $"from {typeof(MonoBehaviour).FullName}.",
                    nameof(serviceType));
            }
            
            _initializingMethodName = initializingMethodName;
        }

        public override object ResolveService(ContextualServiceProvider serviceProvider)
        {
            var component = Object.FindObjectOfType(ServiceType, true);
            var componentType = component.GetType();
            var initializingMethod = GetInitializingMethod(componentType);

            if (initializingMethod is null)
            {
                return component;
            }
            
            var parameters = serviceProvider.GetServices(initializingMethod.GetParameters());
            initializingMethod.Invoke(component, parameters);
            return component;
        }

        private MethodInfo? GetInitializingMethod(Type implementationType)
        {
            if (implementationType == _implementationType)
            {
                return _initializingMethod;
            }

            _implementationType = implementationType;

            if (string.IsNullOrEmpty(_initializingMethodName))
            {
                return null;
            }

            _initializingMethod = implementationType.GetMethod(_initializingMethodName);
            return _initializingMethod;
        }
    }
}