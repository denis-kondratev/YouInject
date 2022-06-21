using System;

namespace YouInject.Internal
{
    internal partial class FactoryDescriptor : IConstructableDescriptor
    {
        private readonly Type _productType;

        internal FactoryDescriptor(Type factoryType, Type productType, ServiceLifetime lifetime)
        {
            if (factoryType.BaseType != typeof(MulticastDelegate))
            {
                throw new ArgumentException($"The '{factoryType.Name}' type is not a delegate.", nameof(factoryType));
            }

            _productType = productType;
            ServiceType = factoryType;
            Lifetime = lifetime;
            ServiceFactory = GetInstanceFactory();
        }

        public Type ServiceType { get; }
        
        public ServiceLifetime Lifetime { get; }
        
        public Func<ScopeContext, object> ServiceFactory { get; }
        
        private Func<ScopeContext, object> GetInstanceFactory()
        {
            var factoryBuilder = new FactoryBuilder(this);
            
            return context =>
            {
                var factoryDelegate = factoryBuilder.GetFactoryDelegate(context);
                return factoryDelegate;
            };
        }
    }
}