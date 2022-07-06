using System;
using System.Reflection;
using Object = UnityEngine.Object;

namespace InjectReady.YouInject.Internal
{
    internal class ComponentDescriptor : DynamicDescriptor
    {
        private readonly string? _initializingMethodName;
        private Type? _implementationType;
        
        public MethodInfo? InitializingMethod { get; private set; }

        public ComponentDescriptor(Type serviceType, bool isSingleton, string? initializingMethodName) 
            : base(serviceType, isSingleton)
        {
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

        public MethodInfo? GetInitializingMethod(Type implementationType)
        {
            if (implementationType == _implementationType)
            {
                return InitializingMethod;
            }

            _implementationType = implementationType;

            if (string.IsNullOrEmpty(_initializingMethodName))
            {
                return null;
            }

            InitializingMethod = implementationType.GetMethod(_initializingMethodName);
            return InitializingMethod;
        }
    }
}