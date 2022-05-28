using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace YouInject
{
    internal partial class ComponentDescriptor : IRawServiceDescriptor, IServiceDescriptor
    {
        public static readonly Type ComponentType = typeof(Component);
        private MethodInfo? _initialisingMethod;
        private Type[]? _initializingParameterTypes;
        private bool _isBaked;

        public ComponentDescriptor(Type serviceType, Type decisionType)
        {
            Assert.IsTrue(decisionType.IsSubclassOf(ComponentType));
            Assert.IsTrue(serviceType.IsAssignableFrom(decisionType));
            
            ServiceType = serviceType;
            DecisionType = decisionType;
            Lifetime = ServiceLifetime.Scoped;
        }

        public Type ServiceType { get; }

        public Type DecisionType { get; }

        public ServiceLifetime Lifetime { get; }
        public bool AsComponent => true;

        internal IReadOnlyList<Type>? InitializingParameterTypes => _initializingParameterTypes;

        public IServiceDescriptor Bake()
        {
            Assert.IsFalse(_isBaked);
            _isBaked = true;
            return this;
        }

        public object InstantiateDecision(Func<Type[], object[]> getParameters)
        {
            throw new NotImplementedException($"Cannot instantiate a component of type '{DecisionType.Name}' by resolving. Use factories for it.");
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
    }
}