using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YouInject.Internal
{
    internal class CacheableContainer : IServiceContainer
    {
        private readonly Dictionary<Type, object> _services;
        private bool _isDisposed;

        public CacheableContainer()
        {
            _services = new Dictionary<Type, object>();
        }

        public async ValueTask DisposeAsync()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            
            foreach (var service in _services.Values)
            {
                switch (service)
                {
                    case IAsyncDisposable asyncDisposable:
                        await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                        break;
                    case IDisposable disposable:
                        disposable.Dispose();
                        break;
                }
            }
        }

        public object GetService(IServiceDescriptor descriptor, ServiceScope.Context context)
        {
            if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));
            if (context == null) throw new ArgumentNullException(nameof(context));
            
            ThrowIfDisposed();
            
            if (_services.TryGetValue(descriptor.ServiceType, out var service))
            {
                return service;
            }

            service = descriptor.InstanceFactory.Invoke(context);
            _services.Add(descriptor.ServiceType, service);
            return service;
        }

        public void AddService(IServiceDescriptor descriptor, object service)
        {
            if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));
            if (service == null) throw new ArgumentNullException(nameof(service));
            
            ThrowIfDisposed();
            
            _services.Add(descriptor.ServiceType, service);
        }

        public void RemoveService(IServiceDescriptor descriptor)
        {
            if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));
            
            ThrowIfDisposed();
            
            _services.Remove(descriptor.ServiceType);
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed) throw new InvalidOperationException("Container is already disposed.");
        }
    }
}