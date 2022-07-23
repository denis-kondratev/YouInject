using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace InjectReady.YouInject.Internal
{
    internal class ServiceProvider : IExtendedServiceProvider, IServiceScopeFactory, IAsyncDisposable
    {
        private delegate object? ServiceResponder(ScopeContext context);
        
        private readonly IReadOnlyDictionary<Type, IServiceDescriptor> _serviceDescriptors;
        private readonly IReadOnlyDictionary<Type, ComponentDescriptor> _componentDescriptors;
        private readonly Dictionary<Type, ServiceResponder> _serviceResponders;
        private readonly ScopeContext _rootContext;
        private bool _isDisposed;

        internal ServiceProvider(
            IReadOnlyDictionary<Type, IServiceDescriptor> serviceDescriptors,
            IReadOnlyDictionary<Type, ComponentDescriptor> componentDescriptors)
        {
            _serviceDescriptors = serviceDescriptors;
            _componentDescriptors = componentDescriptors;
            _rootContext = new ScopeContext();
            _serviceResponders = new Dictionary<Type, ServiceResponder>();
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
            _rootContext.StockpileComponent(component);
        }

        internal object GetService(Type serviceType, ScopeContext context)
        {
            ThrowIfDisposed();

            object? service;
            
            if (_serviceResponders.TryGetValue(serviceType, out var responder))
            {
                service = responder.Invoke(context);
            }
            else
            {
                if (!_serviceDescriptors.TryGetValue(serviceType, out var descriptor))
                {
                    throw new ServiceIsNotRegisteredException(serviceType);
                }

                FabricateServiceAndResponder(descriptor, context, out service);
            }
            
            if (service is null)
            {
                throw new InvalidServiceOperationException(
                    serviceType,
                    "Cannot get the service. It does not exist and cannot be fabricated.");
            }
            
            return service;
        }

        internal void AddDynamicService(Type serviceType, object instance, ScopeContext context)
        {
            ThrowIfDisposed();
            
            if (!_serviceDescriptors.TryGetValue(serviceType, out var descriptor))
            {
                throw new ServiceIsNotRegisteredException(serviceType, "Cannot add a dynamic service.");
            }

            if (descriptor is not DynamicServiceDescriptor dynamicServiceDescriptor)
            {
                throw new InvalidServiceOperationException(
                    serviceType,
                    "Cannot add a dynamic service. The service is not registered as dynamic.");
            }

            var instanceType = instance.GetType();

            if (!serviceType.IsAssignableFrom(instanceType))
            {
                throw new InvalidServiceOperationException(
                    serviceType,
                    $"Cannot add a dynamic service. The service '{serviceType.Name}' is not assignable "
                    + $"from the instance '{instanceType.Name}'.");
            }
            
            AddDynamicService(dynamicServiceDescriptor, instance, context);
        }

        internal void PutComponentIntoService(Type componentType, ScopeContext context)
        {
            ThrowIfDisposed();

            var component = context.PickUpComponent(componentType);
            
            if (!_componentDescriptors.TryGetValue(componentType, out var descriptor))
            {
                throw new InvalidComponentOperationException(
                    componentType,
                    "Cannot put the component into service. The initializing method is not specified and there is no bindings.");
            }
            
            PutComponentIntoService(descriptor, component, context);
        }
        
        private void PutComponentIntoService(
            ComponentDescriptor componentDescriptor,
            MonoBehaviour component,
            ScopeContext context)
        {
            if (componentDescriptor.Initializer is not null)
            {
                InitializeComponent(component, componentDescriptor.Initializer, componentDescriptor.Parameters!, context);
            }

            if (componentDescriptor.TryGetSingleBinding(out var serviceDescriptor))
            {
                AddDynamicService(serviceDescriptor, component, context);
            }

            if (!componentDescriptor.TryGetBindingList(out var serviceDescriptors)) return;
            
            foreach (var descriptor in serviceDescriptors)
            {
                AddDynamicService(descriptor, component, context);
            }
        }

        private void AddDynamicService(DynamicServiceDescriptor descriptor, object service, ScopeContext context)
        {
            var serviceType = descriptor.ServiceType;
            
            if (descriptor.Binding is not null && descriptor.Binding.Type != service.GetType())
            {
                throw new InvalidServiceOperationException(
                    serviceType,
                    $"Cannot add the dynamic service with the instance of the '{service.GetType().Name}' type. "
                    + $"The service is bound to the '{descriptor.Binding.Type.Name}' type.");
            }

            if (_serviceResponders.TryGetValue(serviceType, out var responder))
            {
                var existedService = responder.Invoke(context);
                if (existedService is not null)
                {
                    throw new InvalidServiceOperationException(
                        serviceType,
                        $"Cannot add the dynamic service with the instance of the '{service.GetType().Name}' type. "
                        + $"The instance of the '{existedService.GetType().Name}' type is already added.");
                }
                
                context.CacheService(service, serviceType);
                return;
            }
            
            if (context == _rootContext)
            {
                context.CaptureService(service);
                _serviceResponders.Add(serviceType, _ => service);
                return;
            }
            
            AddScopedDynamicResponder(descriptor);
            context.CacheService(service, serviceType);
        }

        private void InitializeComponent(
            MonoBehaviour component,
            MethodInfo methodInfo,
            ParameterInfo[] parameterInfos,
            ScopeContext context)
        {
            var parameters = parameterInfos.Length == 0 ? Array.Empty<object>() : new object[parameterInfos.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                parameters[i] = GetService(parameterInfos[i].ParameterType, context);
            }

            methodInfo.Invoke(component, parameters);
        }
        
        private void FabricateServiceAndResponder(IServiceDescriptor descriptor, ScopeContext context, out object? service)
        {
            switch (descriptor)
            {
                case DynamicServiceDescriptor dynamic:
                    if (dynamic.Binding is not null
                        && context.TryPickUpComponent(dynamic.Binding.Type, out var component))
                    {
                        PutComponentIntoService(dynamic.Binding, component, context);
                        service = component;
                    }
                    else
                    {
                        service = null;
                    }
                    return;
                case IConstructableServiceDescriptor constructable:
                    FabricateConstructableServiceAndResponder(constructable, context, out service);
                    return;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(descriptor),
                        $"Unexpected descriptor type: {descriptor.GetType()}.");
            }
        }

        private void FabricateConstructableServiceAndResponder(
            IConstructableServiceDescriptor descriptor,
            ScopeContext context,
            out object? service)
        {
            ServiceResponder responder;
            
            switch (descriptor.Lifetime)
            {
                case ServiceLifetime.Transient:
                    AddTransientResponder(descriptor, out responder);
                    break;
                case ServiceLifetime.Scoped:
                    AddScopedResponder(descriptor, out responder);
                    break;
                case ServiceLifetime.Singleton:
                    AddSingletonResponder(descriptor, out service);
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            service = responder.Invoke(context);
        }
        
        private void AddSingletonResponder(IConstructableServiceDescriptor descriptor, out object service)
        {
            service = descriptor.ResolveService(this, _rootContext);
            var singleton = service;
            _serviceResponders.Add(descriptor.ServiceType, _ => singleton);
            _rootContext.CaptureService(service);
        }

        private void AddScopedResponder(IConstructableServiceDescriptor descriptor, out ServiceResponder responder)
        {
            responder = context =>
            {
                if (context.TryGetCachedService(descriptor.ServiceType, out var service))
                {
                    return service;
                }

                service = descriptor.ResolveService(this, context);
                context.CacheService(service, descriptor.ServiceType);
                return service;
            };

            _serviceResponders.Add(descriptor.ServiceType, responder);
        }

        private void AddTransientResponder(IConstructableServiceDescriptor descriptor, out ServiceResponder responder)
        {
            responder = context =>
            {
                var service = descriptor.ResolveService(this, context);
                context.CaptureService(service);
                return service;
            };
            
            _serviceResponders.Add(descriptor.ServiceType, responder);
        }
        
        private void AddScopedDynamicResponder(DynamicServiceDescriptor descriptor)
        {
            if (descriptor.Binding is null)
            {
                _serviceResponders.Add(descriptor.ServiceType, context =>
                {
                    context.TryGetCachedService(descriptor.ServiceType, out var service);
                    return service;
                });
                return;
            }

            _serviceResponders.Add(descriptor.ServiceType, context =>
            {
                if (context.TryGetCachedService(descriptor.ServiceType, out var service))
                {
                    return service;
                }

                if (!context.TryPickUpComponent(descriptor.Binding.Type, out var component))
                {
                    return null;
                }
                
                PutComponentIntoService(descriptor.Binding, component, context);
                return component;
            });
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