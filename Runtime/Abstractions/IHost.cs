using System;

namespace InjectReady.YouInject
{
    public interface IHost : IAsyncDisposable
    {
        IServiceProvider ServiceProvider { get; }
    }
}