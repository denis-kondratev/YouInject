using System;

namespace InjectReady.YouInject.Internal
{
    internal class DynamicServiceDescriptor : IServiceDescriptor
    {
        public Type ServiceType { get; }

        public ComponentDescriptor? Binding { get; private set; }

        public DynamicServiceDescriptor(Type serviceType)
        {
            ServiceType = serviceType;
        }
        
        public void BindComponent(ComponentDescriptor componentDescriptor)
        {
            if (componentDescriptor == null) throw new ArgumentNullException(nameof(componentDescriptor));
            
            if (Binding is not null)
            {
                throw new InvalidOperationException(
                    $"Cannot bind the '{componentDescriptor.Type}' component to the '{ServiceType}' service. "
                    + $"The service has already bound to the '{Binding.Type.Name}' component.");
            }

            Binding = componentDescriptor;
        }
    }
}