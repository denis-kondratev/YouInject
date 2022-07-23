using System;

namespace InjectReady.YouInject
{
    public interface IServiceCollection
    {
        void AddDelegateFactory(Type delegateFactoryType, Type productInstanceType, ServiceLifetime lifetime);
        void AddService(Type serviceType, Type implementationType, ServiceLifetime lifetime);
        DynamicServiceRegistration AddDynamicService(Type serviceType);
        void InitializeComponentWith(Type componentType, string methodName);
    }
}