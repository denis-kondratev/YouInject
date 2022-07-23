using System;

namespace InjectReady.YouInject
{
    public interface IServiceScope : IExtendedServiceProvider, IAsyncDisposable
    {
        void AddScopedService(Type serviceType, object instance);
    }
}