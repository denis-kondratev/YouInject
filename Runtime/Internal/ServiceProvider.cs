using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace YouInject.Internal
{
    internal abstract class ServiceProvider : IServiceScope, IContextualServiceScope
    {
        protected readonly CachingContainer ScopedContainer;
        protected readonly DisposableContainer TransientContainer;
        
        private readonly Stack<ScopeContext> _contextPool;
        private bool _isDisposed;

        protected ServiceProvider()
        {
            ScopedContainer = new CachingContainer();
            TransientContainer = new DisposableContainer();
            _contextPool = new Stack<ScopeContext>();
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
            
            if (!_contextPool.TryPop(out var context))
            {
                context = new ScopeContext(this);
            }

            var service = context.GetService(serviceType);
            context.Release();
            _contextPool.Push(context);
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
                descriptor = new DynamicDescriptor(serviceType);
                AddDynamicDescriptor((DynamicDescriptor)descriptor);
            }
            
            ScopedContainer.AddService(serviceType, service);
        }

        public void RemoveService(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            
            var descriptor = GetDescriptor(serviceType);

            if (descriptor is not DynamicDescriptor)
            {
                throw new InvalidOperationException($"Cannot delete '{serviceType.Name}' service. " +
                                                    "Only dynamic services can be deleted.");
            }
            
            ScopedContainer.RemoveService(descriptor);
        }
        
        public abstract IServiceContainer GetContainer(ServiceLifetime lifetime);

        public abstract IServiceDescriptor GetDescriptor(Type serviceType);

        public abstract bool TryGetDescriptor(Type serviceType, [MaybeNullWhen(false)] out IServiceDescriptor descriptor);

        public abstract void AddDynamicDescriptor(DynamicDescriptor descriptor);

        protected void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new InvalidOperationException("Containers is already disposed");
            }
        }
    }
}