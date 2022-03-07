using System;

namespace YouInject
{
    public interface IHost : IDisposable
    {
        IScope RootScope { get; }
    }
}