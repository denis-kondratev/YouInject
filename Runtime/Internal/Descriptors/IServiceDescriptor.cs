using System;

namespace InjectReady.YouInject.Internal
{
    internal interface IServiceDescriptor
    {
        Type ServiceType { get; }
        ServiceLifetime Lifetime { get; }
        object ResolveService(ContextualServiceProvider serviceProvider);
    }
}