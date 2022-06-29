using System;

namespace YouInject.Internal
{
    public class DynamicDescriptor : IServiceDescriptor
    {
        public DynamicDescriptor(Type serviceType, ServiceLifetime lifetime)
        {
            ServiceType = serviceType;
            Lifetime = lifetime;
        }
        
        public Type ServiceType { get; }
        public ServiceLifetime Lifetime { get; }
    }
}