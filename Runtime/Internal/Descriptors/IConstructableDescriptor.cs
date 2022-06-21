using System;

namespace YouInject.Internal
{
    internal interface IConstructableDescriptor : IServiceDescriptor
    {
        Func<ScopeContext, object> ServiceFactory { get; }
    }
}