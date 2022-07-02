using System;
using System.Reflection;

namespace YouInject
{
    public interface IServiceScope : IServiceProvider, IAsyncDisposable
    {
        void AddService(Type serviceType, object service);
        void RemoveService(Type serviceType);
        void InitializeService(Delegate initializeDelegate);
        void InitializeService(object service, MethodInfo methodInfo);
    }
}