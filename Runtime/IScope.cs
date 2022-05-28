using System;

namespace YouInject
{
    public interface IScope : IAsyncDisposable
    {
        IServiceProvider ServiceProvider { get; }
        IScope CreateDerivedServiceScope(string name);
    }
}