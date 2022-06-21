using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YouInject.Internal
{
    internal class CachingContainer : ServiceContainer
    {
        private readonly Dictionary<Type, object> _services;

        public CachingContainer()
        {
            _services = new Dictionary<Type, object>();
        }

        public override async ValueTask DisposeAsync()
        {
            if (IsDisposed) return;

            IsDisposed = true;
            
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

        public override object GetService(IServiceDescriptor descriptor, ScopeContext context)
        {
            if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));
            if (context == null) throw new ArgumentNullException(nameof(context));
            
            ThrowIfDisposed();
            
            if (_services.TryGetValue(descriptor.ServiceType, out var service))
            {
                return service;
            }

            service = CreateService(descriptor, context);
            _services.Add(descriptor.ServiceType, service);
            return service;
        }

        public void AddService(Type serviceType, object service)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (service == null) throw new ArgumentNullException(nameof(service));
            
            ThrowIfDisposed();
            
            _services.Add(serviceType, service);
        }

        public void RemoveService(IServiceDescriptor descriptor)
        {
            if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));
            
            ThrowIfDisposed();
            
            _services.Remove(descriptor.ServiceType);
        }
    }
}