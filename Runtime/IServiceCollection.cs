using System;

namespace YouInject
{
    public interface IServiceCollection
    {
        void AddFactory(Type factoryType, Type productType, ServiceLifetime lifetime);
        void AddComponent(Type serviceType, Type instanceType, string initializingMethodName);
        void AddService(Type serviceType, Type instanceType, ServiceLifetime lifetime);
    }
}