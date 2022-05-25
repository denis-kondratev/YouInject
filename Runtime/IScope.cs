using System;

namespace YouInject
{
    public interface IScope : IDisposable
    {
        IServiceProvider ServiceProvider { get; }
    }
}