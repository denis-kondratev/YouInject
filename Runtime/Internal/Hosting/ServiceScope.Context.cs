using System;

namespace YouInject.Internal
{
    internal abstract partial class ServiceScope
    {
        private struct Context : IDisposable
        {
            private ServiceScope _serviceScope;
            
            public Context(ServiceScope serviceScope)
            {
                _serviceScope = serviceScope;
                if (!serviceScope._contextPool.TryPop(out var serviceProvider))
                {
                    serviceProvider = new ContextualServiceProvider(serviceScope);
                }

                ServiceProvider = serviceProvider;
            }
            
            public ContextualServiceProvider ServiceProvider { get; private set; }
            
            public void Dispose()
            {
                ServiceProvider.Release();
                _serviceScope._contextPool.Push(ServiceProvider);
                ServiceProvider = null!;
                _serviceScope = null!;
            }
        }
    }
}