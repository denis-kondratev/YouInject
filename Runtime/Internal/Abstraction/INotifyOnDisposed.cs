using System;

namespace InjectReady.YouInject.Internal
{
    internal interface INotifyOnDisposed<out T>
    {
        event Action<T> Disposed;
    }
}