using System;

namespace YouInject
{
    internal abstract class ServiceSpecifier
    {
        public Type ServiceType { get; }
        public Type DecisionType { get; }
        public ServiceLifetime Lifetime { get; }

        protected abstract Type[] ParameterTypes { get; }

        protected ServiceSpecifier(Type serviceType, Type decisionType, ServiceLifetime lifetime)
        {
            ServiceType = serviceType;
            DecisionType = decisionType;
            Lifetime = lifetime;
        }
        
        internal abstract object MakeDecision(ServiceProvider serviceProvider);
    }
}