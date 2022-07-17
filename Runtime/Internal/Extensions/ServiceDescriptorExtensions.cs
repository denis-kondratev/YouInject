using System;

namespace InjectReady.YouInject.Internal
{
    internal static class ServiceDescriptorExtensions
    {
        internal static Type? GetTypeToCache(this IServiceDescriptor descriptor)
        {
            return descriptor.Lifetime is ServiceLifetime.Transient ? null : descriptor.ServiceType;
        }
    }
}