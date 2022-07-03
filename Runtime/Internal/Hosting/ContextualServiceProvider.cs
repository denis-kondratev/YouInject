using System;
using System.Collections.Generic;
using System.Reflection;

namespace InjectReady.YouInject.Internal
{
    internal class ContextualServiceProvider
    {
        private readonly IContextualScope _scope;
        private readonly Stack<IServiceDescriptor> _requestStack;
        private int _singletonPositionInStack;
        private IServiceDescriptor? _singletonDescriptor;

        public ContextualServiceProvider(IContextualScope scope)
        {
            _scope = scope;
            _requestStack = new Stack<IServiceDescriptor>();
        }

        private bool HasSingletonInStack => _singletonDescriptor is not null;
        
        public object GetService(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));

            var descriptor = _scope.GetDescriptor(serviceType);
            PushService(descriptor);
            
            var lifetime = HasSingletonInStack ? ServiceLifetime.Singleton : descriptor.Lifetime;
            var container = _scope.GetContainer(lifetime);
            var service = container.GetService(descriptor, this);
            
            PopService();
            return service;
        }

        public object[] GetServices(Type[] types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));

            return GetServices(types, t => t);
        }
        
        public object[] GetServices(ParameterInfo[] types)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));

            return GetServices(types, p => p.ParameterType);
        }
        
        private object[] GetServices<T>(T[] types, Func<T, Type> getType)
        {
            if (types.Length == 0)
            {
                return Array.Empty<object>();
            }
            
            var services = new object[types.Length];

            for (var i = 0; i < services.Length; i++)
            {
                services[i] = GetService(getType(types[i]));
            }
            
            return services;
        }

        public void Release()
        {
            _singletonPositionInStack = 0;
            _requestStack.Clear();
        }

        private void PushService(IServiceDescriptor descriptor)
        {
            var serviceType = descriptor.ServiceType;
            
            if (_requestStack.Contains(descriptor))
            {
                var anotherType = _requestStack.Peek().ServiceType;
                throw new InvalidOperationException($"'{serviceType.Name}' and '{anotherType.Name}' services refer on each other.");
            }

            if (_singletonDescriptor is not null && descriptor is DynamicDescriptor { Lifetime: ServiceLifetime.Scoped })
            {
                throw new ServiceRegistrationException(
                    _singletonDescriptor, 
                    $"The singleton service '{_singletonDescriptor.ServiceType.FullName}' cannot refer to " +
                    $"the non-singleton dynamic service '{descriptor.ServiceType.FullName}'.");
            }
            
            _requestStack.Push(descriptor);

            if (HasSingletonInStack || descriptor.Lifetime is not ServiceLifetime.Singleton) return;
            
            _singletonPositionInStack = _requestStack.Count;
            _singletonDescriptor = descriptor;
        }

        private void PopService()
        {
            _requestStack.Pop();

            if (!HasSingletonInStack || _singletonPositionInStack <= _requestStack.Count) return;
            
            _singletonPositionInStack = 0;
            _singletonDescriptor = null;
        }
    }
}