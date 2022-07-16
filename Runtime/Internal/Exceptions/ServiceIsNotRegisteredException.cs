using System;

namespace InjectReady.YouInject.Internal
{
    [Serializable]
    internal class ServiceIsNotRegisteredException : InvalidOperationException
    {
        public ServiceIsNotRegisteredException(Type serviceType, string message)
            : base($"The '{serviceType.Name}' service is not registered.\n" + message) 
        { }
    }
}