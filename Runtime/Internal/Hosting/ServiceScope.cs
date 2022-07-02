using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;

namespace InjectReady.YouInject.Internal
{
    internal abstract partial class ServiceScope : IServiceScope, IContextualScope
    {
        protected readonly CachingContainer ScopedContainer;
        protected readonly DisposableContainer TransientContainer;
        private readonly Stack<ContextualServiceProvider> _contextPool;
        private bool _isDisposed;

        protected abstract ServiceLifetime DynamicLifetime { get; }
        
        protected ServiceScope()
        {
            ScopedContainer = new CachingContainer();
            TransientContainer = new DisposableContainer();
            _contextPool = new Stack<ContextualServiceProvider>();
        }
        
        public async ValueTask DisposeAsync()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            
            await ScopedContainer.DisposeAsync().ConfigureAwait(false);
            await TransientContainer.DisposeAsync().ConfigureAwait(false);
        }
        
        public object GetService(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            
            ThrowIfDisposed();
            
            using var context = new Context(this);
            var service = context.ServiceProvider.GetService(serviceType);
            return service;
        }

        public void AddService(Type serviceType, object service)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (service == null) throw new ArgumentNullException(nameof(service));

            ThrowIfDisposed();
            
            if (!serviceType.IsInstanceOfType(service))
            {
                throw new ArgumentException(
                    $"The service of type '{service.GetType().Name}' is not instance of type '{serviceType.Name}'.",
                    nameof(service));
            }
            
            if (!TryGetDescriptor(serviceType, out var descriptor))
            {
                throw new InvalidOperationException($"The '{serviceType.Name}' service is not registered");
            }

            if (descriptor is not IDynamicDescriptor dynamicDescriptor)
            {
                throw new InvalidOperationException($"The '{serviceType.Name}' service is not registered as dynamic one.");
            }

            var lifetime = dynamicDescriptor.Lifetime;
            var container = GetContainer(lifetime) as CachingContainer;

            if (container!.Contains(serviceType))
            {
                throw new InvalidOperationException($"The '{serviceType.Name}' service already exists.");
            }

            if (lifetime != DynamicLifetime)
            {
                dynamicDescriptor.SetLifetime(DynamicLifetime);
                container = GetContainer(dynamicDescriptor.Lifetime) as CachingContainer;
            }
            
            container!.AddService(serviceType, service);
        }

        public void RemoveService(Type serviceType)
        {
            if (_isDisposed) return;

            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            
            var descriptor = GetDescriptor(serviceType);

            if (descriptor is not DynamicDescriptor)
            {
                throw new InvalidOperationException($"Cannot delete '{serviceType.Name}' service. " +
                                                    "Only dynamic services can be deleted.");
            }
            
            var container = GetContainer(descriptor.Lifetime) as CachingContainer;
            container!.RemoveService(descriptor.ServiceType);
        }

        public void InitializeService(Delegate initializeDelegate)
        {
            if (initializeDelegate == null) throw new ArgumentNullException(nameof(initializeDelegate));

            ThrowIfDisposed();

            using var context = new Context(this);
            var parameterTypes = initializeDelegate.Method.GetParameters();
            var parameters = context.ServiceProvider.GetServices(parameterTypes);
            initializeDelegate.DynamicInvoke(parameters);
        }

        public void InitializeService(object service, MethodInfo methodInfo)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (methodInfo == null) throw new ArgumentNullException(nameof(methodInfo));
            
            ThrowIfDisposed();

            using var context = new Context(this);
            var parameterTypes = methodInfo.GetParameters();
            var parameters = context.ServiceProvider.GetServices(parameterTypes);
            methodInfo.Invoke(service, parameters);
        }

        public abstract IServiceContainer GetContainer(ServiceLifetime lifetime);

        public abstract IServiceDescriptor GetDescriptor(Type serviceType);

        public abstract bool TryGetDescriptor(Type serviceType, [MaybeNullWhen(false)] out IServiceDescriptor descriptor);
        
        protected void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new InvalidOperationException("Containers is already disposed");
            }
        }
    }
}