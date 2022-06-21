using System;
using System.Collections.Generic;

namespace YouInject.Internal
{
    internal class ServiceCollection : IServiceCollection
    {
        private readonly Dictionary<Type, IServiceDescriptor> _descriptors; 
        private bool _isBaked;

        internal ServiceCollection()
        {
            _descriptors = new Dictionary<Type, IServiceDescriptor>();
        }
        
        public void AddFactory(Type factoryType, Type productType, ServiceLifetime lifetime)
        {
            ThrowIfCannotAdd(factoryType, "Cannot add factory");
            var descriptor = new FactoryDescriptor(factoryType, productType, lifetime);
            _descriptors.Add(factoryType, descriptor);
        }
        
        public void AddComponent(Type serviceType, Type instanceType, string initializingMethodName)
        {
            ThrowIfCannotAdd(serviceType, "Cannot add component");
            var descriptor = new ComponentDescriptor(serviceType, instanceType, initializingMethodName);
            _descriptors.Add(serviceType, descriptor);
        }

        public void AddService(Type serviceType, Type instanceType, ServiceLifetime lifetime)
        {
            ThrowIfCannotAdd(serviceType, "Cannot add service");
            var descriptor = new ServiceDescriptor(serviceType, instanceType, lifetime);
            _descriptors.Add(serviceType, descriptor);
        }
        
        public IReadOnlyDictionary<Type, IServiceDescriptor> Bake()
        {
            if (_isBaked)
            {
                throw new InvalidOperationException("Cannot bake. Service Collection is already baked.");
            }
            
            _isBaked = true;
            return _descriptors;
        }

        private void ThrowIfCannotAdd(Type key, string message)
        {
            if (_isBaked)
            {
                throw new InvalidOperationException(message + $" '{key.Name}'. Service Collection is already baked.");
            }

            if (_descriptors.ContainsKey(key))
            {
                throw new InvalidOperationException(message + $" '{key.Name}'. It is already registered.");
            }
        }
    }
}