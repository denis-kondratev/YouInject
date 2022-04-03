using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace YouInject
{
    internal partial class ComponentDescriptor : IRawServiceDescriptor, IServiceDescriptor
    {
        public static readonly Type ComponentType = typeof(Component);
        private readonly MethodInfo? _initialisingMethod;
        private bool _isBaked;

        public Type ServiceType { get; }
        public Type DecisionType { get; }
        public ServiceLifetime Lifetime { get; private set; }

        internal IReadOnlyList<Type> ParameterTypes { get; }

        public ComponentDescriptor(Type serviceType, Type decisionType)
        {
            Assert.IsTrue(decisionType.IsSubclassOf(ComponentType));
            Assert.IsTrue(serviceType.IsAssignableFrom(decisionType));
            
            ServiceType = serviceType;
            DecisionType = decisionType;
            _initialisingMethod = GetInitializingMethod(decisionType);
            ParameterTypes = GetMethodParameterTypes(_initialisingMethod);
            Lifetime = ServiceLifetime.Scoped;
        }

        public IServiceDescriptor Bake()
        {
            Assert.IsFalse(_isBaked);
            _isBaked = true;
            return this;
        }

        public object InstantiateDecision(ServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }

        internal IComponentDescriptorBuilder GetBuilder()
        {
            Assert.IsFalse(_isBaked);
            return new Builder(this);
        }

        internal void InitializeComponent(Component component, object[] parameters)
        {
            Assert.IsTrue(_isBaked);
            _initialisingMethod?.Invoke(component, parameters);
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