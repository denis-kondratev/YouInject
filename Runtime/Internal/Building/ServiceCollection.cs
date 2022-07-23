using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace InjectReady.YouInject.Internal
{
    internal class ServiceCollection : IServiceCollection
    {
        private readonly Dictionary<Type, IServiceDescriptor> _serviceDescriptors;
        private readonly Dictionary<Type, ComponentDescriptor> _componentDescriptors;
        private bool _isBaked;

        public Dictionary<Type, IServiceDescriptor> ServiceDescriptors
        {
            get
            {
                ThrowIfNotBaked();
                return _serviceDescriptors;
            }
        }
        
        public Dictionary<Type, ComponentDescriptor> ComponentDescriptors
        {
            get
            {
                ThrowIfNotBaked();
                return _componentDescriptors;
            }
        }

        internal ServiceCollection()
        {
            _serviceDescriptors = new Dictionary<Type, IServiceDescriptor>();
            _componentDescriptors = new Dictionary<Type, ComponentDescriptor>();
        }
        
        public void AddDelegateFactory(Type delegateFactoryType, Type productInstanceType, ServiceLifetime lifetime)
        {
            if (delegateFactoryType == null) throw new ArgumentNullException(nameof(delegateFactoryType));
            if (productInstanceType == null) throw new ArgumentNullException(nameof(productInstanceType));
            
            ThrowIfBaked(delegateFactoryType);
            var descriptor = new DelegateFactoryDescriptor(delegateFactoryType, productInstanceType, lifetime);
            
            if (!_serviceDescriptors.TryAdd(delegateFactoryType, descriptor))
            {
                ThrowServiceAlreadyRegistered(delegateFactoryType);
            }
        }
        
        public void AddService(Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (implementationType == null) throw new ArgumentNullException(nameof(implementationType));
            
            ThrowIfBaked(serviceType);
            var descriptor = new ConstructableServiceDescriptor(serviceType, implementationType, lifetime);
            
            if (!_serviceDescriptors.TryAdd(serviceType, descriptor))
            {
                ThrowServiceAlreadyRegistered(serviceType);
            }
        }

        public DynamicServiceRegistration AddDynamicService(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            
            ThrowIfBaked(serviceType);
            var descriptor = new DynamicServiceDescriptor(serviceType);
            
            if (!_serviceDescriptors.TryAdd(serviceType, descriptor))
            {
                ThrowServiceAlreadyRegistered(serviceType);
            }

            if (Utility.IsComponentType(serviceType))
            {
                BindServiceToComponent(descriptor, serviceType);
            }
            
            var registration = new DynamicServiceRegistration(this, descriptor);
            return registration;
        }

        public void InitializeComponentWith(Type componentType, string methodName)
        {
            if (componentType == null) throw new ArgumentNullException(nameof(componentType));
            if (methodName == null) throw new ArgumentNullException(nameof(methodName));
            
            if (_isBaked)
            {
                throw new InvalidOperationException(
                    $"Cannot add an initializer to the '{componentType.Name}' component with the '{methodName}' "
                    + $"method\nServiceCollection has already been baked.");
            }

            var descriptor = GetComponentDescriptor(componentType);
            descriptor.AddInitializer(methodName);
        }

        internal ComponentDescriptorRegistration BindServiceToComponent(
            DynamicServiceDescriptor dynamicServiceDescriptor,
            Type componentType)
        {
            if (_isBaked)
            {
                throw new ServiceBindingFailureException(
                    dynamicServiceDescriptor.ServiceType,
                    componentType,
                    "ServiceCollection has already been baked.");
            }

            var componentDescriptor = GetComponentDescriptor(componentType);
            dynamicServiceDescriptor.BindComponent(componentDescriptor);
            componentDescriptor.BindService(dynamicServiceDescriptor);
            
            var registration = new ComponentDescriptorRegistration(this, componentType);
            return registration;
        }

        internal void Bake()
        {
            if (_isBaked) return;

            // TODO: Check collection.
            // Services cannot refer to each other.
            // A singleton dynamic service cannot refer to a scoped dynamic service.
            _isBaked = true;
        }

        private ComponentDescriptor GetComponentDescriptor(Type componentType)
        {
            if (_componentDescriptors.TryGetValue(componentType, out var componentDescriptor))
            {
                return componentDescriptor;
            }

            componentDescriptor = new ComponentDescriptor(componentType);
            _componentDescriptors.Add(componentType, componentDescriptor);
            return componentDescriptor;
        }

        [DoesNotReturn]
        private static void ThrowServiceAlreadyRegistered(Type serviceType)
        {
            throw new ServiceRegistrationFailureException(serviceType, "The service has already registered.");
        }
        
        private void ThrowIfBaked(Type serviceType)
        {
            if (_isBaked)
            {
                throw new ServiceRegistrationFailureException(serviceType, "Service Collection is already baked.");
            }
        }
        
        private void ThrowIfNotBaked()
        {
            if (!_isBaked)
            {
                throw new InvalidOperationException($"{nameof(ServiceCollection)} has not been baked yet.");
            }
        }
    }
}