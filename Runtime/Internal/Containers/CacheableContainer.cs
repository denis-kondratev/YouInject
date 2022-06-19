using System;
using System.Collections.Generic;

namespace YouInject.Internal
{
    internal class CacheableContainer : DisposableContainer
    {
        private readonly Dictionary<Type, object> _services;

        public CacheableContainer()
        {
            _services = new Dictionary<Type, object>();
        }

        public override object GetService(IServiceDescriptor descriptor, ServiceScope.Context context)
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
            CaptureDisposable(service);
            return service;
        }
    }
}