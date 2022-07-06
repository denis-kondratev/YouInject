using System;
using UnityEngine;

namespace InjectReady.YouInject
{
    public static class ServiceScopeExtensions
    {
        public static void InitializeService(this IServiceScope scope, object service, string methodName)
        {
            var serviceType = service.GetType();
            var methodInfo = serviceType.GetMethod(methodName);

            if (methodInfo is null)
            {
                throw new ArgumentException($"Cannot find the method {methodName} in the type {serviceType.FullName}");
            }
            
            scope.InitializeService(service, methodInfo);
        }

        public static void InitializeAndAddService<T>(this IServiceScope scope, T service) where T : MonoBehaviour
        {
            scope.InitializeService(service);
            scope.AddService<T>(service);
        }
        
        public static void InitializeAndAddService<T>(this IServiceScope scope, object service)
        {
            var serviceType = typeof(T);
            scope.InitializeService(serviceType, service);
            scope.AddService(serviceType, service);
        }
    }
}