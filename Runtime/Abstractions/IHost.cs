using System;

namespace InjectReady.YouInject
{
    public interface IHost : IAsyncDisposable
    {
        IExtendedServiceProvider ServiceProvider { get; }
    }
}