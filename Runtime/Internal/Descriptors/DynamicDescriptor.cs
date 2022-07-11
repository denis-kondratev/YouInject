using System;

namespace InjectReady.YouInject.Internal
{
    internal class DynamicDescriptor : IServiceDescriptor
    {
        public Type ServiceType { get; }
        public ServiceLifetime Lifetime { get; }
        public Type? Binding { get; private set; }

        public DynamicDescriptor(Type serviceType, bool isSingleton)
        {
            ServiceType = serviceType;
            Lifetime = isSingleton ? ServiceLifetime.Singleton : ServiceLifetime.Scoped;
        }

        public void BindComponent(Type componentType)
        {
            if (componentType == null) throw new ArgumentNullException(nameof(componentType));

            if (!Utility.IsComponentType(componentType))
            {
                throw new ArgumentException(
                    $"Cannot bind the '{ServiceType.FullName}' service to '{componentType.FullName}' type. "
                    + $"{componentType.FullName} is not derived from MonoBehaviour.",
                    nameof(componentType));
            }

            if (Binding is not null)
            {
                throw new InvalidOperationException(
                    $"Cannot bind the '{ServiceType.FullName}' service to '{componentType.FullName}' type. "
                    + $"The service has already bound to '{Binding.FullName}'");
            }

            Binding = componentType;
        }
        
        public virtual object ResolveService(ContextualServiceProvider serviceProvider)
        {
            throw new OperationCanceledException($"The dynamic service '{ServiceType.FullName}' has not been yet added.");
        }
    }
}