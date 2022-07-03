using System;

namespace InjectReady.YouInject
{
    public interface IServiceCollection
    {
        void AddFactory(Type factoryType, Type productType, ServiceLifetime lifetime);
        void AddService(Type serviceType, Type instanceType, ServiceLifetime lifetime);
        void AddDynamicService(Type serviceType, bool isSingleton);
        void AddDynamicComponent(Type serviceType, bool isSingleton, string? initializingMethodName);
    }
}