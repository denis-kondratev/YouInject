using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InjectReady.YouInject.Internal
{
    internal class ThruContainer : CachingContainer
    {
        private readonly CachingContainer _parentContainer;

        public ThruContainer(CachingContainer parentContainer)
        {
            _parentContainer = parentContainer;
        }

        public override bool TryGetService(Type serviceType, out object service)
        {
            return base.TryGetService(serviceType, out service) 
                   || _parentContainer.TryGetService(serviceType, out service);
        }
    }
    
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

        public override object GetService(IServiceDescriptor descriptor, ContextualServiceProvider serviceProvider)
        {
            if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));
            
            ThrowIfDisposed();
            
            if (TryGetService(descriptor.ServiceType, out var service))
            {
                return service;
            }
            
            service = descriptor.ResolveService(serviceProvider);
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

        public void RemoveService(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));

            ThrowIfDisposed();
            
            _services.Remove(serviceType);
        }

        public bool Contains(Type serviceType)
        {
            return _services.ContainsKey(serviceType);
        }

        public virtual bool TryGetService(Type serviceType, out object service)
        {
            return _services.TryGetValue(serviceType, out service);
        }
    }
}