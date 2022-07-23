using InjectReady.YouInject.Internal;
using UnityEngine;

namespace InjectReady.YouInject
{
    public readonly struct DynamicServiceRegistration
    {
        private readonly ServiceCollection _serviceCollection;
        private readonly DynamicServiceDescriptor _serviceDescriptor;

        internal DynamicServiceRegistration(ServiceCollection serviceCollection, DynamicServiceDescriptor serviceDescriptor)
        {
            _serviceCollection = serviceCollection;
            _serviceDescriptor = serviceDescriptor;
        }

        public ComponentDescriptorRegistration BindTo<T>() where T : MonoBehaviour
        {
            return _serviceCollection.BindServiceToComponent(_serviceDescriptor, typeof(T));
        }

        public void InitializeWith(string methodName)
        {
            _serviceCollection.InitializeComponentWith(_serviceDescriptor.ServiceType, methodName);
        }
    }
}