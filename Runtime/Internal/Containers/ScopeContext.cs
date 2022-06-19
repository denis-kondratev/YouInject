using System;
using System.Collections.Generic;

namespace YouInject.Internal
{
    internal class ScopeContext
    {
        private readonly IContextualServiceScope _scope;
        private readonly Stack<Type> _requests;
        private int _singletonPosition;

        public ScopeContext(IContextualServiceScope scope)
        {
            _scope = scope;
            _requests = new Stack<Type>();
        }

        private bool IsSingleton => _singletonPosition > 0;
        
        public object GetService(Type serviceType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));

            var descriptor = _scope.GetDescriptor(serviceType);
            PushService(descriptor);
            var lifetime = IsSingleton ? ServiceLifetime.Singleton : descriptor.Lifetime;
            var container = _scope.GetContainer(lifetime);
            var service = container.GetService(descriptor, this);
            PopService();
            return service;
        }
        
        public object[] GetServices(Type[] parameterTypes)
        {
            if (parameterTypes == null) throw new ArgumentNullException(nameof(parameterTypes));
            
            if (parameterTypes.Length == 0)
            {
                return Array.Empty<object>();
            }
            
            var decisions = new object[parameterTypes.Length];

            for (var i = 0; i < decisions.Length; i++)
            {
                decisions[i] = GetService(parameterTypes[i]);
            }
            
            return decisions;
        }

        public void Release()
        {
            _requests.Clear();
        }

        private void PushService(IServiceDescriptor descriptor)
        {
            var serviceType = descriptor.ServiceType;
            if (_requests.Contains(serviceType))
            {
                var anotherType = _requests.Peek();
                throw new InvalidOperationException($"'{serviceType.Name}' and '{anotherType.Name}' services refer on each other.");
            }
            
            _requests.Push(serviceType);
            
            if (!IsSingleton && descriptor.Lifetime is ServiceLifetime.Singleton)
            {
                _singletonPosition = _requests.Count;
            }
        }

        private void PopService()
        {
            _requests.Pop();

            if (IsSingleton && _singletonPosition < _requests.Count)
            {
                _singletonPosition = 0;
            }
        }
    }
}