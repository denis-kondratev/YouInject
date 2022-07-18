using System;
using System.Collections.Generic;

namespace InjectReady.YouInject.Internal
{
    internal class ServiceCollection : IServiceCollection
    {
        private readonly Dictionary<Type, IServiceDescriptor> _serviceMap;
        private readonly Dictionary<Type, ComponentDescriptor> _componentDescriptors;
        private bool _isBaked;

        public Dictionary<Type, IServiceDescriptor> ServiceMap
        {
            get
            {
                ThrowIfNotBaked();
                return _serviceMap;
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
            _serviceMap = new Dictionary<Type, IServiceDescriptor>();
            _componentDescriptors = new Dictionary<Type, ComponentDescriptor>();
        }
        
        public void AddDelegateFactory(Type delegateFactoryType, Type productInstanceType, ServiceLifetime lifetime)
        {
            if (delegateFactoryType == null) throw new ArgumentNullException(nameof(delegateFactoryType));
            if (productInstanceType == null) throw new ArgumentNullException(nameof(productInstanceType));
            
            ThrowIfCannotAddService(delegateFactoryType, "Cannot add the delegate factory");

            var descriptor = new DelegateFactoryDescriptor(delegateFactoryType, productInstanceType, lifetime);
            _serviceMap.Add(delegateFactoryType, descriptor);
        }
        
        public void AddService(Type serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (implementationType == null) throw new ArgumentNullException(nameof(implementationType));
            
            ThrowIfCannotAddService(serviceType, "Cannot add the service");

            var descriptor = new ConstructableDescriptor(serviceType, implementationType, lifetime);
            _serviceMap.Add(serviceType, descriptor);
        }

        public DynamicServiceRegistration AddDynamicService(Type serviceType, bool isSingleton)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            
            ThrowIfCannotAddService(serviceType, "Cannot add the dynamic service");

            var descriptor = new DynamicDescriptor(serviceType, isSingleton);
            _serviceMap.Add(serviceType, descriptor);

            if (Utility.IsComponentType(serviceType))
            {
                BindServiceToComponent(descriptor, serviceType);
            }
            
            var registration = new DynamicServiceRegistration(this, descriptor);
            return registration;
        }

        public ComponentDescriptorRegistration BindServiceToComponent(
            DynamicDescriptor dynamicDescriptor,
            Type componentType)
        {
            if (dynamicDescriptor == null) throw new ArgumentNullException(nameof(dynamicDescriptor));
            if (componentType == null) throw new ArgumentNullException(nameof(componentType));

            if (_isBaked)
            {
                throw new ServiceBindingException(dynamicDescriptor.ServiceType, componentType, $"{nameof(ServiceCollection)} has already been baked.");
            }

            var componentDescriptor = GetComponentDescriptor(componentType);
            dynamicDescriptor.BindComponent(componentDescriptor);
            componentDescriptor.BindService(dynamicDescriptor);
            
            var registration = new ComponentDescriptorRegistration(this, componentType);
            return registration;
        }

        public void InitializeComponentWith(Type componentType, string methodName)
        {
            if (componentType == null) throw new ArgumentNullException(nameof(componentType));
            if (methodName == null) throw new ArgumentNullException(nameof(methodName));
            
            if (_isBaked)
            {
                var message = $"\n{nameof(ServiceCollection)} has already been baked.";
                throw new InvalidOperationException(GetExceptionMessage() + message);
            }

            var descriptor = GetComponentDescriptor(componentType);
            descriptor.AddInitializer(methodName);
            
            string GetExceptionMessage()
            {
                return $"Cannot initialize the '{componentType.Name}' component with the '{methodName}' method.";
            }
        }
        
        public void Bake()
        {
            if (_isBaked) return;

            // TODO: Check collection.
            _isBaked = true;
        }

        private void ThrowIfCannotAddService(Type serviceType, string message)
        {
            if (_isBaked)
            {
                throw new InvalidOperationException(message + $" '{serviceType.Name}'. Service Collection is already baked.");
            }

            if (_serviceMap.ContainsKey(serviceType))
            {
                throw new InvalidOperationException(message + $" '{serviceType.Name}'. The service has already been added.");
            }
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

        private void ThrowIfNotBaked()
        {
            if (!_isBaked)
            {
                throw new InvalidOperationException($"{nameof(ServiceCollection)} has not been baked yet.");
            }
        }
    }
}