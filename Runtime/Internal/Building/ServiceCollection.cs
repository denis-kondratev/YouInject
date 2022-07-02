using System;
using System.Collections.Generic;

namespace InjectReady.YouInject.Internal
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
            if (factoryType == null) throw new ArgumentNullException(nameof(factoryType));
            if (productType == null) throw new ArgumentNullException(nameof(productType));
            
            ThrowIfCannotAdd(factoryType, "Cannot add factory");
            
            var descriptor = new FactoryDescriptor(factoryType, productType, lifetime);
            _descriptors.Add(factoryType, descriptor);
        }

        public void AddService(Type serviceType, Type instanceType, ServiceLifetime lifetime)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));
            
            ThrowIfCannotAdd(serviceType, "Cannot add service");
            
            var descriptor = new ConstructableDescriptor(serviceType, instanceType, lifetime);
            _descriptors.Add(serviceType, descriptor);
        }

        public void AddDynamicService(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            
            ThrowIfCannotAdd(serviceType, "Cannot add dynamic service");
            
            var descriptor = new DynamicDescriptor(serviceType);
            _descriptors.Add(serviceType, descriptor);
        }

        public void AddDynamicComponent(Type serviceType, string? initializingMethodName)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            
            ThrowIfCannotAdd(serviceType, "Cannot add dynamic component");

            var descriptor = new ComponentDescriptor(serviceType, initializingMethodName);
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