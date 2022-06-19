using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YouInject.Internal
{
    internal partial class ServiceScope : IServiceScope, IServiceScopeFactory
    {
        private readonly IServiceContainer _singletonContainer;
        private readonly IServiceContainer _scopedContainer;
        private readonly IServiceContainer _transientContainer;
        private readonly IReadOnlyDictionary<Type, IServiceDescriptor> _descriptors;
        private readonly Stack<Context> _contextPool;
        private bool _isDisposed;

        public ServiceScope(IServiceContainer singletonContainer, IReadOnlyDictionary<Type, IServiceDescriptor> descriptors)
        {
            _singletonContainer = singletonContainer;
            _descriptors = descriptors;
            _scopedContainer = new CacheableContainer();
            _transientContainer = new DisposableContainer();
            _contextPool = new Stack<Context>();
        }
        
        public async ValueTask DisposeAsync()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            
            await _scopedContainer.DisposeAsync().ConfigureAwait(false);
            await _transientContainer.DisposeAsync().ConfigureAwait(false);
        }
        
        public object GetService(Type serviceType)
        {
            ThrowIfDisposed();
            
            if (!_contextPool.TryPop(out var context))
            {
                context = new Context(this);
            }

            var service = GetService(serviceType, context);
            context.Release();
            _contextPool.Push(context);
            return service;
        }

        public IServiceScope CreateScope()
        {
            var scope = new ServiceScope(_singletonContainer, _descriptors);
            return scope;
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

        private object GetService(Type serviceType, Context context)
        {
            var descriptor = GetDescriptor(serviceType);
            var container = GetContainer(descriptor.Lifetime);
            var service = container.GetService(descriptor, context);
            return service;
        }

        private IServiceContainer GetContainer(ServiceLifetime lifetime)
        {
            var container = lifetime switch
            {
                ServiceLifetime.Transient => _transientContainer,
                ServiceLifetime.Scoped => _scopedContainer,
                ServiceLifetime.Singleton => _singletonContainer,
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
            };

            return container;
        }

        private IServiceDescriptor GetDescriptor(Type serviceType)
        {
            if (!_descriptors.TryGetValue(serviceType, out var descriptor))
            {
                throw new InvalidOperationException($"Service '{serviceType}' is not registered.");
            }

            return descriptor;
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new InvalidOperationException("Containers is already disposed");
            }
        }
    }
}