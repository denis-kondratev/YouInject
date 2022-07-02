using System;

namespace InjectReady.YouInject.Internal
{
    internal class DynamicDescriptor : IDynamicDescriptor
    {
        public Type ServiceType { get; }
        public ServiceLifetime Lifetime { get; private set; }

        public DynamicDescriptor(Type serviceType)
        {
            ServiceType = serviceType;
            Lifetime = ServiceLifetime.Scoped;
        }

        public virtual object ResolveService(ContextualServiceProvider serviceProvider)
        {
            throw new OperationCanceledException($"The dynamic service '{ServiceType.FullName}' has not been added");
        }

        public void SetLifetime(ServiceLifetime lifetime)
        {
            if (lifetime == ServiceLifetime.Transient)
            {
                throw new ArgumentOutOfRangeException(nameof(lifetime), "DynamicDescriptor cannot be Transient.");
            }
            
            Lifetime = lifetime;
        }
    }
}