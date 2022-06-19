using System;

namespace YouInject
{
    public interface IHost : IAsyncDisposable
    {
        IServiceProvider ServiceProvider { get; }
    }
}