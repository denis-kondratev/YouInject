using System;
using InjectReady.YouInject.Internal;

namespace InjectReady.YouInject
{
    public readonly struct ComponentDescriptorRegistration
    {
        private readonly ServiceCollection _serviceCollection;
        private readonly Type _componentType;

        internal ComponentDescriptorRegistration(ServiceCollection serviceCollection, Type componentType)
        {
            _serviceCollection = serviceCollection;
            _componentType = componentType;
        }

        public void InitializeWith(string methodName)
        {
            _serviceCollection.InitializeComponentWith(_componentType, methodName);
        }
    }
}