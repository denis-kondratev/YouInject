using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace YouInject.Internal
{
    internal class ComponentDescriptor : IServiceDescriptor
    {
        public ComponentDescriptor(Type serviceType, Type instanceType, string initializingMethodName)
        {
            ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
            Lifetime = ServiceLifetime.Scoped;
            Initializer = GetInitializer(instanceType, initializingMethodName);
        }
        
        public Type ServiceType { get; }

        public ServiceLifetime Lifetime { get; }
        
        public Action<Component, ContextualServiceProvider> Initializer { get; }
        
        private static Action<Component, ContextualServiceProvider> GetInitializer(Type instanceType, string methodName)
        {
            var methodInfo = GetInitializingMethod(instanceType, methodName);
            var parameterTypes = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();

            return (instance, context) =>
            {
                var parameters = context.GetServices(parameterTypes);
                methodInfo.Invoke(instance, parameters);
            };
        }
        
        private static MethodInfo GetInitializingMethod(Type instanceType, string methodName)
        {
            var methodInfo = instanceType.GetMethod(methodName);
            
            if (methodInfo is null)
            {
                throw new ArgumentException($"Cannot find the '{methodName}' method in '{instanceType.Name}' type.");
            }
            
            return methodInfo;
        }
    }
}