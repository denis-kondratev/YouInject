using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InjectReady.YouInject.Internal
{
    internal class ScopeContext : IAsyncDisposable
    {
        private readonly List<object> _disposables;
        private readonly Dictionary<Type, object> _cachedServices;
        private bool _isDisposed;

        internal ScopeContext()
        {
            _cachedServices = new Dictionary<Type, object>();
            _disposables = new List<object>();
        }

        public async ValueTask DisposeAsync()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            
            foreach (var service in _disposables)
            {
                if (service is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
                else
                {
                    ((IDisposable)service).Dispose();
                }
            }
        }
        
        internal bool TryGetCachedService(Type serviceType, out object service)
        {
            ThrowIfDisposed();

            return _cachedServices.TryGetValue(serviceType, out service);
        }
        
        internal void CaptureService(object service, Type? serviceTypeToCache = null)
        {
            ThrowIfDisposed();
            
            if (service is IDisposable or IAsyncDisposable)
            {
                _disposables.Add(service);
            }

            if (serviceTypeToCache is null) return;
            
            if (!_cachedServices.TryAdd(serviceTypeToCache, service))
            {
                throw new InvalidServiceOperationException(
                    serviceTypeToCache,
                    "Cannot cache the service. Another instance has already been cached.");
            }
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