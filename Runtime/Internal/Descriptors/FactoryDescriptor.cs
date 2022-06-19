using System;

namespace YouInject.Internal
{
    internal partial class FactoryDescriptor : IServiceDescriptor
    {
        internal FactoryDescriptor(Type factoryType, Type productType, ServiceLifetime lifetime)
        {
            if (factoryType.BaseType != typeof(MulticastDelegate))
            {
                throw new ArgumentException($"The '{factoryType.Name}' type is not a delegate.", nameof(factoryType));
            }

            ServiceType = factoryType;
            Lifetime = lifetime;
            InstanceFactory = GetInstanceFactory(factoryType, productType);
        }

        public Type ServiceType { get; }
        
        public ServiceLifetime Lifetime { get; }
        
        public Func<ServiceScope.Context, object> InstanceFactory { get; }
        
        private Func<ServiceScope.Context, object> GetInstanceFactory(Type factoryType, Type productType)
        {
            var factoryBuilder = new FactoryBuilder(factoryType, productType);
            
            return context =>
            {
                var factoryDelegate = factoryBuilder.GetFactoryDelegate(context);
                return factoryDelegate;
            };
        }
    }
}