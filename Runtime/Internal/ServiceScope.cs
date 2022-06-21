using System;

namespace YouInject.Internal
{
    internal class ServiceScope : ServiceProvider
    {
        private readonly RootServiceScope _root;

        public ServiceScope(RootServiceScope root)
        {
            _root = root;
        }
        
        public override IServiceContainer GetContainer(ServiceLifetime lifetime)
        {
            ThrowIfDisposed();
            
            var container = lifetime switch
            {
                ServiceLifetime.Transient => TransientContainer,
                ServiceLifetime.Scoped => ScopedContainer,
                ServiceLifetime.Singleton => _root.GetContainer(ServiceLifetime.Singleton),
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
            };

            return container;
        }

        public override IServiceDescriptor GetDescriptor(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            
            ThrowIfDisposed();

            return _root.GetDescriptor(serviceType);
        }

        public override bool TryGetDescriptor(Type serviceType, out IServiceDescriptor descriptor)
        {
            return _root.TryGetDescriptor(serviceType, out descriptor);
        }

        public override void AddDynamicDescriptor(DynamicDescriptor descriptor)
        {
            _root.AddDynamicDescriptor(descriptor);
        }
    }
}