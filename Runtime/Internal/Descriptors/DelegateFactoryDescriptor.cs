using System;

namespace InjectReady.YouInject.Internal
{
    internal partial class DelegateFactoryDescriptor : IConstructableServiceDescriptor
    {
        private readonly Type _productType;
        private readonly Type _delegateType;
        private readonly Func<ServiceProvider, ScopeContext, object> _instanceFactory;

        public Type ServiceType => _delegateType;
        public ServiceLifetime Lifetime { get; }

        internal DelegateFactoryDescriptor(Type delegateType, Type productType, ServiceLifetime lifetime)
        {
            if (delegateType.BaseType != typeof(MulticastDelegate))
            {
                throw new DelegateFactoryRegistrationException(delegateType, "The specified type is not a delegate.");
            }

            _productType = productType;
            _delegateType = delegateType;
            Lifetime = lifetime;
            _instanceFactory = CreateFactory();
        }

        public object ResolveService(ServiceProvider serviceProvider, ScopeContext scopeContext)
        {
            var service = _instanceFactory.Invoke(serviceProvider, scopeContext);
            return service;
        }

        private Func<ServiceProvider, ScopeContext, object> CreateFactory()
        {
            var factoryBuilder = new FactoryBuilder(_delegateType, _productType);
            
            return (provider, context) =>
            {
                var factoryDelegate = factoryBuilder.CreateFactoryDelegate(provider, context);
                return factoryDelegate;
            };
        }
    }
}