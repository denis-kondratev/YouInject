using System;

namespace InjectReady.YouInject.Internal
{
    internal partial class DelegateFactoryDescriptor : IServiceDescriptor
    {
        private readonly Type _productInstanceType;
        private readonly Func<ServiceProvider, ScopeContext, object> _instanceFactory;
        
        public Type ServiceType { get; }
        public ServiceLifetime Lifetime { get; }

        internal DelegateFactoryDescriptor(Type delegateType, Type productInstanceType, ServiceLifetime lifetime)
        {
            if (delegateType.BaseType != typeof(MulticastDelegate))
            {
                throw new ArgumentException($"The '{delegateType.Name}' type is not a delegate.", nameof(delegateType));
            }

            _productInstanceType = productInstanceType;
            ServiceType = delegateType;
            Lifetime = lifetime;
            _instanceFactory = GetInstanceFactory();
        }

        public object ResolveService(ServiceProvider serviceProvider, ScopeContext scopeContext)
        {
            var service = _instanceFactory.Invoke(serviceProvider, scopeContext);
            return service;
        }

        private Func<ServiceProvider, ScopeContext, object> GetInstanceFactory()
        {
            var factoryBuilder = new FactoryBuilder(this);
            
            return (provider, context) =>
            {
                var factoryDelegate = factoryBuilder.CreateFactoryDelegate(provider, context);
                return factoryDelegate;
            };
        }
    }
}