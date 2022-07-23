using System;

namespace InjectReady.YouInject.Internal
{
    [Serializable]
    internal class ServiceBindingFailureException : InvalidOperationException
    {
        public ServiceBindingFailureException(Type serviceType, Type componentType, string message)
            : base($"Cannot bind the '{serviceType.Name}' service to the '{componentType.Name}' component.\n" + message) 
        { }
        
        public ServiceBindingFailureException(Type serviceType, Type componentType, Exception innerException)
            : base($"Cannot bind the '{serviceType.Name}' service to the '{componentType.Name}' component.", innerException) 
        { }
    }
}