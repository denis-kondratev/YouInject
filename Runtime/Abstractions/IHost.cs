using System;

namespace InjectReady.YouInject
{
    public interface IHost : IAsyncDisposable
    {
        IRootServiceProvider ServiceProvider { get; }
    }
}