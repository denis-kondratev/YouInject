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

        public object ResolveService(Func<Type, object> serviceProvider)
        {
            throw new ServiceIsNotRegisteredException(ServiceType, "The service has not been yet added.");
        }

        public void BindComponent(Type componentType)
        {
            if (componentType == null) throw new ArgumentNullException(nameof(componentType));

            if (!Utility.IsComponentType(componentType))
            {
                throw new ServiceBindingException(
                    ServiceType,
                    componentType,
                    $"{componentType.FullName} is not derived from MonoBehaviour.");
            }

            if (Binding is not null)
            {
                throw new ServiceBindingException(
                    ServiceType,
                    componentType,
                    $"The service has already bound to '{Binding.FullName}'");
            }

            Binding = componentType;
        }
    }
}