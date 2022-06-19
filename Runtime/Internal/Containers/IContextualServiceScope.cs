using System;

namespace YouInject.Internal
{
    internal interface IContextualServiceScope
    {
        IServiceContainer GetContainer(ServiceLifetime lifetime);
        IServiceDescriptor GetDescriptor(Type serviceType);
    }
}