using System;

namespace InjectReady.YouInject.Internal
{
    internal class DynamicDescriptor : IServiceDescriptor
    {
        public Type ServiceType { get; }
        public ServiceLifetime Lifetime { get; }

        public ComponentDescriptor? Binding { get; private set; }

        public DynamicDescriptor(Type serviceType, bool isSingleton)
        {
            ServiceType = serviceType;
            Lifetime = isSingleton ? ServiceLifetime.Singleton : ServiceLifetime.Scoped;
        }

        public object ResolveService(ServiceProvider serviceProvider, ScopeContext scopeContext)
        {
            throw new ServiceIsNotRegisteredException(ServiceType, "The service has not been yet added.");
        }

        public void BindComponent(ComponentDescriptor componentDescriptor)
        {
            if (componentDescriptor == null) throw new ArgumentNullException(nameof(componentDescriptor));
            
            if (Binding is not null)
            {
                throw new ServiceBindingException(
                    ServiceType,
                    componentDescriptor.Type,
                    $"The service has already bound to '{Binding.Type.Name}'.");
            }

            Binding = componentDescriptor;
        }
    }
}