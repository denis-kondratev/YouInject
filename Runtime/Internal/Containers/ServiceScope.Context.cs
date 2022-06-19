using System;
using System.Collections.Generic;

namespace YouInject.Internal
{
    internal partial class ServiceScope
    {
        public class Context
        {
            private readonly ServiceScope _serviceScope;
            private readonly Stack<Type> _requests;

            public Context(ServiceScope serviceScope)
            {
                _serviceScope = serviceScope;
                _requests = new Stack<Type>();
            }

            public object[] GetInitializingParameters(Type initializedServiceType, Type[] parameterTypes)
            {
                if (parameterTypes.Length == 0)
                {
                    return Array.Empty<object>();
                }
                
                PushService(initializedServiceType);
                var decisions = new object[parameterTypes.Length];

                for (var i = 0; i < decisions.Length; i++)
                {
                    decisions[i] = _serviceScope.GetService(parameterTypes[i], this);
                }
                
                PopService();
                return decisions;
            }

            public void Release()
            {
                _requests.Clear();
            }

            private void PushService(Type serviceType)
            {
                if (_requests.Contains(serviceType))
                {
                    var anotherType = _requests.Peek();
                    throw new InvalidOperationException($"'{serviceType.Name}' and '{anotherType.Name}' services refer on each other.");
                }
            
                _requests.Push(serviceType);
            }

            private void PopService()
            {
                _requests.Pop();
            }
        }
    }
}