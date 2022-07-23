using System;

namespace InjectReady.YouInject.Internal
{
    [Serializable]
    internal class ComponentIsNotRegisteredException : InvalidOperationException
    {
        public ComponentIsNotRegisteredException(Type componentType, string message)
            : base(message + $"\nThe '{componentType.Name}' component is not registered.") 
        { }
    }
}