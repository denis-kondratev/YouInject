using System;

namespace YouInject
{
    internal interface IServiceDescriptor
    {
        Type ServiceType { get; }
        Type DecisionType { get; }
        ServiceLifetime Lifetime { get; }
        object InstantiateDecision(ServiceProvider serviceProvider);
    }
}