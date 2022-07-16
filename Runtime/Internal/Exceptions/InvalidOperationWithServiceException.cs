using System;

namespace InjectReady.YouInject.Internal
{
    [Serializable]
    internal class InvalidOperationWithServiceException : Exception
    {
        public InvalidOperationWithServiceException(Type serviceType, string message) 
            : base($"Invalid operation with the '{serviceType.Name}' service.\n" + message)
        {
        }
    }
}