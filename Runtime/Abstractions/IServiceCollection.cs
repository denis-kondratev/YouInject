using System;

namespace InjectReady.YouInject
{
    public interface IServiceCollection
    {
        void AddFactory(Type factoryType, Type productType, ServiceLifetime lifetime);
        void AddService(Type serviceType, Type instanceType, ServiceLifetime lifetime);
        void AddDynamicService(Type serviceType);
        void AddDynamicComponent(Type serviceType, string? initializingMethodName);
    }
}