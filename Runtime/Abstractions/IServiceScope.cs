using System;

namespace YouInject
{
    public interface IServiceScope : IServiceProvider, IAsyncDisposable
    {
        void AddService(Type serviceType, object service);
        void RemoveService(Type serviceType);
        void InitializeComponent(Delegate initializeDelegate);
    }
}