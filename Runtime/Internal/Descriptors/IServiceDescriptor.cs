using System;

namespace YouInject.Internal
{
    internal interface IServiceDescriptor
    {
        Type ServiceType { get; }
        ServiceLifetime Lifetime { get; }
    }

    public class DynamicDescriptor : IServiceDescriptor
    {
        public DynamicDescriptor(Type serviceType)
        {
            ServiceType = serviceType;
        }
        
        public Type ServiceType { get; }
        public ServiceLifetime Lifetime => ServiceLifetime.Scoped;
    }
}