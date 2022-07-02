using System;

namespace YouInject.Internal
{
    internal interface IServiceDescriptor
    {
        Type ServiceType { get; }
        ServiceLifetime Lifetime { get; }
        object ResolveService(ContextualServiceProvider serviceProvider);
    }
}