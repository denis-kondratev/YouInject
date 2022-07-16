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
        private readonly ScopeContext _scopeContext;
        private bool _isDisposed;

        public IExtendedServiceProvider ServiceProvider => this;
        
        public RootServiceScope(
            IReadOnlyDictionary<Type, IServiceDescriptor> serviceMap,
            IReadOnlyDictionary<Type, ComponentDescriptor> descriptorMap)
        {
            _serviceMap = serviceMap;
            _descriptorMap = descriptorMap;
            _scopeContext = new ScopeContext(GetService);
        }

        public async ValueTask DisposeAsync()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            await _scopeContext.DisposeAsync();
        }

        public IServiceScope CreateScope()
        {
            throw new NotImplementedException();
        }
        
        public object GetService(Type serviceType)
        {
            return GetService(serviceType, _scopeContext);
        }

        public void AddDynamicService(Type serviceType, object instance)
        {
            throw new NotImplementedException();
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

        private object GetService(Type serviceType, ScopeContext scopeContext)
        {
            if (!_serviceMap.TryGetValue(serviceType, out var descriptor))
            {
                throw new ServiceResolvingException(serviceType, "The service is not registered");
            }

            var context = descriptor.Lifetime == ServiceLifetime.Singleton ? _scopeContext : scopeContext;
            var service = context.GetService(descriptor);
            return service;
        }
    }
}