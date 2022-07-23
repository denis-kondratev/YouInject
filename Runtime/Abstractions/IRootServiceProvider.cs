using System;

namespace InjectReady.YouInject
{
    public interface IRootServiceProvider : IExtendedServiceProvider
    {
        void AddSingletonService(Type serviceType, object instance);
    }
}