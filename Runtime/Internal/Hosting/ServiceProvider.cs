using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace InjectReady.YouInject.Internal
{
    internal class ServiceProvider : IExtendedServiceProvider, IServiceScopeFactory, IAsyncDisposable
    {
        private delegate object ServiceResponder(ScopeContext context, bool useStockpile);
        
        private readonly IReadOnlyDictionary<Type, IServiceDescriptor> _serviceDescriptors;
        private readonly IReadOnlyDictionary<Type, ComponentDescriptor> _componentDescriptors;
        private readonly ScopeContext _rootContext;
        private bool _isDisposed;
        private readonly Dictionary<Type, ServiceResponder> _serviceResponders;
        private readonly Stockpile _stockpile;

        public ServiceProvider(
            IReadOnlyDictionary<Type, IServiceDescriptor> serviceDescriptors,
            IReadOnlyDictionary<Type, ComponentDescriptor> componentDescriptors)
        {
            _serviceDescriptors = serviceDescriptors;
            _componentDescriptors = componentDescriptors;
            _rootContext = new ScopeContext();
            _serviceResponders = new Dictionary<Type, ServiceResponder>();
            _stockpile = new Stockpile();
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
            
            var service = GetService(serviceType, _rootContext);
            return service;
        }

        public void AddDynamicService(Type serviceType, object instance)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            
            AddDynamicService(serviceType, instance, _rootContext);
        }

        public void PutComponentIntoService(Type componentType)
        {
            if (componentType == null) throw new ArgumentNullException(nameof(componentType));
            
            PutComponentIntoService(componentType, _rootContext);
        }

        public void StockpileComponent(MonoBehaviour component)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));
            
            ThrowIfDisposed();
            var componentType = component.GetType();

            if (!Utility.IsComponentType(componentType))
            {
                throw new InvalidComponentOperationException(
                    componentType,
                    "Cannot stockpile the component. It is not derived from the MonoBehaviour type.");
            }

            _stockpile.Add(componentType, component);
        }

        internal object GetService(Type serviceType, ScopeContext context, bool useStockpile = false)
        {
            ThrowIfDisposed();
            
            if (_serviceResponders.TryGetValue(serviceType, out var responder))
            {
                return responder.Invoke(context, useStockpile);
            }
            
            responder = CreateServiceResponder(serviceType, useStockpile) ?? _serviceResponders[serviceType];
            _serviceResponders.Add(serviceType, responder);
            var service = responder.Invoke(context, useStockpile);
            return service;
        }

        internal void AddDynamicService(Type serviceType, object instance, ScopeContext context)
        {
            ThrowIfDisposed();
            
            if (!_serviceDescriptors.TryGetValue(serviceType, out var descriptor))
            {
                throw new ServiceIsNotRegisteredException(serviceType, "Cannot add a dynamic service.");
            }

            AddDynamicService(descriptor, instance, context);
        }

        internal void PutComponentIntoService(Type componentType, ScopeContext context)
        {
            ThrowIfDisposed();

            var component = _stockpile.PickUpComponent(componentType);
            
            
            if (!_componentDescriptors.TryGetValue(componentType, out var descriptor))
            {
                throw new InvalidComponentOperationException(
                    componentType,
                    "Cannot put the component into service. The initializing method is not specified and there is no bindings.");
            }
            
            PutComponentIntoService(descriptor, component, context);
        }
        
        private void PutComponentIntoService(ComponentDescriptor componentDescriptor, MonoBehaviour component, ScopeContext context)
        {
            if (componentDescriptor.Initializer is not null)
            {
                InitializeComponent(component, componentDescriptor.Initializer, componentDescriptor.Parameters!, context);
            }

            if (componentDescriptor.TryGetSingleBinding(out var serviceDescriptor))
            {
                AddDynamicService(serviceDescriptor, component, context);
            }

            if (componentDescriptor.TryGetBindingList(out var serviceDescriptors))
            {
                foreach (var descriptor in serviceDescriptors)
                {
                    AddDynamicService(descriptor, component, context);
                }
            }
        }

        private void AddDynamicService(IServiceDescriptor descriptor, object instance, ScopeContext context)
        {
            var serviceType = descriptor.ServiceType;
            
            if (descriptor is not DynamicDescriptor dynamicDescriptor)
            {
                throw new InvalidServiceOperationException(serviceType, "Cannot add a non-dynamic service.");
            }
            
            if (dynamicDescriptor.Binding is not null && dynamicDescriptor.Binding.Type != instance.GetType())
            {
                throw new InvalidServiceOperationException(
                    serviceType,
                    $"Cannot add the service with the instance of '{instance.GetType().Name}'. "
                    + $"The service is bound to '{dynamicDescriptor.Binding.Type.Name}' type.");
            }

            if (descriptor.Lifetime != ServiceLifetime.Singleton)
            {
                var serviceTypeToCache = descriptor.Lifetime == ServiceLifetime.Transient ?  null : serviceType;
                context.CaptureService(instance, serviceTypeToCache);
                return;
            }
            
            if (!_serviceResponders.TryAdd(serviceType, (_, _) => instance))
            {
                throw new InvalidServiceOperationException(
                    serviceType,
                    "Cannot add a dynamic service. The instance of the service already exists.");
            }

            _rootContext.CaptureService(instance);
        }

        private void InitializeComponent(MonoBehaviour component, MethodInfo methodInfo, ParameterInfo[] parameterInfos, ScopeContext context)
        {
            var parameters = parameterInfos.Length == 0 ? Array.Empty<object>() : new object[parameterInfos.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                parameters[i] = GetService(parameterInfos[i].ParameterType, context, true);
            }

            methodInfo.Invoke(component, parameters);
        }

        private ServiceResponder? CreateServiceResponder(Type serviceType, bool useStockpile)
        {
            if (!_serviceDescriptors.TryGetValue(serviceType, out var descriptor))
            {
                throw new ServiceIsNotRegisteredException(serviceType);
            }

            if (descriptor.Lifetime != ServiceLifetime.Singleton)
            {
                return (context, stockpile) => RespondServiceWithContext(descriptor, context, stockpile);
            }

            if (useStockpile && TryGetServiceFromStockpile(descriptor, _rootContext, out var service))
            {
                return null;
            }
            
            service = descriptor.ResolveService(this, _rootContext);
            _rootContext.CaptureService(service);
            return (_, _) => service;
        }

        private object RespondServiceWithContext(IServiceDescriptor descriptor, ScopeContext context, bool useStockpile)
        {
            if (context.TryGetCachedService(descriptor.ServiceType, out var service))
            {
                return service;
            }
            
            if (useStockpile && TryGetServiceFromStockpile(descriptor, context, out service))
            {
                return service;
            }
            
            service = descriptor.ResolveService(this, context);
            context.CaptureService(service, descriptor.GetTypeToCache());
            return service;
        }

        private bool TryGetServiceFromStockpile(IServiceDescriptor descriptor, ScopeContext context, [MaybeNullWhen(false)] out object service)
        {
            if (descriptor is not DynamicDescriptor dynamic
                || dynamic.Binding is null
                || !_stockpile.TryPickUpComponent(dynamic.Binding.Type, out var component))
            {
                service = null;
                return false;
            }
            
            PutComponentIntoService(dynamic.Binding, component, context);
            service = component;
            return true;
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