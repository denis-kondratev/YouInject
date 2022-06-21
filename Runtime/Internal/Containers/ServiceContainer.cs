using System;
using System.Threading.Tasks;

namespace YouInject.Internal
{
    internal abstract class ServiceContainer : IServiceContainer
    {
        protected bool IsDisposed;
        
        public abstract ValueTask DisposeAsync();

        public abstract object GetService(IServiceDescriptor descriptor, ScopeContext context);
        
        protected static object CreateService(IServiceDescriptor descriptor, ScopeContext context)
        {
            switch (descriptor)
            {
                case IConstructableDescriptor constructable:
                    return constructable.ServiceFactory.Invoke(context);
                default:
                    throw new InvalidOperationException($"The service '{descriptor.ServiceType}' cannot be created.");
            }
        }
        
        protected void ThrowIfDisposed()
        {
            if (IsDisposed) throw new InvalidOperationException("The Service Container is already disposed.");
        }
    }
}