using System;
using System.Collections.Generic;

namespace YouInject.Internal
{
    internal class RootServiceScope : ServiceProvider, IServiceScopeFactory
    {
        private readonly IReadOnlyDictionary<Type, IServiceDescriptor> _descriptors;
        
        public RootServiceScope(IReadOnlyDictionary<Type, IServiceDescriptor> descriptors)
        {
            _descriptors = descriptors;
        }

        public IServiceScope CreateScope()
        {
            var scope = new ServiceScope(this);
            return scope;
        }
        
        public override IServiceContainer GetContainer(ServiceLifetime lifetime)
        {
            ThrowIfDisposed();
            
            var container = lifetime switch
            {
                ServiceLifetime.Transient => TransientContainer,
                ServiceLifetime.Scoped => ScopedContainer,
                ServiceLifetime.Singleton => ScopedContainer,
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
            };

            return container;
        }
        
        public override IServiceDescriptor GetDescriptor(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            
            ThrowIfDisposed();
            
            if (!_descriptors.TryGetValue(serviceType, out var descriptor))
            {
                throw new InvalidOperationException($"Service '{serviceType}' is not registered.");
            }

            return descriptor;
        }
    }
}