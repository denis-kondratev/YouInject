using System;

namespace YouInject
{
    public interface IServiceScope : IAsyncDisposable
    {
        Action AddService(Type serviceType, object service);
        void RemoveScope(Type serviceType);
    }
}