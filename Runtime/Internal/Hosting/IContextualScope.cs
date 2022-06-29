using System;

namespace YouInject.Internal
{
    internal interface IContextualScope
    {
        IServiceContainer GetContainer(ServiceLifetime lifetime);
        IServiceDescriptor GetDescriptor(Type serviceType);
    }
}