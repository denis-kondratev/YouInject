using System;

namespace InjectReady.YouInject.Internal
{
    [Serializable]
    internal class ServiceRegistrationException : Exception
    {
        private readonly IServiceDescriptor _serviceDescriptor;

        public ServiceRegistrationException(IServiceDescriptor serviceDescriptor, string message) : base(message)
        {
            _serviceDescriptor = serviceDescriptor;
        }

        public override string Message
        {
            get
            {
                var message = $"Incorrect registration of the '{_serviceDescriptor.ServiceType.FullName}' service.\n";
                return message + base.Message;
            }
            
        }
    }
}