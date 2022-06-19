using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YouInject.Internal
{
    internal abstract class ServiceProvider : IServiceScope, IContextualServiceScope
    {
        protected readonly IServiceContainer ScopedContainer;
        protected readonly IServiceContainer TransientContainer;
        
        private readonly Stack<ScopeContext> _contextPool;
        private bool _isDisposed;

        protected ServiceProvider()
        {
            ScopedContainer = new CacheableContainer();
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
            
            var descriptor = GetDescriptor(serviceType);
            var container = GetContainer(descriptor.Lifetime);
            container.AddService(descriptor, service);
        }

        public void RemoveScope(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            var descriptor = GetDescriptor(serviceType);
            var container = GetContainer(descriptor.Lifetime);
            container.RemoveService(descriptor);
        }
        
        public abstract IServiceContainer GetContainer(ServiceLifetime lifetime);

        public abstract IServiceDescriptor GetDescriptor(Type serviceType);

        protected void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new InvalidOperationException("Containers is already disposed");
            }
        }
    }
}