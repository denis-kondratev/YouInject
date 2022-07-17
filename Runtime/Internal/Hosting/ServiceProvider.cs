using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace InjectReady.YouInject.Internal
{
    internal class ServiceProvider : IExtendedServiceProvider, IServiceScopeFactory, IAsyncDisposable
    {
        private readonly IReadOnlyDictionary<Type, IServiceDescriptor> _serviceDescriptors;
        private readonly IReadOnlyDictionary<Type, ComponentDescriptor> _componentDescriptors;
        private readonly ScopeContext _rootContext;
        private bool _isDisposed;
        private readonly Dictionary<Type, Func<ScopeContext, object>> _serviceResponders;

        public ServiceProvider(
            IReadOnlyDictionary<Type, IServiceDescriptor> serviceDescriptors,
            IReadOnlyDictionary<Type, ComponentDescriptor> componentDescriptors)
        {
            _serviceDescriptors = serviceDescriptors;
            _componentDescriptors = componentDescriptors;
            _rootContext = new ScopeContext();
            _serviceResponders = new Dictionary<Type, Func<ScopeContext, object>>();
        }

        public ValueTask DisposeAsync()
        {
            if (_isDisposed) return default;

            _isDisposed = true;
            return _rootContext.DisposeAsync();
        }

        public IServiceScope CreateScope()
        {
            ThrowIfDisposed();
            
            var scope = new ServiceScope(this);
            return scope;
        }
        
        public object GetService(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            
            ThrowIfDisposed();

            var service = GetService(serviceType, _rootContext);
            return service;
        }

        public void AddDynamicService(Type serviceType, object instance)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            
            ThrowIfDisposed();
            
            AddDynamicService(serviceType, instance, _rootContext);
        }

        public void InitializeComponent(MonoBehaviour instance)
        {
            throw new NotImplementedException();
        }

        public void StockpileComponent(MonoBehaviour component)
        {
            throw new NotImplementedException();
        }

        internal object GetService(Type serviceType, ScopeContext callSiteContext)
        {
            ThrowIfDisposed();
            
            if (_serviceResponders.TryGetValue(serviceType, out var responder))
            {
                return responder.Invoke(callSiteContext);
            }
            
            responder = CreateServiceResponder(serviceType);
            _serviceResponders.Add(serviceType, responder);
            var service = responder.Invoke(callSiteContext);
            return service;
        }

        internal void AddDynamicService(Type serviceType, object instance, ScopeContext callSiteContext)
        {
            ThrowIfDisposed();
            
            if (!_serviceDescriptors.TryGetValue(serviceType, out var descriptor))
            {
                throw new ServiceIsNotRegisteredException(serviceType, "Cannot add a dynamic service.");
            }

            if (descriptor is not DynamicDescriptor dynamicDescriptor)
            {
                throw new InvalidServiceOperationException(serviceType, "Cannot add a non-dynamic service.");
            }
            
            if (dynamicDescriptor.Binding is not null && dynamicDescriptor.Binding != instance.GetType())
            {
                throw new InvalidServiceOperationException(
                    serviceType,
                    $"Cannot add the service with the instance of '{instance.GetType().Name}'. "
                    + $"The service is bound to '{dynamicDescriptor.Binding.Name}' type.");
            }

            if (descriptor.Lifetime != ServiceLifetime.Singleton)
            {
                var serviceTypeToCache = descriptor.Lifetime == ServiceLifetime.Transient ?  null : serviceType;
                callSiteContext.CaptureService(instance, serviceTypeToCache);
                return;
            }
            
            _rootContext.CaptureService(instance);

            if (!_serviceResponders.TryAdd(serviceType, _ => instance))
            {
                throw new InvalidServiceOperationException(
                    serviceType,
                    "Cannot add a dynamic service. The instance of the service already exists.");
            }
        }

        private Func<ScopeContext, object> CreateServiceResponder(Type serviceType)
        {
            if (!_serviceDescriptors.TryGetValue(serviceType, out var descriptor))
            {
                throw new ServiceIsNotRegisteredException(serviceType);
            }

            if (descriptor.Lifetime == ServiceLifetime.Singleton)
            {
                var service = descriptor.ResolveService(this, _rootContext);
                _rootContext.CaptureService(service);
                return _ => service;
            }

            return scopeContext =>
            {
                if (scopeContext.TryGetCachedService(serviceType, out var service))
                {
                    return service;
                }

                service = descriptor.ResolveService(this, scopeContext);
                scopeContext.CaptureService(service, descriptor.GetTypeToCache());
                return service;
            };
        }
        
        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(ServiceProvider));
            }
        }
    }
}