using System;
using System.Threading.Tasks;

namespace YouInject.Internal
{
    internal abstract class ServiceContainer : IServiceContainer
    {
        protected bool IsDisposed;
        
        public abstract ValueTask DisposeAsync();

        public abstract object GetService(IServiceDescriptor descriptor, ContextualServiceProvider serviceProvider);
        
        protected static object CreateService(IServiceDescriptor descriptor, ContextualServiceProvider context)
        {
            return descriptor switch
            {
                IConstructableDescriptor constructable => constructable.ServiceFactory.Invoke(context),
                DynamicDescriptor => throw new InvalidOperationException($"The dynamic service '{descriptor.ServiceType.Name}' is not yet added."),
                _ => throw new InvalidOperationException($"The service '{descriptor.ServiceType.Name}' cannot be created.")
            };
        }
        
        protected void ThrowIfDisposed()
        {
            if (IsDisposed) throw new InvalidOperationException("The Service Container is already disposed.");
        }
    }
}