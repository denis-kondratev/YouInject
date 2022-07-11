using System;

namespace InjectReady.YouInject
{
    public interface IServiceCollection
    {
        void AddDelegateFactory(Delegate delegateType, Type productType, ServiceLifetime lifetime);
        void AddService(Type serviceType, Type implementationType, ServiceLifetime lifetime);
        void AddDynamicService(Type serviceType, bool isSingleton);
        void BindMonoBehaviourToService(Type monoBehaviourType, Type serviceType);
        void AddMonoBehaviourInitialization(Type monoBehaviourType, string? initializingMethodName);
    }
}