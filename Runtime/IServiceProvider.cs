using System;

namespace YouInject
{
    public interface IServiceProvider : IAsyncDisposable
    {
        T Resolve<T>();
    }
}