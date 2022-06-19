using System;

namespace YouInject.Internal
{
    internal interface IServiceDescriptor
    {
        Type ServiceType { get; }
        ServiceLifetime Lifetime { get; }
        Func<ScopeContext, object> InstanceFactory { get; }
    }
}