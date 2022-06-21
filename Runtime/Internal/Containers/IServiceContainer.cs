using System;

namespace YouInject.Internal
{
    internal interface IServiceContainer : IAsyncDisposable
    {
        object GetService(IServiceDescriptor descriptor, ScopeContext context);
        void AddService(IServiceDescriptor descriptor, object service);
        void RemoveService(IServiceDescriptor descriptor);
    }
}