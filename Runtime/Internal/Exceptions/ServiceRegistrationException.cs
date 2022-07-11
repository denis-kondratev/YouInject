using System;

namespace InjectReady.YouInject.Internal
{
    [Serializable]
    internal class ServiceRegistrationException : Exception
    {
        private readonly Type _serviceType;

        public ServiceRegistrationException(Type serviceType, string message) : base(message)
        {
            _serviceType = serviceType;
        }

        public override string Message
        {
            get
            {
                var message = $"Incorrect registration of the '{_serviceType.FullName}' service.\n";
                return message + base.Message;
            }
            
        }
    }
}