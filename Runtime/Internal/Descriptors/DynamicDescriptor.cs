using System;

namespace InjectReady.YouInject.Internal
{
    internal class DynamicDescriptor : IServiceDescriptor
    {
        public Type ServiceType { get; }
        public ServiceLifetime Lifetime { get; }

        public DynamicDescriptor(Type serviceType, bool isSingleton)
        {
            ServiceType = serviceType;
            Lifetime = isSingleton ? ServiceLifetime.Singleton : ServiceLifetime.Scoped;
        }

        public virtual object ResolveService(ContextualServiceProvider serviceProvider)
        {
            throw new OperationCanceledException($"The dynamic service '{ServiceType.FullName}' has not been yet added.");
        }
    }
}