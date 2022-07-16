using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace InjectReady.YouInject.Internal
{
    internal class RootServiceScope : IServiceScope, IExtendedServiceProvider, IServiceScopeFactory
    {
        private readonly IReadOnlyDictionary<Type, IServiceDescriptor> _serviceMap;
        private readonly IReadOnlyDictionary<Type, ComponentDescriptor> _descriptorMap;
        private readonly ScopeContext _rootContext;
        private bool _isDisposed;

        public IExtendedServiceProvider ServiceProvider => this;
        
        public RootServiceScope(
            IReadOnlyDictionary<Type, IServiceDescriptor> serviceMap,
            IReadOnlyDictionary<Type, ComponentDescriptor> descriptorMap)
        {
            _serviceMap = serviceMap;
            _descriptorMap = descriptorMap;
            _rootContext = new ScopeContext(GetService);
        }

        public async ValueTask DisposeAsync()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            await _rootContext.DisposeAsync();
        }

        public IServiceScope CreateScope()
        {
            throw new NotImplementedException();
        }
        
        public object GetService(Type serviceType)
        {
            return GetService(serviceType, _rootContext);
        }

        public void AddDynamicService(Type serviceType, object instance)
        {
            AddDynamicService(serviceType, instance, _rootContext);
        }

        public void RemoveComponent(MonoBehaviour instance)
        {
            throw new NotImplementedException();
        }

        public void InitializeComponent(MonoBehaviour instance)
        {
            throw new NotImplementedException();
        }

        public void AddComponent(MonoBehaviour instance)
        {
            throw new NotImplementedException();
        }

        private object GetService(Type serviceType, ScopeContext callSiteContext)
        {
            if (!_serviceMap.TryGetValue(serviceType, out var descriptor))
            {
                throw new ServiceIsNotRegisteredException(serviceType, "Cannot resolve the service.");
            }

            var context = descriptor.Lifetime == ServiceLifetime.Singleton ? _rootContext : callSiteContext;
            var service = context.GetService(descriptor);
            return service;
        }
        
        private void AddDynamicService(Type serviceType, object instance, ScopeContext callSiteContext)
        {
            if (!_serviceMap.TryGetValue(serviceType, out var descriptor))
            {
                throw new ServiceIsNotRegisteredException(serviceType, "Cannot add the dynamic service.");
            }

            if (descriptor is not DynamicDescriptor dynamicDescriptor)
            {
                throw new InvalidOperationWithServiceException(
                    serviceType,
                    "Cannot add the service. It is not registered as dynamic.");
            }
            
            if (dynamicDescriptor.Binding is not null
                && dynamicDescriptor.Binding != instance.GetType())
            {
                throw new InvalidOperationWithServiceException(
                    serviceType,
                    $"Cannot add the service with the instance of '{instance.GetType().Name}'. "
                    + $"The service is bound to '{dynamicDescriptor.Binding.Name}' type.");
            }

            var context = descriptor.Lifetime == ServiceLifetime.Singleton ? _rootContext : callSiteContext;
            context.AddService(instance, descriptor);
        }

        private IServiceDescriptor GetDescriptor(Type serviceType)
        {
            if (_serviceMap.TryGetValue(serviceType, out var descriptor))
            {
                return descriptor;
            }
            
            throw new ServiceIsNotRegisteredException(serviceType, "The service is not registered");
        }
    }
}