using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InjectReady.YouInject.Internal
{
    internal class ScopeContext : IAsyncDisposable
    {
        private readonly Func<Type, object> _serviceProvider;
        private readonly List<object> _disposables;
        private readonly Dictionary<Type, object> _cachedServices;
        private bool _isDisposed;

        public ScopeContext(Func<Type, ScopeContext, object> serviceProvider)
        {
            _serviceProvider = type => serviceProvider.Invoke(type, this);
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

        public object GetService(IServiceDescriptor descriptor)
        {
            if (descriptor.Lifetime != ServiceLifetime.Transient
                && _cachedServices.TryGetValue(descriptor.ServiceType, out var service))
            {
                return service;
            }

            service = descriptor.ResolveService(_serviceProvider);
            OnServiceResolved(service, descriptor);
            return service;
        }

        private void OnServiceResolved(object service, IServiceDescriptor descriptor)
        {
            if (service is IDisposable or IAsyncDisposable)
            {
                _disposables.Add(service);
            }

            if (descriptor.Lifetime != ServiceLifetime.Transient)
            {
                _cachedServices.Add(descriptor.ServiceType, service);
            }
        }
    }
}