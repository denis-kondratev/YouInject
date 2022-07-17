using System;

namespace InjectReady.YouInject.Internal
{
    [Serializable]
    internal class InvalidComponentOperationException : Exception
    {
        public InvalidComponentOperationException(Type componentType, string message) 
            : base($"Invalid operation with the '{componentType.Name}' component.\n" + message)
        {
        }
    }
}