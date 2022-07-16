using System;

namespace InjectReady.YouInject.Internal
{
    [Serializable]
    internal class ServiceResolvingException : InvalidOperationException
    {
        public ServiceResolvingException(Type serviceType, string message)
            : base($"Cannot resolve the '{serviceType.Name}' service.\n" + message) 
        { }
    }
}