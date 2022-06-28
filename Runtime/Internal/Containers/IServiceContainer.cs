using System;

namespace YouInject.Internal
{
    internal interface IServiceContainer : IAsyncDisposable
    {
        object GetService(IServiceDescriptor descriptor, ContextualServiceProvider serviceProvider);
    }
}