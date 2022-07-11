using InjectReady.YouInject.Internal;
using UnityEngine;

namespace InjectReady.YouInject
{
    public readonly struct DynamicServiceRegistration
    {
        private readonly ServiceCollection _serviceCollection;
        private readonly DynamicDescriptor _descriptor;

        internal DynamicServiceRegistration(ServiceCollection serviceCollection, DynamicDescriptor descriptor)
        {
            _serviceCollection = serviceCollection;
            _descriptor = descriptor;
        }

        public ComponentDescriptorRegistration BindTo<T>() where T : MonoBehaviour
        {
            return _serviceCollection.BindServiceToComponent(_descriptor, typeof(T));
        }

        public void InitializeWith(string methodName)
        {
            _serviceCollection.InitializeComponentWith(_descriptor.ServiceType, methodName);
        }
    }
}