using System;

namespace InjectReady.YouInject.Internal
{
    internal partial class FactoryDescriptor : IServiceDescriptor
    {
        private readonly Type _productType;
        private readonly Func<ContextualServiceProvider, object> _instanceFactory;
        
        public Type ServiceType { get; }
        public ServiceLifetime Lifetime { get; }
        
        internal FactoryDescriptor(Type factoryType, Type productType, ServiceLifetime lifetime)
        {
            if (factoryType.BaseType != typeof(MulticastDelegate))
            {
                throw new ArgumentException($"The '{factoryType.Name}' type is not a delegate.", nameof(factoryType));
            }

            _productType = productType;
            ServiceType = factoryType;
            Lifetime = lifetime;
            _instanceFactory = GetInstanceFactory();
        }

        public object ResolveService(ContextualServiceProvider serviceProvider)
        {
            var service = _instanceFactory.Invoke(serviceProvider);
            return service;
        }

        private Func<ContextualServiceProvider, object> GetInstanceFactory()
        {
            var factoryBuilder = new FactoryBuilder(this);
            
            return serviceProvider =>
            {
                var factoryDelegate = factoryBuilder.GetFactoryDelegate(serviceProvider);
                return factoryDelegate;
            };
        }
    }
}