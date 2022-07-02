using System;
using System.Threading.Tasks;

namespace InjectReady.YouInject.Internal
{
    internal abstract class ServiceContainer : IServiceContainer
    {
        protected bool IsDisposed;
        
        public abstract ValueTask DisposeAsync();

        public abstract object GetService(IServiceDescriptor descriptor, ContextualServiceProvider serviceProvider);
        
        protected void ThrowIfDisposed()
        {
            if (IsDisposed) throw new InvalidOperationException("The Service Container is already disposed.");
        }
    }
}