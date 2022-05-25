using System;

namespace YouInject
{
    internal interface IServiceDescriptor
    {
        Type ServiceType { get; }
        Type DecisionType { get; }
        ServiceLifetime Lifetime { get; }
        bool AsComponent { get; }
        object InstantiateDecision(Func<Type[], object[]> getParameters);
    }
}