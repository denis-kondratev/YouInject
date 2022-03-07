using System;

namespace YouInject
{
    public interface IScope : IDisposable
    {
        IServiceProvider GetServiceProvider();

        IComponentInjector GetComponentInjector();
        
        IScope CreateScope();
    }
}