using System;

namespace InjectReady.YouInject.Internal
{
    [Serializable]
    internal class ServiceRegistrationException : InvalidOperationException
    {
        internal ServiceRegistrationException(Type serviceType, string message) 
            : base($"Cannot register the '{serviceType.Name}' service.\n" + message)
        {
        }
    }
}