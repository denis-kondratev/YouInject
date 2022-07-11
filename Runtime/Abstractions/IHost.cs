using System;

namespace InjectReady.YouInject
{
    public interface IHost : IAsyncDisposable
    {
        IServiceScope RootScope { get; }
    }
}