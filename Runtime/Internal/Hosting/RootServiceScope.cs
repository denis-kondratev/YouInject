using System;
using System.Collections.Generic;

namespace InjectReady.YouInject.Internal
{
    internal class RootServiceScope : ServiceScope, IServiceScopeFactory
    {
        private readonly IReadOnlyDictionary<Type, IServiceDescriptor> _descriptors;
        
        public RootServiceScope(IReadOnlyDictionary<Type, IServiceDescriptor> descriptors) 
            : base(new Stack<ContextualServiceProvider>(), new CachingContainer())
        {
            _descriptors = descriptors;
        }

        public override IServiceContainer GetContainer(ServiceLifetime lifetime)
        {
            ThrowIfDisposed();
            
            IServiceContainer container = lifetime switch
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
            
            if (!TryGetDescriptor(serviceType, out var descriptor))
            {
                throw new ServiceRegistrationException(serviceType, "Service is not registered.");
                //throw new InvalidOperationException($"Service '{serviceType}' is not registered.");
            }

            return descriptor;
        }

        public override bool TryGetDescriptor(Type serviceType, out IServiceDescriptor descriptor)
        {
            return _descriptors.TryGetValue(serviceType, out descriptor);
        }

        public ServiceScope CreateScope(ThruContainer scopedContainer)
        {
            var scope = new InheritedServiceScope(this, ContextPool, scopedContainer);
            return scope;
        }
    }
}