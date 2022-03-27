using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace YouInject
{
    internal partial class ComponentDescriptor : IServiceDescriptor
    {
        public static readonly Type ComponentType = typeof(Component);
        private readonly MethodInfo? _initialisingMethod;
        internal IReadOnlyList<Type> ParameterTypes { get; }

        public Type ServiceType { get; }
        public Type DecisionType { get; }
        public ServiceLifetime Lifetime { get; private set; }
        
        public ComponentDescriptor(Type serviceType, Type decisionType)
        {
            Assert.IsTrue(decisionType.IsSubclassOf(ComponentType));
            
            ServiceType = serviceType;
            DecisionType = decisionType;
            _initialisingMethod = GetInitializingMethod(decisionType);
            ParameterTypes = GetMethodParameterTypes(_initialisingMethod);
            Lifetime = ServiceLifetime.Scoped;
        }

        internal void InitializeComponent(Component component, object[] parameters)
        {
            _initialisingMethod?.Invoke(component, parameters);
        }

        internal IComponentDescriptorBuilder GetBuilder()
        {
            return new Builder(this);
        }

        private static MethodInfo? GetInitializingMethod(Type decisionType)
        {
            var methods = decisionType.GetMethods();
            MethodInfo? result = null;
            
            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<InitializingAttribute>();
                
                if (attr is null) continue;

                if (result is not null)
                {
                    throw new Exception($"{decisionType.Name} type has more than one methods " +
                                        $"with {nameof(InitializingAttribute)}");
                }

                result = method;
            }

            return result;
        }

        private static Type[] GetMethodParameterTypes(MethodInfo? method)
        {
            if (method is null) return Array.Empty<Type>();

            var parameters = method.GetParameters();
            var types = parameters.Select(parameter => parameter.ParameterType).ToArray();

            return types;
        }
    }
}