using System;

namespace InjectReady.YouInject
{
    public interface IServiceScope : IAsyncDisposable
    {
        IExtendedServiceProvider ServiceProvider { get; }
    }
}