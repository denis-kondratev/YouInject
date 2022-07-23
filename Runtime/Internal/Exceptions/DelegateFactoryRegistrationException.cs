using System;

namespace InjectReady.YouInject.Internal
{
    [Serializable]
    internal class DelegateFactoryRegistrationException : InvalidOperationException
    {
        internal DelegateFactoryRegistrationException(Type serviceType, string message) 
            : base($"Cannot register the '{serviceType.Name}' delegate factory.\n" + message)
        {
        }
    }
}