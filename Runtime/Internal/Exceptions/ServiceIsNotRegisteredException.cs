using System;

namespace InjectReady.YouInject.Internal
{
    [Serializable]
    internal class ServiceIsNotRegisteredException : InvalidOperationException
    {
        public ServiceIsNotRegisteredException(Type serviceType)
            : base($"The '{serviceType.Name}' service is not registered.") 
        { }
        
        public ServiceIsNotRegisteredException(Type serviceType, string message)
            : base(message + $"\nThe '{serviceType.Name}' service is not registered.") 
        { }
    }
}