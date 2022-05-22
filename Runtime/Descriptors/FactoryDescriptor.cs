using System;
using UnityEngine.Assertions;

namespace YouInject
{
    internal partial class FactoryDescriptor : IRawServiceDescriptor, IServiceDescriptor
    {
        private bool _isBaked;
        private readonly FactoryBuilder _factoryBuilder;
        public Type ServiceType { get; }
        public Type DecisionType { get; }
        public ServiceLifetime Lifetime { get; }

        internal FactoryDescriptor(Type factoryType, Type resultType)
        {
            Assert.AreEqual(factoryType.BaseType, typeof(MulticastDelegate), $"The '{factoryType.Name}' type is not a delegate.");

            _factoryBuilder = new FactoryBuilder(factoryType, resultType);
            ServiceType = factoryType;
            DecisionType = factoryType;
            Lifetime = ServiceLifetime.Transient;
        }

        public IServiceDescriptor Bake()
        {
            Assert.IsFalse(_isBaked);
            
            _isBaked = true;
            return this;
        }

        public object InstantiateDecision(ServiceProvider serviceProvider)
        {
            Assert.IsTrue(_isBaked);
            
            var factory = _factoryBuilder.BuildFactory(serviceProvider);
            var factoryMethodInfo = _factoryBuilder.FactoryMethodInfo;
            var factoryDelegate = Delegate.CreateDelegate(DecisionType, factory, factoryMethodInfo, true);
            return factoryDelegate!;
        }
    }
}