using System;

namespace InjectReady.YouInject.Internal
{
    [Serializable]
    internal class InvalidServiceOperationException : Exception
    {
        public InvalidServiceOperationException(Type serviceType, string message) 
            : base($"Invalid operation with the '{serviceType.Name}' service.\n" + message)
        {
        }
    }
}