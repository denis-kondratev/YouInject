using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YouInject.Internal
{
    internal partial class ServiceScope : IServiceProvider, IAsyncDisposable
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
        
        private object GetService(Type serviceType, Context context)
        {
            if (!_descriptors.TryGetValue(serviceType, out var descriptor))
            {
                throw new InvalidOperationException($"Service '{serviceType}' is not registered.");
            }

            var container = GetContainer(descriptor.Lifetime);
            var service = container.GetService(descriptor, context);
            return service;
        }
        
        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new InvalidOperationException("Containers is already disposed");
            }
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
    }
}