using System;

namespace InjectReady.YouInject.Internal
{
    [Serializable]
    internal class ServiceRegistrationFailureException : InvalidOperationException
    {
        internal ServiceRegistrationFailureException(Type serviceType, string message) 
            : base($"Cannot register the '{serviceType.Name}' service.\n" + message)
        {
        }
    }
}