using System;
using System.Collections.Generic;
using System.Reflection;

namespace InjectReady.YouInject.Internal
{
    internal class ContextualServiceProvider
    {
        private readonly IContextualScope _scope;
        private readonly Stack<IServiceDescriptor> _requestStack;
        private int _singletonPosition;

        public ContextualServiceProvider(IContextualScope scope)
        {
            _scope = scope;
            _requestStack = new Stack<IServiceDescriptor>();
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
            _singletonPosition = 0;
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
            
            _requestStack.Push(descriptor);
            
            if (!IsSingleton && descriptor.Lifetime is ServiceLifetime.Singleton)
            {
                _singletonPosition = _requestStack.Count;
            }
        }

        private void PopService()
        {
            _requestStack.Pop();

            if (IsSingleton && _singletonPosition > _requestStack.Count)
            {
                _singletonPosition = 0;
            }
        }
    }
}