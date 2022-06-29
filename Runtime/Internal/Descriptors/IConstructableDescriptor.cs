using System;

namespace YouInject.Internal
{
    internal interface IConstructableDescriptor : IServiceDescriptor
    {
        Func<ContextualServiceProvider, object> ServiceFactory { get; }
    }
}